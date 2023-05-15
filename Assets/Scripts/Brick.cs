using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{
    public UnityEvent<int> onDestroyed;
    
    public int PointValue;

    void Start()
    {
        var renderer = GetComponentInChildren<Renderer>();

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        switch (PointValue)
        {
            case int i when          i <= 10:
                block.SetColor("_BaseColor", Color.green);
                break;
            case int i when i > 10  && i <= 12:
                block.SetColor("_BaseColor", Color.yellow);
                break;
            case int i when i > 12 && i <= 14:
                block.SetColor("_BaseColor", Color.blue);
                break;
            case int i when i > 14 && i <= 17:
                block.SetColor("_BaseColor", Color.red);
                break;
            default:
                block.SetColor("_BaseColor", new Color(
                                                Random.Range(0f, 1f),
                                                Random.Range(0f, 1f),
                                                Random.Range(0f, 1f)
                                                ));
                break;
        }
        renderer.SetPropertyBlock(block);
    }

    private void OnCollisionEnter(Collision other)
    {
        KillBlock();
    }

    public void KillBlock() 
    {
        onDestroyed.Invoke(PointValue);

        //slight delay to be sure the ball have time to bounce
        Destroy(gameObject, 0.2f);
    }
}
