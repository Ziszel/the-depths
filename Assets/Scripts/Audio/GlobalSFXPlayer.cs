using UnityEngine;

public class GlobalSFXPlayer : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioClip _clip;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void PlaySfx(AudioClip audioClip)
    {
        _clip = audioClip;
        _audioSource.clip = _clip;
        _audioSource.Play();
    }
}
