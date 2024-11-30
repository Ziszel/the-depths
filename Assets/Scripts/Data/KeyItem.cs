using UnityEngine;

[CreateAssetMenu(fileName = "KeyItem", menuName = "Scriptable Objects/KeyItem")]
public class KeyItem : InventoryItem
{
    public override void UseItem()
    {
        Debug.Log("keys cannot be used from the inventory");
    }
}
