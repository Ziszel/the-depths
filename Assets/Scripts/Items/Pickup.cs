using System;
using TMPro;
using UnityEngine;

public class Pickup : MonoBehaviour, IInteractable, IResettable
{
    public static event Action<PickupItem> OnPickupOccurred;
    public PickupItem itemData;
    
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private AudioClip interactClip;

    private Inventory _playerInventory;
    private GlobalSFXPlayer _globalSfxPlayer;
    private Camera _camera;
    private TMP_Text _itemName;

    private void Start()
    {
        _camera = Camera.main;
        _playerInventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        _itemName = GetComponentInChildren<TMP_Text>(true);

        if (itemData.GetType() == typeof(InventoryItem))
        {
            InventoryItem inventoryItem = (InventoryItem)itemData;
            _itemName.text = inventoryItem.itemName;
        }
        else if (itemData.GetType() == typeof(FileItem))
        {
            FileItem fileItem = (FileItem)itemData;
            _itemName.text = fileItem.displayName;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            _itemName.enabled = true;
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Quaternion rotation = Quaternion.LookRotation(-(_camera.transform.position - transform.position).normalized, Vector3.up);
        _itemName.rectTransform.rotation = rotation;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            _itemName.enabled = false;
            playerInteractable.NoActivePickup();
        }
    }

    // IInteractable
    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }

    public void AttemptToInteract()
    {
        if (gameObject.TryGetComponent(out UpdatePlayerGoalText updatePlayerStatusText))
        {
            updatePlayerStatusText.UpdateStatusText();
        }
        
        if (itemData.GetType() == typeof(InventoryItem))
        {
            OnPickupOccurred?.Invoke(itemData);
            _playerInventory.AddItem((InventoryItem)itemData);
            _globalSfxPlayer.PlaySfx(interactClip);
        }
                
        if (itemData.GetType() == typeof(FileItem))
        {
            OnPickupOccurred?.Invoke(itemData);
            _playerInventory.AddFile((FileItem)itemData);
            _globalSfxPlayer.PlaySfx(interactClip);
            
            FileItem fileItem = (FileItem)itemData;
            FileData? fd = _playerInventory.GetFileDataByFileItem(fileItem);
            LevelManager.ShowFileReaderUIImmediately(fd);
        }
            
        // Clean-up
        gameObject.SetActive(false);
    }

    public void ResetObjectState()
    {
        FindAnyObjectByType<PlayerInteractable>().NoActivePickup();
        gameObject.SetActive(true);
    }
}
