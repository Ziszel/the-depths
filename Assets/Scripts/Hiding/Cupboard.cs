using UnityEngine;

public class Cupboard : MonoBehaviour, IInteractable
{

    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private Transform playerHideLocation;
    [SerializeField] private Transform playerExitLocation;
    [SerializeField] private AudioClip interactClip;
    [SerializeField] private Transform hidingAngle;
    [SerializeField] private BoxCollider doorOne;
    [SerializeField] private BoxCollider doorTwo;
    
    private GlobalSFXPlayer _globalSfxPlayer;
    private Transform _playerTransform;

    private void Start()
    {
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerController.OnLeavingCupboard += ReenableCupboard;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = false;
            playerInteractable.NoActivePickup();
        }
    }

    public void AttemptToInteract()
    {
        // Teleport the player into the hiding location area
        _playerTransform.position = playerHideLocation.position;
        CameraManager.ForceCurrentCameraRotation(Quaternion.LookRotation(playerHideLocation.forward, Vector3.up));
        // Toggle input action map Hiding (no longer using ActionMaps for now so commented out)
        // InputManager.ToggleActionMap(InputManager.PlayerInputActions.Hiding);
        // Set player state to hiding
        if (_playerTransform.gameObject.TryGetComponent(out PlayerController pc))
        {
            pc.SetPlayerState(playerActionState.Hiding);
            pc.SetPlayerExitPosition(playerExitLocation);
        }
        // Ensure the exit location is set correctly
        _globalSfxPlayer.PlaySfx(interactClip);
        enabled = false;
        doorOne.enabled = false;
        doorTwo.enabled = false;
    }

    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }

    private void ReenableCupboard()
    {
        enabled = true;
        doorOne.enabled = true;
        doorTwo.enabled = true;
    }
}
