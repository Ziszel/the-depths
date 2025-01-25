using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI instance;
    private InventoryItem _selectedItem;
    private Inventory _inventory;
    
    // UI elements
    [Header("UI Elements")]
    [SerializeField] private GameObject horizontalMenu;

    private void Start()
    {
        
    }

    // Entry point from GameManager
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
        horizontalMenu.SetActive(true);
    }

    private void ShowStatusUI()
    {
        
    }

    private void ShowInventoryUI()
    {
        
    }

    private void ShowFileUI()
    {
        // Show file selection
    }

    public void CloseInventory()
    {
        // hide all UI elements (close all because we can't know which one the player can see)
        // Global
        horizontalMenu.SetActive(false);
        
        // Screen specific
    }

    // Update local variables
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
