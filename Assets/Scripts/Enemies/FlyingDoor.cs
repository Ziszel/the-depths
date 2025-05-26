using System.Collections;
using UnityEngine;

public class FlyingDoor : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private float _flightTime;
    
    private AudioSource _audioSource;
    private bool _isFlying;

    private void Start()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        _isFlying = false;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (_isFlying && !tag.Equals("FloorWood"))
        {
            // Shouldn't need both but sometimes it was hitting player for some reason so this catches everything
            if (collision.gameObject.CompareTag("PlayerTrigger") ||collision.gameObject.CompareTag("Player") )
            {
                if (collision.gameObject.TryGetComponent(out PlayerController pc))
                {
                    pc.OnKillPlayer();
                }
            }
            else
            {
                _audioSource.PlayOneShot(_audioClip);
            }
        }
    }

    public void SetWalkableTag()
    {
        StartCoroutine(DelayBeforeApplyingTag(_flightTime));
    }

    public void SetFlyingDoor(bool isFlying)
    {
        _isFlying = isFlying;
    }

    private IEnumerator DelayBeforeApplyingTag(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        tag = "FloorWood";
        foreach (Transform child in transform)
        {
            child.tag = "FloorWood";
        }
        _isFlying = false;
    }

    public void SetInitialState()
    {
        _isFlying = false;
        foreach (Transform child in transform)
        {
            child.tag = null;
        }
    }
}
