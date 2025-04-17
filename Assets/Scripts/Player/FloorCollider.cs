using System.Collections;
using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    public PlayerController player;
    [SerializeField] private AudioClip landingGroundClip;
    [SerializeField] private float longFlightTimeThreshold;
    
    private bool _onGround;
    
    private GlobalSFXPlayer _globalSfxPlayer;
    private bool _isHighFall;

    private void Start()
    {
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        _onGround = false;
        _isHighFall = false;
    }
    
    private IEnumerator LongFlightTime()
    {
        yield return new WaitForSeconds(longFlightTimeThreshold);

        if (_onGround == false)
        {
            _isHighFall = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            if (_isHighFall)
            {
                _globalSfxPlayer.PlaySfx(landingGroundClip);
                player.SetPlayerValuesToWalk();
                player.SetPlayerState(playerActionState.Standing);
            }
            _isHighFall = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            if (!_onGround)
            {
                player.GetRigidBody().linearDamping = 5;
                _onGround = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            StartCoroutine(LongFlightTime());
            player.GetRigidBody().linearDamping = 1;
            _onGround = false;
        }
    }

    public bool IsOnGround()
    {
        return _onGround;
    }
}
