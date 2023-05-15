using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public MainManager Manager;

    private void Awake()
    {
        Manager = MainManager.Instance != null ? MainManager.Instance : FindObjectOfType<MainManager>(); 
        
    }
    private void OnCollisionEnter(Collision other)
    {
        Destroy(other.gameObject);
        if (FindObjectsOfType<Brick>().Length == 0) Manager.ContinuePlay();
        else                                        Manager.GameOver();
    }
}
