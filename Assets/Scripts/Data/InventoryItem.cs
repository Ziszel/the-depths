using UnityEngine;

public enum InventoryItemType
{
    Item,
    File,
    Other
}

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
    
    [Tooltip("Which menu displays the item")]
    public InventoryItemType itemType;

    [Tooltip("File ID (only used for files)")]
    public int fileId;

    // health items / files will affect the player from the inventory
    // the flashlight can be equipped (though this might be a one time thing which can't be unequipped)
    // keys won't do anything, etc...
    [Tooltip("Overwritten method, leave alone")]
    public virtual void UseItem()
    {
        Debug.unityLogger.Log("UseItem");
    }

}
