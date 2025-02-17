using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image selectedItemImg;
    [SerializeField] private Image previousItemImg;
    [SerializeField] private Image nextItemImg;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    
    private Inventory _inventory;
    private int _inventoryLength;
    private int _inventoryIndex;

    private void Start()
    {
        nextButton.onClick.AddListener(NextItem);
        previousButton.onClick.AddListener(PreviousItem);
    }

    private void NextItem()
    {
        _inventoryIndex++;
        UpdateItemUIElements();
        previousButton.interactable = true;

        if (_inventoryIndex == _inventoryLength - 1)
        {
            nextButton.interactable = false;
        }
    }

    private void PreviousItem()
    {
        _inventoryIndex--;
        UpdateItemUIElements();
        nextButton.interactable = true;

        if (_inventoryIndex == 0)
        {
            previousButton.interactable = false;
        }
    }

    private void UpdateItemUIElements()
    {
        selectedItemImg.sprite = _inventory.GetItemByIndex(_inventoryIndex).itemImage;
        descriptionText.text = _inventory.GetItemByIndex(_inventoryIndex).itemDescription;
        itemNameText.text = _inventory.GetItemByIndex(_inventoryIndex).itemName;
        
        SetNextItemImage();
        SetPreviousItemImage();
    }

    private void SetNextItemImage()
    {
        if (_inventoryIndex == _inventoryLength - 1 || _inventoryLength < 2)
        {
            nextItemImg.sprite = null;
            return;
        }
        
        nextItemImg.sprite = _inventory.GetItemByIndex(_inventoryIndex + 1).itemImage;
    }

    private void SetPreviousItemImage()
    {
        if (_inventoryIndex == 0)
        {
            previousItemImg.sprite = null;
            return;
        }
        
        previousItemImg.sprite = _inventory.GetItemByIndex(_inventoryIndex - 1).itemImage;
    }

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }

    public void InitialiseInventory()
    {
        previousButton.interactable = false;
        previousItemImg.sprite = null;
        nextItemImg.sprite = null;
        nextButton.interactable = false;
        _inventoryLength = _inventory.GetItems().Count;
        _inventoryIndex = 0;
        
        if (_inventoryLength > 0) // If at least one item exists populate fields
        {
            UpdateItemUIElements();
        }

        if (_inventoryLength > 1) // We have more than one item (so we need to be able to access other items)
        {
            nextButton.interactable = true;
        }
    }
}
