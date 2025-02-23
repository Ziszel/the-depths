using UnityEngine;

public class InteractAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip interactClip;

    private void Start()
    {
        sfxSource.clip = interactClip;
    }

    public void PlaySfx()
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.Play();
        }
    }
}
