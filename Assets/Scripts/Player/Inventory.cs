using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> _items;
    private string _currentGoalText;
    private List<FileData> _foundByPlayerFileData;

    private void Start()
    {
        _items = new List<InventoryItem>();
    }

    public void AddItem(InventoryItem item)
    {
        _items.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        _items.Remove(item);
    }

    public void UseItem(InventoryItem item)
    {
        item.UseItem();
    }

    public List<InventoryItem> GetItems()
    {
        return _items;
    }

    public InventoryItem GetItemByIndex(int index)
    {
        return _items[index];
    }

    public string GetCurrentGoalText()
    {
        return _currentGoalText;
    }
}
