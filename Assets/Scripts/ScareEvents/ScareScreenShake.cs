using System.Collections;
using UnityEngine;

public class ScareScreenShake : MonoBehaviour, IScareEvent
{
     [SerializeField] private float eventFreqGain;
     [SerializeField] private float eventAmpGain;
     [SerializeField] private float duration;
     [SerializeField] private float rumbleDuration;
     private CameraManager _cameraManager;
     private AudioSource _audioSource;

     private void Start()
     {
          _cameraManager = FindFirstObjectByType<CameraManager>();
          _audioSource = GetComponent<AudioSource>();
     }

     public void TriggerScareEvent()
     {
          _cameraManager.SetCameraShake(eventFreqGain, eventAmpGain, duration);
          if (_audioSource)
          {
               StartCoroutine(PlayRumbleAudioWithFadeOut(rumbleDuration));
          }
     }

     IEnumerator PlayRumbleAudioWithFadeOut(float duration)
     {
          float timeElapsed = 0.0f;
          _audioSource.volume = 1.0f;
          _audioSource.Play();

          while (timeElapsed < duration)
          {
               _audioSource.volume = Mathf.Lerp(1.0f, 0.0f, timeElapsed / duration);
               timeElapsed += Time.deltaTime;
               yield return null;
          }
          _audioSource.Stop();
     }
}
