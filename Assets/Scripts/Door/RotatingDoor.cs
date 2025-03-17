using System.Collections;
using UnityEngine;

public class RotatingDoor : DoorBase, ISwitchable, IInteractable
{
    // Rotate by this amount of degrees over movementDuration
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private bool isKeyPowered;
    [SerializeField] private InventoryItem keyItem;
    [SerializeField] private Vector3 rotateByDegrees = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private float rotationSpeed;

    private Quaternion _initialRotation;
    private Quaternion _endRotation;
    private Inventory _playerInventory;
    
    private void Start()
    {
        _initialRotation = transform.rotation;
        _endRotation = Quaternion.Inverse(_initialRotation) * Quaternion.Euler(rotateByDegrees);
        DoorAudio = GetComponent<DoorAudio>();
        _playerInventory = FindFirstObjectByType<Inventory>();
        
        DoorAudio.SetDoorAudio(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = false;
            playerInteractable.NoActivePickup();
        }
    }

    protected override IEnumerator OpenDoor()
    {
        while (Quaternion.Angle(_initialRotation, _endRotation) > 0.01f)
        {
            // returns a quaternion rotated towards endRotation by the step value
            // Updates initialRotation since the next frame will rotate from that point
            _initialRotation = Quaternion.RotateTowards(_initialRotation, _endRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = _initialRotation;
            yield return null;
        }

        transform.rotation = _endRotation;
    }
    
    // IInteractable
    public void AttemptToInteract()
    {
        if (!IsOpen)
        {
            if (isKeyPowered)
            {
                if (_playerInventory.ContainsItem(keyItem))
                {
                    DoorAudio.SetDoorAudio(true);
                    DoorAudio.PlaySfx();
                    StartCoroutine(OpenDoor());
                    IsOpen = true;
                }
            }
            DoorAudio.PlaySfx();
        }
    }
    
    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }
    
    // ISwitchable
    public void Toggle()
    {
        if (!IsOpen)
        {
            DoorAudio.SetDoorAudio(true);
            DoorAudio.PlaySfx();
            StartCoroutine(OpenDoor());
            IsOpen = true;
        }
    }
}
