using System.Collections;
using UnityEngine;

public class LockedOtherSideDoor : DoorBase, IInteractable
{
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private Vector3 rotateByDegrees = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private float rotationSpeed;
    
    private Quaternion _initialRotation;
    private Quaternion _endRotation;

    private bool _isPlayerOnLockedSide;

    private void Start()
    {
        _isPlayerOnLockedSide = true;
        _initialRotation = transform.rotation;
        _endRotation = Quaternion.Inverse(_initialRotation) * Quaternion.Euler(rotateByDegrees);
        DoorAudio = GetComponent<DoorAudio>();
        DoorCollisionHelper.OnColliderEntered += PlayerEnteredChildCollider;
        DoorCollisionHelper.OnColliderExited += PlayerExitedChildCollider;
        DoorMesh = childDoorMeshObject;
    }
    public void AttemptToInteract()
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
            StartCoroutine(OpenDoor(0.0f));
        }
    }

    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }
    
    protected override IEnumerator OpenDoor(float dot)
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

    private void PlayerEnteredChildCollider(bool isPlayerOnLockedSide, PlayerInteractable interactable)
    {
        Debug.Log("Player entered child collider");
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
