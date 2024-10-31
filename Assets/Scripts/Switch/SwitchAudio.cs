using UnityEngine;

public class SwitchAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip _switchBeingPulled;

    private void Start()
    {
        sfxSource.clip = _switchBeingPulled;
    }

    public void PlaySfx()
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.Play();
        }
    }
}
