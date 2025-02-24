using System;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] private PlayerHUDUI pcHud;
    [SerializeField] private float pickupDistance;
    private GameObject _interactableInRange;
    
    private LayerMask _layerMask;
    private Camera _camera;
    private bool _canBeInteracted;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("InteractableMesh");
        _camera = Camera.main;
        _canBeInteracted = false;
    }

    private void FixedUpdate()
    {
        Debug.Log("interactable active");
        if (DetectItem())
        {
            if (_interactableInRange.TryGetComponent(out IInteractable interactable))
            {
                _canBeInteracted = true;
                pcHud.SetActiveInteractable(interactable.GetInteractableSprite());  
            }
        }
        else
        {
            _canBeInteracted = false;
            pcHud.DisableActiveInteractable();
        }
    }

    private bool DetectItem()
    {
        Vector3 fwd = _camera.transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(_camera.transform.position, fwd, pickupDistance, _layerMask))
        {
            return true;
        }
        return false;
    }
    
    public void SetActivePickup(GameObject interactable)
    {
        _interactableInRange = interactable;
    }

    public void NoActivePickup()
    {
        _interactableInRange = null;
        pcHud.DisableActiveInteractable();
        enabled = false;
    }

    private void NoActivePickup(PickupItem pickupItem)
    {
        _interactableInRange = null;
        pcHud.DisableActiveInteractable();
        enabled = false;
    }

    public void UseInteractable()
    {
        if (_canBeInteracted)
        {
            if (_interactableInRange.TryGetComponent(out IInteractable interactable))
            {
                interactable.AttemptToInteract();
            }
        }
    }

    private void OnEnable()
    {
        Pickup.OnPickupOccurred += NoActivePickup;
    }
}
