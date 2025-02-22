using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite interactableSprite;
    public PickupItem itemData;

    private Inventory _playerInventory;

    private void Start()
    {
        _playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
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
        }
                
        if (itemData.GetType() == typeof(FileItem))
        {
            _playerInventory.AddFile((FileItem)itemData);
        }
            
        // Clean-up
        Destroy(gameObject);
    }
}
