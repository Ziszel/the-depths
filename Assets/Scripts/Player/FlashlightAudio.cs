using System.Collections;
using UnityEngine;

public class FlashlightAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource crankAudio;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip windupClip;
    [SerializeField] private AudioClip toggleClip;

    [SerializeField] private float windupVolumeNormalised = 0.1f;
    [SerializeField] private float toggleVolumeNormalised = 0.5f;

    [Header("Audio parameters")] [SerializeField] private float windDownTime;
    
    void Start()
    {
        crankAudio.clip = windupClip;
        crankAudio.volume = toggleVolumeNormalised;
    }

    public IEnumerator PlayFlashlightOnSound()
    {
        crankAudio.PlayOneShot(toggleClip);
        yield return new WaitForSeconds(toggleClip.length);
        crankAudio.volume = windupVolumeNormalised;
        crankAudio.Play();
    }
    
    public IEnumerator FadeOutFlashlightAudio()
    {
        // Gradually reduce the volume
        for (float t = 0; t < windDownTime; t += Time.deltaTime)
        {
            crankAudio.volume = Mathf.Lerp(windupVolumeNormalised, 0.0f, t / windDownTime);
            yield return null;
        }

        // Ensure the volume is set to 0
        crankAudio.volume = 0;
        crankAudio.Stop();
        
        // Reset the volume and play the toggle clip
        crankAudio.volume = toggleVolumeNormalised;
        crankAudio.PlayOneShot(toggleClip);
    }
}
