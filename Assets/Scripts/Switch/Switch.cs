using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] private float switchMovementTime = 2.0f;
    [SerializeField] private Sprite interactableSprite;
    
    private Vector3 rotationVectorUp = new ( 0.0f, 0.0f, 60.0f );
    private Vector3 rotationVectorDown = new (0.0f, 0.0f, 125.0f);
    
    public Action<GameObject> SwitchAnimation;
    public List<GameObject> Switchables;
    public SoundTrigger soundTrigger;
    
    private Animator _animator;
    private PlayerController _player;
    private bool _isSwitchDown;
    private Transform _lever;
    private SwitchAudio _switchAudio;
    private LevelManager _levelManager;
    
    private void Start()
    {
        _isSwitchDown = false;
        _player = FindAnyObjectByType<PlayerController>();
        _lever = GetComponentsInChildren<Transform>().First(k => k.gameObject.name == "Lever");
        _switchAudio = GetComponent<SwitchAudio>();
        _levelManager = FindAnyObjectByType<LevelManager>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if(other.CompareTag("Player"))
        {
            _levelManager.ActivateInteractUI();
            _player.SetCurrentInteractable(this.gameObject);
        }*/
        
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /*if (other.CompareTag("Player"))
        {
            _levelManager.DeActivateInteractUI();
            _player.SetCurrentInteractable(null);
        }*/
        
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = false;
            playerInteractable.NoActivePickup();
        }
    }

    // IInteractable
    public void AttemptToInteract()
    {
        foreach (var Switchable in Switchables)
        {
            if (Switchable.TryGetComponent(out ISwitchable switchable))
            {
                _animator.SetBool("Pressed", true);
                //StartCoroutine(MoveSwitch());
                SwitchAnimation?.Invoke(this.gameObject);
                switchable.Toggle();
                _switchAudio.PlaySfx();
                this.soundTrigger.TriggerSound();
            }

            if (Switchable.TryGetComponent(out SoundTrigger soundTrigger))
            {
                soundTrigger.TriggerSound();
            }
        }
    }

    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }
}
