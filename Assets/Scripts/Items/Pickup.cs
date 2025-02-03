using UnityEngine;

public class Pickup : MonoBehaviour
{
    public PickupItem itemData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
        }
    }
}
