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
    [SerializeField] private GameObject statusUIObj;
    [SerializeField] private GameObject itemsUIObj;
    [SerializeField] private GameObject filesUIObj;
    [SerializeField] private GameObject optionsUIObj;
    [SerializeField] private Image backgroundImage;
    private Button _statusButton;
    private Button _itemsButton;
    private Button _fileButton;
    private Button _optionsButton;
    
    // Section managers
    private StatusUIManager _statusUIManager;
    private ItemsUIManager _itemsUIManager;
    private FilesUIManager _filesUIManager;

    private void Start()
    {
        AssociateNavigationButtons();
        
        _statusUIManager = GetComponentInChildren<StatusUIManager>(true);
        _itemsUIManager = GetComponentInChildren<ItemsUIManager>(true);
        _filesUIManager = GetComponentInChildren<FilesUIManager>(true);
    }

    private void AssociateNavigationButtons()
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

            if (b.name == "OptionsBtn")
            {
                _optionsButton = b;
            }
        }
        
        _statusButton.onClick.AddListener(ShowStatusUI);
        _itemsButton.onClick.AddListener(ShowItemsUI);
        _fileButton.onClick.AddListener(ShowFileUI);
        _optionsButton.onClick.AddListener(ShowOptionsUI);
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
        backgroundImage.gameObject.SetActive(true);
        ShowStatusUI();
    }

    private void ShowStatusUI()
    {
        _statusUIManager.InitialiseStatusScreen();
        SetActiveUIElements(true, false, false, false);
    }

    private void ShowItemsUI()
    {
        _itemsUIManager.InitialiseInventory();
        SetActiveUIElements(false, true, false, false);
    }

    private void ShowFileUI()
    {
        _filesUIManager.InitialiseFiles();
        SetActiveUIElements(false, false, true, false);
    }

    private void ShowOptionsUI()
    {
        SetActiveUIElements(false, false, false, true);
    }

    private void SetActiveUIElements(bool statusActivated, bool itemsActivated, bool fileActivated, 
        bool optionsActivated)
    {
        statusUIObj.SetActive(statusActivated);
        itemsUIObj.SetActive(itemsActivated);
        filesUIObj.SetActive(fileActivated);
        optionsUIObj.SetActive(optionsActivated);
    }

    public void CloseInventory()
    {
        // hide all UI elements (close all because we can't know which one the player can see)
        // Global
        horizontalMenu.SetActive(false);
        SetActiveUIElements(false, false, false, false);
        backgroundImage.gameObject.SetActive(false);
    }

    // Update local variables
    private void SetupInventory(Inventory updatedInventory)
    {
        _itemsUIManager.SetInventory(updatedInventory);
        _filesUIManager.SetInventory(updatedInventory);
    }
}
