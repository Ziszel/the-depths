using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] private PlayerHUDUI pcHud;
    private Pickup _pickupInRange;

    private void Update()
    {
        Debug.Log("I'm active");
        if (DetectItem())
        {
            pcHud.SetActiveInteractable(_pickupInRange.GetInteractableSprite());
        }
    }

    private bool DetectItem()
    {
        // Each frame see if the center of the camera is actually pointing at an item
        // Debug.DrawRay(transform.position, transform.forward, Color.red);
        return true;
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
