using System.Collections;
using UnityEngine;

public class KnockbackDoorTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    
    private Level1Flags _level1Flags;
    private AudioSource _audioSource;
    
    private bool _hasTriggered;
    
    private void Start()
    {
        _level1Flags = FindFirstObjectByType<Level1Flags>();
        _audioSource = GetComponentInChildren<AudioSource>();
        _hasTriggered = false;
    }

    // Using an IEnumerator here is perhaps a little sloppy / overkill.
    // It'd be better use something similar to GlobalSFXPlayer to play a sound away from this object as it's destroyed
    // but that can take in a location to play it in 3D.
    private void OnTriggerEnter(Collider other)
    {
        if (_level1Flags.IsEventDoorKnocked && !_hasTriggered)
        {
            _audioSource.PlayOneShot(audioClip);
            _hasTriggered = true;
            StartCoroutine(DeleteAfterDelay());
        }
    }

    private IEnumerator DeleteAfterDelay()
    {
        yield return new WaitForSeconds(10.0f);
        
        Destroy(gameObject);
    }
}
