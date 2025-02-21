using System;
using System.Collections;
using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    public PlayerController player;
    private bool _onGround;
    private bool _didNotLeaveGround;

    private void Start()
    {
        _onGround = false;
    }
    
    private IEnumerator DelayGroundCheck()
    {
        yield return new WaitForSeconds(0.1f);
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
        if (other.gameObject.CompareTag("Floor"))
        {
            StartCoroutine(DelayGroundCheck());
            if (!_didNotLeaveGround)
            {
                player.GetRigidBody().linearDamping = 1;
                _onGround = false;
            }
        }
    }

    public bool IsOnGround()
    {
        return _onGround;
    }
}
