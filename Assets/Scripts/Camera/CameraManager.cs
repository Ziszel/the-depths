using UnityEngine;
using Unity.Cinemachine;
public class CameraManager : MonoBehaviour
{
    public CinemachineCamera[] cameras;
    
    public CinemachineCamera leftLeanCamera;
    public CinemachineCamera rightLeanCamera;
    public CinemachineCamera crouchCamera;
    public CinemachineCamera fpsCamera;

    public CinemachineCamera startCamera;
    private CinemachineCamera _currentCamera;

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
}
