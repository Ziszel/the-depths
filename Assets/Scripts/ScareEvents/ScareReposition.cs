using UnityEngine;

public class ScareReposition : MonoBehaviour, IScareEvent
{
    [SerializeField] private Vector3 newPosition;
    [SerializeField] private Vector3 newRotation;
    private Transform _transform;
    
    void Start()
    {
        _transform = GetComponent<Transform>();
    }

    public void TriggerScareEvent()
    {
        _transform.position = newPosition;
        _transform.rotation = Quaternion.Euler(newRotation);
    }
}
