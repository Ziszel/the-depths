using System;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private LevelManager _levelManager;

    public void Start()
    {
        _levelManager = FindAnyObjectByType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_levelManager.IsPlayerDead())
            {
                GameManager.instance.SetBestTime(LevelManager.GetTimer());
                GameManager.instance.LoadLevel("EndGame");
            }
        }
    }
}
