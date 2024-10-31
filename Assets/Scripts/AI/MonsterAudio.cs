using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio clips")]
    [SerializeField] private List<AudioClip> patrolSounds;
    [SerializeField] private List<AudioClip> chaseSounds;

    private Monster _monster;

    private void Start()
    {
        _monster = GetComponentInParent<Monster>();
    }

    public void SetRandomClipFromPatrol()
    {
        sfxSource.clip = patrolSounds[Random.Range(0, patrolSounds.Count)];  
    }
    
    public void SetRandomClipFromChase()
    {
        sfxSource.clip = patrolSounds[Random.Range(0, patrolSounds.Count)];  
    }
    
    public void PlaySFX()
    {
        if (_monster.GetMonsterState() == Monster.MonsterState.Chase)
        {
            if (sfxSource.isPlaying)
            {
                sfxSource.Stop();
            }
            SetRandomClipFromChase();
        }
        else
        {
            SetRandomClipFromPatrol();
        }
        
        sfxSource.Play();
    }
}
