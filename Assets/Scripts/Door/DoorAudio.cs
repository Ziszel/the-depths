using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip doorOpening;
    [SerializeField] private AudioClip doorLocked;

    private void Start()
    {
        sfxSource.clip = doorOpening;
    }

    public void PlaySfx()
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.Play();
        }
    }

    public void SetDoorAudio(bool isOpen)
    {
        if (isOpen)
        {
            sfxSource.clip = doorOpening;
        }
        else
        {
            sfxSource.clip = doorLocked;
        }
    }
}
