using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDUI : MonoBehaviour
{
    [SerializeField] private Sprite lightWounds;
    [SerializeField] private Sprite heavyWounds;
    [SerializeField] private Image activeInteractable;
    [SerializeField] private TMP_Text pickupText;
    [SerializeField] private string pickupTextInventory;
    [SerializeField] private string pickupTextFile;
    
    private Image _damageImage;
    private HealthManager _healthManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _damageImage = GetComponent<Image>();
    }

    public void SetImageFromHP(float health)
    {
        switch (health)
        {
            case 0: // Player is dead
                DisableImage();
                break;
            case 1:
                SetHeavyWounds();
                break;
            case 2:
                SetLightWounds();
                break;
            case 3: // Player has max health
                DisableImage();
                break;
            default:
                break;
        }
    }

    // Health UI
    private void DisableImage()
    {
        _damageImage.enabled = false;
    }

    private void SetLightWounds()
    {
        _damageImage.enabled = true;
        _damageImage.sprite = lightWounds;
    }

    private void SetHeavyWounds()
    {
        _damageImage.enabled = true;
        _damageImage.sprite = heavyWounds;
    }
    
    // Interactable UI
    public void SetActiveInteractable(Sprite sprite)
    {
        activeInteractable.enabled = true;
        activeInteractable.sprite = sprite;
    }
    
    public void DisableActiveInteractable()
    {
        activeInteractable.enabled = false;
    }
    
    private void ShowDescriptiveText(string text)
    {
        pickupText.text = text;
        pickupText.enabled = true;
        StopCoroutine("HidePickupText");
        StartCoroutine("HidePickupText");
    }
    
    // Pickup text
    private void ShowPickupText(PickupItem pickupItem)
    {
        if (pickupItem.GetType() == typeof(InventoryItem))
        {
            InventoryItem inventoryItem = (InventoryItem)pickupItem;
            pickupText.text = inventoryItem.itemName + pickupTextInventory;
        }

        if (pickupItem.GetType() == typeof(FileItem))
        {
            FileItem fileItem = (FileItem)pickupItem;
            pickupText.text = fileItem.displayName + pickupTextFile;
        }
        
        pickupText.enabled = true;
        StopCoroutine("HidePickupText");
        StartCoroutine("HidePickupText");
    }

    private IEnumerator HidePickupText()
    {
        yield return new WaitForSeconds(4.0f);
        pickupText.enabled = false;
    }

    private void OnEnable()
    {
        HealthManager.HealthChanged += SetImageFromHP;
        Pickup.OnPickupOccurred += ShowPickupText;
        DisplayDescriptiveText.OnTextUpdate += ShowDescriptiveText;
    }

    private void OnDisable()
    {
        HealthManager.HealthChanged -= SetImageFromHP;
        Pickup.OnPickupOccurred -= ShowPickupText;
        DisplayDescriptiveText.OnTextUpdate -= ShowDescriptiveText;
    }
}
