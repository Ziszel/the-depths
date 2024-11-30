using System.Collections;
using UnityEngine;

public class FlashlightAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource crankAudio;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip windupClip;

    [Header("Audio parameters")] [SerializeField] private float windDownTime;
    
    void Start()
    {
        crankAudio.clip = windupClip;
    }
    
    public void PlaySfx()
    {
        if (!crankAudio.isPlaying)
        {
            crankAudio.Play();
        }
    }

    public IEnumerator StopSfxRoutine()
    {
        yield return new WaitForSeconds(windDownTime);
        crankAudio.Stop();
    }
    
    public void StopSfx()
    {
        if (crankAudio.isPlaying)
        {
            StartCoroutine(StopSfxRoutine());
        }
    }
}
