using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    private AudioSource _musicSource;

    private void Start()
    {
        _musicSource = GetComponent<AudioSource>();
        _musicSource.volume = _musicSource.volume;
    }

    public void PlayMusic(AudioClip musicToPlay, float fadeDuration, float volume)
    {
        if (_musicSource.isPlaying)
        {
            StartCoroutine(FadeOutAndPlayMusic(fadeDuration, musicToPlay));
        }
        else
        {
            _musicSource.clip = musicToPlay;
            _musicSource.volume = volume;
            _musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (_musicSource.isPlaying)
        {
            _musicSource.Stop();
        }
    }

    public void StopMusicWithDelay(float fadeDuration)
    {
        StartCoroutine(FadeOutAndStop(fadeDuration));
    }
    
    // Possibly wrap these two functions into one another?
    private IEnumerator FadeOutAndStop(float fadeDuration)
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
    
    private IEnumerator FadeOutAndPlayMusic(float fadeDuration, AudioClip musicToPlay)
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
        
        _musicSource.clip = musicToPlay;
        _musicSource.volume = 1.0f;
        _musicSource.Play();
    }
}
