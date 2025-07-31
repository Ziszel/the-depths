using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    [SerializeField] private float baseAmplitudeGain;
    [SerializeField] private float baseFrequencyGain;
    
    public CinemachineCamera[] cameras;
    
    public CinemachineCamera leftLeanCamera;
    public CinemachineCamera rightLeanCamera;
    public CinemachineCamera fpsCamera;

    public CinemachineCamera startCamera;
    private static CinemachineCamera _currentCamera;

    private void Start()
    {
        _currentCamera = startCamera;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == _currentCamera)
            {
                cameras[i].Priority = 20;
            }
            else
            {
                cameras[i].Priority = 10;
            }
        }
    }

    public void SwitchCamera(CinemachineCamera newCamera)
    {
        _currentCamera.Priority = 10;
        _currentCamera = newCamera;
        _currentCamera.Priority = 20;
    }

    public static void ForceCurrentCameraRotation(Quaternion rotation)
    {
        // Rotation control stops camera rotating freely (pan tilt).
        // this overrides it but if smooth transitions are needed later, look to temp disable rotation control
        _currentCamera.ForceCameraPosition(_currentCamera.transform.position, rotation);
    }

    public void SetCameraShake(float freqGain, float ampGain, float duration)
    {
        if (duration == 0)
        { 
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = freqGain;
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = ampGain;
            }
        }

        StartCoroutine(SetCameraShakeOverDuration(freqGain, ampGain, duration));
    }

    IEnumerator SetCameraShakeOverDuration(float freqGainTarget, float ampGainTarget, float duration)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = Mathf.SmoothStep(baseFrequencyGain, freqGainTarget, timeElapsed / duration);
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = Mathf.SmoothStep(baseAmplitudeGain, ampGainTarget, timeElapsed / duration);;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed = 0;
        
        while (timeElapsed < duration)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = Mathf.SmoothStep(freqGainTarget, baseFrequencyGain, timeElapsed / duration);
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = Mathf.SmoothStep(ampGainTarget, baseAmplitudeGain, timeElapsed / duration);;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 0.0f;
            cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0.0f;
        }
    }
}
