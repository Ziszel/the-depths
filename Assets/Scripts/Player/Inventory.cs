using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<InventoryItem> _items;
    private string _currentGoalText;
    private Dictionary<string, string> _files;

    private void Start()
    {
        _items = new List<InventoryItem>();
        _files = new Dictionary<string, string>();
        
        // testing
        _files.Add("File 1", "This is a test file to demonstrate functionality of the file menu within the inventory system.");
        _currentGoalText = "Test goal.";
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

    public string GetCurrentGoalText()
    {
        return _currentGoalText;
    }
}
