using System;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] private PlayerHUDUI pcHud;
    private Pickup _pickupInRange;
    
    private LayerMask _layerMask;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("InteractableMesh");
    }

    private void FixedUpdate()
    {
        if (DetectItem())
        {
            pcHud.SetActiveInteractable(_pickupInRange.GetInteractableSprite());
        }
        else
        {
            pcHud.DisableActiveInteractable();
        }
        
    }

    private bool DetectItem()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, fwd, Color.red);
        if (Physics.Raycast(transform.position, fwd, 3, _layerMask))
        {
            return true;
        }

        return false;
    }
    
    public void SetActivePickup(Pickup pickup)
    {
        _pickupInRange = pickup;
    }

    public void NoActivePickup()
    {
        _pickupInRange = null;
        pcHud.DisableActiveInteractable();
    }
}
