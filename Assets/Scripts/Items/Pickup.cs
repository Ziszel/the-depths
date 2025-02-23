using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    public PickupItem itemData;
    
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private AudioClip interactClip;

    private Inventory _playerInventory;
    private GlobalSFXPlayer _globalSfxPlayer;

    private void Start()
    {
        _playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
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
    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }

    public void AttemptToInteract()
    {
        if (itemData.GetType() == typeof(InventoryItem))
        {
            _playerInventory.AddItem((InventoryItem)itemData);
            _globalSfxPlayer.PlaySfx(interactClip);
        }
                
        if (itemData.GetType() == typeof(FileItem))
        {
            _playerInventory.AddFile((FileItem)itemData);
            _globalSfxPlayer.PlaySfx(interactClip);
        }
            
        // Clean-up
        Destroy(gameObject);
    }
}
