using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    public CinemachineCamera[] cameras;
    
    public CinemachineCamera leftLeanCamera;
    public CinemachineCamera rightLeanCamera;
    public CinemachineCamera fpsCamera;

    public CinemachineCamera startCamera;
    private static CinemachineCamera _currentCamera;
    
    private const float BaseAmplitudeGain = 0.5f;
    private const float BaseFrequencyGain = 0.5f;

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

    public void SetCameraShake(float freqGain, float ampGain, float duration, float freqGainStart = BaseFrequencyGain, 
        float ampGainStart = BaseAmplitudeGain)
    {
        if (duration == 0)
        { 
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = freqGain;
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = ampGain;
            }
        }

        StartCoroutine(SetCameraShakeOverDuration(freqGain, ampGain, duration, freqGainStart, ampGainStart));
    }

    IEnumerator SetCameraShakeOverDuration(float freqGainTarget, float ampGainTarget, float duration, 
        float freqGainStart, float ampGainStart)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = Mathf.SmoothStep(freqGainStart, freqGainTarget, timeElapsed / duration);
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = Mathf.SmoothStep(ampGainStart, ampGainTarget, timeElapsed / duration);;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed = 0;
        
        while (timeElapsed < duration)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = Mathf.SmoothStep(freqGainTarget, freqGainStart, timeElapsed / duration);
                cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = Mathf.SmoothStep(ampGainTarget, ampGainStart, timeElapsed / duration);;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = BaseFrequencyGain;
            cameras[i].GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = BaseAmplitudeGain;
        }
    }
}
