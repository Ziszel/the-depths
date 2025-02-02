using UnityEngine;

public class Pickup : MonoBehaviour
{
    public InventoryItem itemData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.TryGetComponent<Inventory>(out Inventory inventory))
            {
                if (itemData.itemType == InventoryItemType.Item)
                {
                    inventory.AddItem(itemData);
                }
                else if (itemData.itemType == InventoryItemType.File)
                {
                    inventory.AddFile(itemData);
                }
                
                Destroy(gameObject);
            }
        }
    }
}
