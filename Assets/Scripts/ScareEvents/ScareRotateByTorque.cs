using UnityEngine;

public class ScareRotateByTorque : MonoBehaviour, IScareEvent
{
    [SerializeField] float torque;
    
    private Rigidbody _rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void TriggerScareEvent()
    {
        // Debug.Log("Applying torque");
        _rb.AddRelativeTorque(transform.forward * -torque, ForceMode.Impulse);
    }
}
