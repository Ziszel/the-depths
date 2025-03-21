using System.Collections;
using UnityEngine;

public class RotatingDoor : DoorBase, ISwitchable, IInteractable
{
    // Rotate by this amount of degrees over movementDuration
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private bool isKeyPowered;
    [SerializeField] private InventoryItem keyItem;
    [SerializeField] private float rotationAmount = 90.0f;
    [SerializeField] private GameObject childDoorMeshObject;

    private Vector3 _initialRotation;
    private Inventory _playerInventory;
    private Vector3 _playerForwardVector;
    private Vector3 _forward;
    
    private void Start()
    {
        _initialRotation = transform.rotation.eulerAngles;
        DoorAudio = GetComponent<DoorAudio>();
        _playerInventory = FindFirstObjectByType<Inventory>();
        IsOpen = false;
        DoorAudio.SetDoorAudio(false);
        _forward = transform.forward;
        _playerForwardVector = FindAnyObjectByType<PlayerController>().GetComponent<Transform>().forward;
        DoorMesh = childDoorMeshObject;
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

    protected override IEnumerator OpenDoor(float dot)
    {
        Quaternion startingRotation = transform.rotation;
        Quaternion targetRotation;

        if (dot >= 0.0f)
        {
            targetRotation = Quaternion.Euler( new Vector3(0.0f, _initialRotation.y + rotationAmount, 0.0f));
        }
        else
        {
            targetRotation = Quaternion.Euler( new Vector3(0.0f, _initialRotation.y - rotationAmount, 0.0f));
        }
        
        
        IsOpen = true;

        float time = 0.0f;
        while (time < 1.0f)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, time);
            yield return null;
            time += Time.deltaTime * movementDuration;
        }
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
                    
                    // Calculate forward
                    float dot = Vector3.Dot(_forward, (_playerForwardVector - transform.position).normalized);
                    StartCoroutine(OpenDoor(dot));
                    DoorMesh.layer = LayerMask.NameToLayer("Default");
                }
            }
            DoorAudio.SetDoorAudio(false);
            DoorAudio.PlaySfx(); // Door is not meant to open
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
            float dot = Vector3.Dot(_forward, (_playerForwardVector - transform.position).normalized);
            StartCoroutine(OpenDoor(dot));
            IsOpen = true;
        }
    }
}
