using System.Collections;
using UnityEngine;

public class ScareLerpMovement : MonoBehaviour, IScareEvent, IResettable
{
    [SerializeField] private Vector3 newLocation;
    [SerializeField] private float lerpDuration;
    private Transform _transform;
    private Vector3 _startPosition;

    private void Start()
    {
        _transform = gameObject.GetComponent<Transform>();
        _startPosition = _transform.position;
    }
    public void TriggerScareEvent()
    {
        StartCoroutine("MoveToLocation");
    }

    IEnumerator MoveToLocation()
    {
        float time = 0;
        Vector3 startPos = _transform.position;

        while (time < lerpDuration)
        {
            _transform.position = Vector3.Lerp(startPos, newLocation, time / lerpDuration);
            time += Time.deltaTime;
            yield return null;
        }
        
        _transform.position = newLocation;
    }

    public void ResetObjectState()
    {
        _transform.position = _startPosition;
    }
}
