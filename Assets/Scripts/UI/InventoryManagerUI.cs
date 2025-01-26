using UnityEngine;
using UnityEngine.UI;

public class InventoryManagerUI : MonoBehaviour
{
    private static InventoryManagerUI _instance;
    private InventoryItem _selectedItem;
    private Inventory _inventory;
    
    // UI elements
    [Header("UI Elements")]
    [SerializeField] private GameObject horizontalMenu;
    [SerializeField] private Image backgroundImage;
    private Button _statusButton;
    private Button _itemsButton;
    private Button _fileButton;

    private void Start()
    {
        AssociateButtons();
    }

    private void AssociateButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button b in buttons)
        {
            if (b.name == "StatusBtn")
            {
                _statusButton = b;
            }

            if (b.name == "ItemsBtn")
            {
                _itemsButton = b;
            }

            if (b.name == "FilesBtn")
            {
                _fileButton = b;
            }
        }
        
        _statusButton.onClick.AddListener(ShowStatusUI);
        _itemsButton.onClick.AddListener(ShowItemsUI);
        _fileButton.onClick.AddListener(ShowFileUI);
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
        Debug.Log("Show status UI");
    }

    private void ShowItemsUI()
    {
        Debug.Log("Show items UI");
    }

    private void ShowFileUI()
    {
        Debug.Log("Show file UI");
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
