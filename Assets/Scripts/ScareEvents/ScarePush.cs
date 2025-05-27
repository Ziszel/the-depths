using UnityEngine;

public class ScarePush : MonoBehaviour, IScareEvent
{
    [SerializeField] private Vector3 forceToPush;
    [SerializeField] private bool isGravityActive;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;
    
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    public void TriggerScareEvent()
    {
        if (audioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
        _rb.isKinematic = false; // used to stop the player pushing the object
        _rb.AddForce(forceToPush, ForceMode.Impulse);
        _rb.useGravity = isGravityActive;
    }

    public void SetInitialState()
    {
        _rb.isKinematic = true;
        _rb.useGravity = !isGravityActive;
    }
}
