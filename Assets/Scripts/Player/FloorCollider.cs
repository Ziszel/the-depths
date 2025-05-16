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
            
            UpdateFootstepAudio(other.tag);
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
            UpdateFootstepAudio(other.tag);
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

    // Update footstep sounds
    // Runs when entering (takes new tag, updates _oldSound to _currentSound and then updates _currentSound)
    // Runs when exiting (compares if tag != _oldSound and then updates _currentSound to _oldSound)
    private void UpdateFootstepAudio(string newSound)
    {
        bool isCrouching = (player.GetPlayerActionState() == playerActionState.Crouching);
        bool isSprinting = (player.GetPlayerActionState() == playerActionState.Sprinting);
        
        if (newSound.Contains("Cement"))
        {
            playerAudio.SetFootstepsToCement(isCrouching, isSprinting);
        }
            
        if (newSound.Contains("Wood"))
        {
            playerAudio.SetFootstepsToWood(isCrouching, isSprinting);
        }

        if (newSound.Contains("Glass"))
        {
            playerAudio.SetFootstepsToGlass(isCrouching, isSprinting);
        }
            
        if (newSound.Contains("SolidSteel"))
        {
            playerAudio.SetFootstepsToSolidSteel(isCrouching, isSprinting);
        }
    }
}
