using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SocialPlatforms.Impl;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public static string PlayerName;


    [SerializeField] Brick BrickPrefab;
    [SerializeField] int LineCount = 6; 

    //cached references
    Rigidbody ball;
    TMP_InputField nameInput;
    Text scoreText;
    Text bestScoreText;
    public GameObject gameOverText;

    //variables
    HighScore highScore = new HighScore();
    bool onMenu;
    private bool m_GameOver = false; 
    private bool m_Started = false;
    private int m_Points;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance.StartGame();
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
            StartGame();
        }

    }
    public  void StartGame()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) onMenu = true;
        else onMenu = false;

        if (onMenu)
        {
            FindNameInput();
            bestScoreText = GameObject.Find("BestScoreText").GetComponent<Text>();
        }
        else
        {
            FindUI();
            Findball();
            CreatBricks();
            AddPoint(0);
        }

        bestScoreText.text = $"Best Score : {highScore.name} : {highScore.score}";
    }
    private void FindUI()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        gameOverText = GameObject.Find("GameoverText");
        gameOverText.SetActive(false);

        bestScoreText = GameObject.Find("BestScoreText").GetComponent<Text>();
        bestScoreText.text = $"Best Score : {highScore.name} : {highScore.score}";
    }
    private void FindNameInput()
    {
        nameInput = FindObjectOfType<TMP_InputField>();
    }
    private void CreatBricks()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 , 7, 7, 9, 9, 11, 11, 15, 15 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i] + LineCount;
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }
    private void Findball()
    {
        Ball myBall = FindAnyObjectByType<Ball>();

        if (myBall != null)
        {
            ball = myBall.GetComponent<Rigidbody>();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && onMenu) GetName();

        if (onMenu) return;

        if (!m_Started)
        {
            LaunchBall();
        }
        else if (m_GameOver)
        {
            ResetAfterGameOver();
        }

        Cheat();
    }

    private void Cheat()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            foreach(Brick brick in FindObjectsOfType<Brick>())
            {
                brick.KillBlock();
            }
        }
    }

    private void ResetAfterGameOver()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            m_GameOver = false;
            m_Points = 0;
            m_Started = false;
            LineCount = 2;
        }
    }

    private void LaunchBall()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Started = true;
            float randomDirection = Random.Range(-1.0f, 1.0f);
            Vector3 forceDir = new Vector3(randomDirection, 1, 0);
            forceDir.Normalize();

            ball.transform.SetParent(null);
            ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
        }
    }

    private void GetName()
    {
        PlayerName = nameInput.text;

        SceneManager.LoadScene(1);

    }
    void AddPoint(int point)
    {
        m_Points += point;
        scoreText.text = $"{PlayerName} Score : {m_Points}";
        CompareHighScore();
    }
    public void GameOver()
    {
        m_GameOver = true;
        gameOverText.SetActive(true);
        SaveHighScore();
    }
    public void ContinuePlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        m_Started = false;
        LineCount = Mathf.Clamp(LineCount + 1, LineCount, 9);
        Time.timeScale += 0.1f;
    }
    void SaveHighScore()
    {
        HighScore data = new HighScore();
        data.score = highScore.score;
        data.name = highScore.name;
        
        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/highScore.json", json);
    }
    void LoadHighScore()
    {

        string path = Application.persistentDataPath + "/highScore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            HighScore data = JsonUtility.FromJson<HighScore>(json);

            highScore.name = data.name;
            highScore.score = data.score;
        }

    }
    void CompareHighScore()
    {
        if(m_Points > highScore.score)
        {
            highScore.score = m_Points;
            highScore.name = PlayerName;
            bestScoreText.text = $"Best Score : {highScore.name} : {highScore.score}";
        }
    }
    [System.Serializable]
    class HighScore
    {
        public string name = "John";
        public int score = 2;
    }
}
