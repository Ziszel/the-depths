using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScareShake : MonoBehaviour, IScareEvent
{
    [SerializeField] private float timeBetweenShakes = 1.0f;
    [SerializeField] private float timeToShake = 0.5f;
    [SerializeField] private float shakeAmount;
    [SerializeField] private AudioClip bangingOnDoor;
    
    private Transform _objectToShake;
    private Vector3 _startingPosition;
    private Quaternion _startingRotation;
    private float _shakeDelay;
    private bool _waitingToShake;
    private AudioSource _audioSource;

    private void Start()
    {
        _objectToShake = GetComponent<Transform>();
        _audioSource = GetComponentInChildren<AudioSource>();
        _audioSource.clip = bangingOnDoor;
        _startingPosition = _objectToShake.position;
        _startingRotation = _objectToShake.rotation;
        _waitingToShake = false;
        enabled = false;
    }

    private void Update()
    {
        if (_waitingToShake)
        {
            _shakeDelay += Time.deltaTime;

            if (_shakeDelay >= timeBetweenShakes)
            {
                StartCoroutine(Shake());
                _shakeDelay = 0.0f;
                _waitingToShake = false;
            }
        }
    }
    
    public void TriggerScareEvent()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        // Shake for x amount of seconds and then stop for x amount of time, repeat until
        // t is greater than duration
        
        float t = 0.0f;
        _audioSource.Play();
        while (t < timeToShake)
        {
            t += Time.deltaTime;
            
            _objectToShake.position = _startingPosition + (Random.insideUnitSphere * shakeAmount);
            yield return null;
        }
        _objectToShake.position = _startingPosition;
        _waitingToShake = true;
    }

    private void OnDisable()
    {
        _waitingToShake = false;
    }

    public void SetInitialState()
    {
        _objectToShake.position = _startingPosition;
        _objectToShake.rotation = _startingRotation;
    }
}
