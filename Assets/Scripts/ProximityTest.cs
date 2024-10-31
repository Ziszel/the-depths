using System;
using UnityEngine;

public class ProximityTest : MonoBehaviour
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB; 
    [SerializeField] private float _speed = 2f;
    [SerializeField] private bool _testProximity = true;
    [SerializeField] private LightController[] lights;

    private Vector3 _target; 
    private float _proximityDistance = 7.0f;

    void Start()
    {
        Debug.Log("Lights found: " + lights.Length);
        _target = _pointB.position;
    }

    void Update()
    {
        Move();

        if (_testProximity)
        {
            foreach (LightController light in lights)
            {
                float distance = Vector3.Distance(transform.position, light.transform.position);

                if (distance < _proximityDistance)
                {
                    light.StartFlicker();  // Start flickering when the monster is close
                }
                else
                {
                    light.StopFlicker();  // Stop flickering when the monster moves away
                }
            }
        }
    }

    private void Move()
    {
        // Move towards the target point
        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);

        // Switch to the other point
        if (Vector3.Distance(transform.position, _target) < 1.0f)
        {
            _target = _target == _pointA.position ? _pointB.position : _pointA.position;  // Toggle target
        }
    }
}
