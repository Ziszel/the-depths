using UnityEngine;

public class ScarePush : MonoBehaviour, IScareEvent
{
    [SerializeField] private Vector3 forceToPush;
    [SerializeField] private bool isGravityActive;
    [SerializeField] private AudioClip audioClip;
    
    private Rigidbody _rb;
    private AudioSource _audioSource;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (gameObject.TryGetComponent(out AudioSource audioSource))
        {
            _audioSource = audioSource;
        }
    }
    
    public void TriggerScareEvent()
    {
        if (audioClip != null)
        {
            _audioSource.PlayOneShot(audioClip);
        }
        _rb.isKinematic = false; // used to stop the player pushing the object
        _rb.AddForce(forceToPush, ForceMode.Impulse);
        _rb.useGravity = isGravityActive;
    }
}
