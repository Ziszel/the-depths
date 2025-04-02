using UnityEngine;

public class ChangeLevel : MonoBehaviour, IInteractable
{
    [SerializeField] private string levelToLoad;
    
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private AudioClip interactClip;
    
    private GlobalSFXPlayer _globalSfxPlayer;
    private PlayerInteractable _storedPlayerInteractable;

    void Start()
    {
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            _storedPlayerInteractable = playerInteractable;
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(this.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            _storedPlayerInteractable = null;
            playerInteractable.NoActivePickup();
        }
    }
    
    // IInteractable
    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }

    public void AttemptToInteract()
    {
        _globalSfxPlayer.PlaySfx(interactClip);
        _storedPlayerInteractable.NoActivePickup();
        enabled = false; // disable script not object
        InputManager.ToggleActionMap(InputManager.PlayerInputActions.Hiding);
        GameManager.Instance.LoadLevel(levelToLoad);
    }
}
