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

    private IEnumerator HitGroundReset()
    {
        yield return new WaitForSeconds(0.2f);
        _didNotLeaveGround = false;
    }
    
    private IEnumerator DelayGroundCheck()
    {
        yield return new WaitForSeconds(0.1f);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            if (_onGround)
            {
                _didNotLeaveGround = true;
                StartCoroutine(HitGroundReset());
            }
            Debug.Log("Now on ground");
            player.GetRigidBody().linearDamping = 5;
            _onGround = true;
        }
    }*/

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            if (!_onGround)
            {
                Debug.Log("Now on ground");
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
                Debug.Log("Now in the air");
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
