using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI instance;
    private InventoryItem _selectedItem;
    private Inventory _inventory;
    
    // UI Elements
    private TMP_Text _goalTMP;
    private TMP_Text _selectedTMP;

    // , then show 
    public void ShowOnOpen(Inventory updatedInventory)
    {
        SetupInventory(updatedInventory);
        ShowInitialUI();
    }
    
    // UI Elements
    // Show initial inventory UI items when the player opens the inventory
    private void ShowInitialUI()
    {
        // Show initial UI
    }

    private void ShowFileUI()
    {
        // Show file selection
    }

    public void CloseInventory()
    {
        // hide all UI elements
    }

    // Update local variables and update UI based on those values
    private void SetupInventory(Inventory updatedInventory)
    {
        SetInventory(updatedInventory);
        //_goalTMP.text = _inventory.GetCurrentGoalText();
        Debug.Log(_inventory.GetCurrentGoalText());
    }
    
    private void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }
}
