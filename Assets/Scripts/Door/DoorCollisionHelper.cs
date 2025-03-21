using System;
using UnityEngine;

public class DoorCollisionHelper : MonoBehaviour
{
    [SerializeField] private bool isLockedSide;
    [SerializeField] private GameObject otherSide;
    public static Action<bool, PlayerInteractable> OnColliderEntered;
    public static Action<PlayerInteractable> OnColliderExited;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered DoorCollisionHelper");
        if (other.TryGetComponent(out PlayerInteractable playerInteractable))
        {
            Debug.Log("Player is a player interactable");
            OnColliderEntered?.Invoke(isLockedSide, playerInteractable);
            otherSide.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractable playerInteractable))
        {
            OnColliderExited?.Invoke(playerInteractable);
            otherSide.SetActive(true);
        }
    }
}
