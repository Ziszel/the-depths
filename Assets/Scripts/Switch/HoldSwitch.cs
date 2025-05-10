using System;
using UnityEngine;

public class HoldSwitch : MonoBehaviour, IInteractable
{
    public Action<GameObject> SwitchAnimation;
    public GameObject switchableObject;
    public SoundTrigger soundTrigger;
    
    [Header("Interactable settings (A/V")]
    [SerializeField] private Sprite interactableSpriteHand;
    [SerializeField] private Sprite interactableSpriteGear;
    [SerializeField] private AudioClip interactClip;

    [SerializeField] private InventoryItem requiredKeyItem;
    [SerializeField] private bool missingSwitch;
    
    private Animator _animator;
    private MeshRenderer _leverRenderer;
    private Sprite _interactableSprite;
    private bool _isSwitchDown;
    private Inventory _playerInventory;
    
    // AUDIO
    private GlobalSFXPlayer _globalSfxPlayer;
    
    private void Start()
    {
        _interactableSprite = interactableSpriteHand;
        _isSwitchDown = false;
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        _playerInventory = FindFirstObjectByType<Inventory>();
        _animator = GetComponentInChildren<Animator>(true);
        _leverRenderer = _animator.gameObject.GetComponent<MeshRenderer>();

        // Set both animator and level renderer to false for the lever to not be rendered (animation must be set first).
        if (missingSwitch)
        {
            _animator.enabled = false;
            _leverRenderer.enabled = false;
        }
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
        if (!_animator.enabled)
        {
            // Check if the player has the relevant item in their inventory.
            if (_playerInventory.ContainsItem(requiredKeyItem))
            {
                // TODO: Display text informing player of handle removed from inventory
                // Debug.Log("Key item in inventory, activate lever");
                UpdateInteractableSpriteToGear();
                // show the lever
                _animator.enabled = true;
                _leverRenderer.enabled = true;
                _playerInventory.RemoveItem(requiredKeyItem);
                return;
            }
            
            // TODO: Display text informing player of missing handle (update journal too)

            // Debug.Log("lever not rendered. Switch does not have lever!");
            return;
        }
        
        // if the switch hasn't been pressed, we can press it.
        // if the lever can be seen, it can be operated.
        if (!_isSwitchDown && _animator.enabled)
        {
            if (switchableObject.TryGetComponent(out ISwitchable switchable))
            {
                _animator.SetBool("Pressed", true);
                // TODO: check that without this, the lever still goes down
                // SwitchAnimation?.Invoke(this.gameObject);
                switchable.Toggle();
                _globalSfxPlayer.PlaySfx(interactClip);
                this.soundTrigger.TriggerSound();
                _isSwitchDown = true;
            }
        }

        if (switchableObject.TryGetComponent(out SoundTrigger soundTrigger))
        {
            soundTrigger.TriggerSound();
        }
    }

    private void UpdateInteractableSpriteToGear()
    {
        _interactableSprite = interactableSpriteGear;
    }

    public Sprite GetInteractableSprite()
    {
        return _interactableSprite;
    }
}
