using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip _footstepLeft;
    [SerializeField] private AudioClip _footstepRight;
    [SerializeField] private AudioClip _death;

    private void Start()
    {
        sfxSource.clip = _footstepLeft;
    }

    private IEnumerator SwapFeetWithDelay()
    {
        while (sfxSource.isPlaying)
        {
            yield return null;
        }
        
        if (sfxSource.clip == _footstepLeft)
        {
            sfxSource.clip = _footstepRight;
        }
        else
        {
            sfxSource.clip = _footstepLeft;
        }
    }

    public void SwapFeet()
    {
        StartCoroutine(SwapFeetWithDelay());
    }
    
    public void PlaySfx()
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.Play();
        }
    }

    public void PlayDeathSound()
    {
        sfxSource.PlayOneShot(_death);
    }
}
