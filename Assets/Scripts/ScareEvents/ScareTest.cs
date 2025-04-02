using System;
using UnityEngine;

public class ScareTest : MonoBehaviour, IScareEvent
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void TriggerScareEvent()
    {
        _rigidbody.AddForce(new Vector3(0.0f, 10.0f, 0.0f), ForceMode.Impulse);
    }
}
