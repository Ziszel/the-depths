using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Sprite interactableSprite;
    public PickupItem itemData;
    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(this);
        }
        Debug.Log("Item inside of Player item detector!");
        /*if (other.CompareTag("Player"))
        {
            if(other.TryGetComponent<Inventory>(out Inventory inventory))
            {
                if (itemData.GetType() == typeof(InventoryItem))
                {
                    inventory.AddItem((InventoryItem)itemData);
                }
                
                if (itemData.GetType() == typeof(FileItem))
                {
                    inventory.AddFile((FileItem)itemData);
                }
                
                Destroy(gameObject);
            }
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = false;
            playerInteractable.NoActivePickup();
        }
    }

    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }
}
