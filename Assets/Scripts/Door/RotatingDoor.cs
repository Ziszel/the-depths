using System.Collections;
using UnityEngine;

public class RotatingDoor : DoorBase, ISwitchable, IInteractable
{
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private float rotationAmount = 90.0f;

    private Vector3 _initialRotation;
    private Inventory _playerInventory;
    private Vector3 _playerForwardVector;
    private Vector3 _forward;
    private bool _isPlayerOnLockedSide; // manages which collider is active
    
    private void Start()
    {
        // locked from other side
        if (lockedFromOtherSideDoor)
        {
            DoorCollisionHelper.OnColliderEntered += PlayerEnteredChildCollider;
            DoorCollisionHelper.OnColliderExited += PlayerExitedChildCollider;
        }
        else // Disable locked from other side related code
        {
            doorCollisionHelperLocked.SetActive(false);
            doorCollisionHelperOpen.SetActive(false);
        }
        
        _initialRotation = transform.rotation.eulerAngles;
        DoorAudio = GetComponent<DoorAudio>();
        _playerInventory = FindFirstObjectByType<Inventory>();
        DoorAudio.SetDoorAudio(false);
        _forward = transform.forward;
        _playerForwardVector = FindAnyObjectByType<PlayerController>().GetComponent<Transform>().forward;
        DoorMesh = childDoorMeshObject;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Use separate colliders if it's a locked from other side door!
        if (!lockedFromOtherSideDoor)
        {
            if (other.TryGetComponent(out PlayerInteractable playerInteractable))
            {
                playerInteractable.enabled = true;
                playerInteractable.SetActivePickup(gameObject);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Use separate colliders if it's a locked from other side door!
        if (!lockedFromOtherSideDoor)
        {
            if (other.TryGetComponent(out PlayerInteractable playerInteractable))
            {
                playerInteractable.enabled = false;
                playerInteractable.NoActivePickup();
            }
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
        
        isOpen = true;

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
        if (!isOpen)
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
                else
                {
                    DoorAudio.SetDoorAudio(false);
                    DoorAudio.PlaySfx(); // Door is not meant to open
                }
            }

            if (lockedFromOtherSideDoor)
            {
                if (_isPlayerOnLockedSide)
                {
                    DoorAudio.SetDoorAudio(false);
                    DoorAudio.PlaySfx();
                }
                else
                {
                    DoorMesh.layer = LayerMask.NameToLayer("Default");
                    doorCollisionHelperLocked.SetActive(false);
                    doorCollisionHelperOpen.SetActive(false);
                    DoorAudio.SetDoorAudio(true);
                    DoorAudio.PlaySfx();
                    float dot = Vector3.Dot(_forward, (_playerForwardVector - transform.position).normalized);
                    StartCoroutine(OpenDoor(dot));
                }
            }
        }
    }
    
    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }
    
    // ISwitchable
    public void Toggle()
    {
        if (!isOpen)
        {
            DoorAudio.SetDoorAudio(true);
            DoorAudio.PlaySfx();
            float dot = Vector3.Dot(_forward, (_playerForwardVector - transform.position).normalized);
            StartCoroutine(OpenDoor(dot));
            isOpen = true;
        }
    }
    
    private void PlayerEnteredChildCollider(bool isPlayerOnLockedSide, PlayerInteractable interactable)
    {
        Debug.Log("Player entered child collider " + isPlayerOnLockedSide);
        interactable.enabled = true;
        interactable.SetActivePickup(gameObject);
        _isPlayerOnLockedSide = isPlayerOnLockedSide;
    }

    private void PlayerExitedChildCollider(PlayerInteractable interactable)
    {
        Debug.Log("Player exited child collider");
        interactable.enabled = false;
        interactable.NoActivePickup();
        _isPlayerOnLockedSide = false;
    }
}
