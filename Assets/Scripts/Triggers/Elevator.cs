using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public HorizontalDoor doorOne;
    public HorizontalDoor doorTwo;
    
    [SerializeField] private float wobbleAmplitude;
    [SerializeField] private float wobbleFrequency;
    [SerializeField] private float sequenceDuration = 5.0f;

    private PlayerController _player;
    private CinemachineCamera _camera;
    private CameraManager _cameraManager;
    private ElevatorAudio _elevatorAudio;

    private float _previousAmplitudeGain;
    private float _previousFrequencyGain;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _cameraManager = FindAnyObjectByType<CameraManager>();
        _elevatorAudio = GetComponent<ElevatorAudio>();
    }

    public IEnumerator ElevatorSequence()
    {
        float timeElapsed = 0.0f;
        _cameraManager.SetCameraShake(wobbleFrequency, wobbleAmplitude, sequenceDuration / 2, 
            wobbleFrequency, wobbleAmplitude);
        _player.DisableInputActions();
        _elevatorAudio.PlaySfx();

        while (timeElapsed < sequenceDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        // TODO: Make sure to set the correct audio on the doors used for the elevator!!
        doorOne.Toggle();
        doorTwo.Toggle();
        _elevatorAudio.StopSfx();
        _player.EnableInputActions();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            StartCoroutine(ElevatorSequence());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            Destroy(this);
        }
    }
}
