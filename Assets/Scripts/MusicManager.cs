using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _musicSource;

    [Header("Audio clips")]
    [SerializeField] private AudioClip _introMusic;
    [SerializeField] private AudioClip _wanderingMusic;
    [SerializeField] private AudioClip _chaseMusic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (_musicSource)
        {
            AssignIntroMusic();
        }
    }

    public void AssignIntroMusic()
    {
        _musicSource.clip = _introMusic;
    }
    public void AssignWanderingMusic()
    {
        _musicSource.clip = _wanderingMusic;
    }
    public void AssignChaseMusic()
    {
        _musicSource.clip = _chaseMusic;
    }
    public bool IsAssignedWanderingMusic()
    {
        return _musicSource.clip == _wanderingMusic;
    }

    public void Play()
    {
        _musicSource.volume = 1.0f;
        _musicSource.Play();
    }

    public bool IsPlaying()
    {
        return _musicSource.isPlaying;
    }

    // Update is called once per frame
    public void TriggerFadeOutMusic(float fadeDuration = 3.0f)
    {
        StartCoroutine(FadeOutMusic(fadeDuration));
    }

    private IEnumerator FadeOutMusic(float fadeDuration)
    {
        float startVolume = _musicSource.volume;

        // Gradually reduce the volume
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            _musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        // Ensure the volume is set to 0 at the end
        _musicSource.volume = 0;
        _musicSource.Stop(); // Optionally stop the music when it reaches 0 volume
    }
}
