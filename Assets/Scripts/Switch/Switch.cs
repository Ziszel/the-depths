using System;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    public Action<GameObject> SwitchAnimation;
    public List<GameObject> Switchables;
    public SoundTrigger soundTrigger;
    
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private AudioClip interactClip;
    
    private Animator _animator;
    private bool _isSwitchDown;
    
    // AUDIO
    private GlobalSFXPlayer _globalSfxPlayer;
    
    private void Start()
    {
        _isSwitchDown = false;
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = false;
            playerInteractable.NoActivePickup();
        }
    }

    // IInteractable
    public void AttemptToInteract()
    {
        // if the switch hasn't been pressed, we can press it.
        // TODO: If we need to interact more than once we need to update the code here
        if (!_isSwitchDown)
        {
            foreach (var Switchable in Switchables)
            {
                if (Switchable.TryGetComponent(out ISwitchable switchable))
                {
                    _animator.SetBool("Pressed", true);
                    SwitchAnimation?.Invoke(this.gameObject);
                    switchable.Toggle();
                    _globalSfxPlayer.PlaySfx(interactClip);
                    this.soundTrigger.TriggerSound();
                }

                if (Switchable.TryGetComponent(out SoundTrigger soundTrigger))
                {
                    soundTrigger.TriggerSound();
                }
            }

            if (!_isSwitchDown)
            {
                _isSwitchDown = true;
            }
            else
            {
                _isSwitchDown = false;
            }
        }
    }

    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }
}
