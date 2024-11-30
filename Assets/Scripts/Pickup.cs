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
                inventory.AddItem(itemData);
                Destroy(gameObject);
            }
        }
    }
}
