using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Scriptable Objects/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    [Tooltip("Presentation name of the item")]
    public string itemName;
    
    [Tooltip("UI description of the item")]
    public string itemDescription;
    
    [Tooltip("UI image represenation of the item")]
    public Sprite itemImage;
    
    [Tooltip("The associated prefab of the item")]
    public GameObject itemPrefab;

    // health items / files will affect the player from the inventory
    // the flashlight can be equipped (though this might be a one time thing which can't be unequipped)
    // keys won't do anything, etc...
    [Tooltip("Overwritten method, leave alone")]
    public virtual void UseItem()
    {
        Debug.unityLogger.Log("UseItem");
    }

}
