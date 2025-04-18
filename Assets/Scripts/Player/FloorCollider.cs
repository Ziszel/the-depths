using System.Collections;
using UnityEngine;

public class FloorCollider : MonoBehaviour
{
    [Header("External Components")]
    public PlayerController player;
    public PlayerAudio playerAudio;
    
    [SerializeField] private AudioClip landingGroundClip;
    [SerializeField] private float longFlightTimeThreshold;
    
    // Components
    private GlobalSFXPlayer _globalSfxPlayer;
    
    private bool _isHighFall;
    private bool _onGround;

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
        if (other.tag.Contains("Floor"))
        {
            if (_isHighFall)
            {
                _globalSfxPlayer.PlaySfx(landingGroundClip);
                player.SetPlayerValuesToWalk();
                player.SetPlayerState(playerActionState.Standing);
            }
            _isHighFall = false;
            
            // Update footstep sounds
            if (other.tag.Contains("Cement"))
            {
                Debug.Log("Walking on cement");
                playerAudio.SetFootstepsToCement();
            }
            
            if (other.tag.Contains("Wood"))
            {
                Debug.Log("Walking on wood");
                playerAudio.SetFootstepsToWood();
            }
            
            if (other.tag.Contains("SolidSteel"))
            {
                Debug.Log("Walking on solid steel");
                playerAudio.SetFootstepsToSolidSteel();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Contains("Floor"))
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
        if (other.tag.Contains("Floor"))
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
