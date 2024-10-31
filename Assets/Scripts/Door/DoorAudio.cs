using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip _doorOpening;

    private void Start()
    {
        sfxSource.clip = _doorOpening;
    }

    public void PlaySfx()
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.Play();
        }
    }
}
