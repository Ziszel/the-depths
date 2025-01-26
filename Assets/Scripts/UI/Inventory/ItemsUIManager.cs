using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _selectedItemImg;
    [SerializeField] private Image _previousItemImg;
    [SerializeField] private Image _nextItemImg;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    
    private Inventory _inventory;
    private int _inventoryLength;
    private int _inventoryIndex;

    private void Start()
    {
        _nextButton.onClick.AddListener(NextItem);
        _previousButton.onClick.AddListener(PreviousItem);
    }

    private void NextItem()
    {
        _inventoryIndex++;
        SetCurrentItem();
        SetNextItemImage();
        _previousButton.interactable = true;

        if (_inventoryIndex == _inventoryLength)
        {
            _nextButton.interactable = false;
        }
    }

    private void PreviousItem()
    {
        _inventoryIndex--;
        SetCurrentItem();
        SetPreviousItemImage();
        _nextButton.interactable = true;

        if (_inventoryIndex == 0)
        {
            _previousButton.interactable = false;
        }
    }

    private void SetCurrentItem()
    {
        _selectedItemImg.sprite = _inventory.GetItemByIndex(_inventoryIndex).itemImage;
        descriptionText.text = _inventory.GetItemByIndex(_inventoryIndex).itemDescription;
    }

    private void SetNextItemImage()
    {
        if (_inventoryIndex == _inventoryLength || _inventoryLength < 1)
        {
            // Don't set anything as we will run into errors
            return;
        }
        
        _nextItemImg.sprite = _inventory.GetItemByIndex(_inventoryIndex + 1).itemImage;
    }

    private void SetPreviousItemImage()
    {
        if (_inventoryIndex == 0)
        {
            // Don't set anything, first item does not have a previous item image to update
            return;
        }
        
        _previousItemImg.sprite = _inventory.GetItemByIndex(_inventoryIndex - 1).itemImage;
    }

    public void SetInventory(Inventory inventory)
    {
        Debug.Log("inventory set on ItemsUIManager");
        _inventory = inventory;
    }

    public void InitialiseInventory()
    {
        _previousButton.interactable = false;
        _nextButton.interactable = false;
        _inventoryLength = _inventory.GetItems().Count - 1;
        _inventoryIndex = 0;
        if (_inventoryLength > 0) // then set up the first item
        {
            SetCurrentItem();
        }

        if (_inventoryIndex > 1)
        {
            _nextButton.interactable = true;
            SetNextItemImage();
        }
    }
}
