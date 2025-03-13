using UnityEngine;

public class Cupboard : MonoBehaviour, IInteractable
{

    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private Vector3 playerHideLocation;
    [SerializeField] private Vector3 playerExitLocation;
    [SerializeField] private AudioClip interactClip;
    
    private GlobalSFXPlayer _globalSfxPlayer;
    private Transform _playerTransform;
    private Transform _parentTransform;

    private void Start()
    {
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _parentTransform = GetComponentInParent<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(this.gameObject);
        }

        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.SetExitHidingLocation(playerExitLocation);
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
        _playerTransform.position = playerHideLocation;
        _playerTransform.rotation = Quaternion.LookRotation(_parentTransform.forward, Vector3.up);
        // Toggle input action map Hiding (no longer using ActionMaps for now so commented out)
        //InputManager.ToggleActionMap(InputManager.PlayerInputActions.Hiding);
        // Set player state to hiding
        if (_playerTransform.gameObject.TryGetComponent(out PlayerController pc))
        {
            pc.SetPlayerState(playerActionState.Hiding);
            pc.SetExitHidingLocation(playerExitLocation);
        }
        // Ensure the exit location is set correctly
        _globalSfxPlayer.PlaySfx(interactClip);
    }

    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }
}
