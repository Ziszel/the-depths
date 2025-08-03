using UnityEngine;

public class ChangeLevel : MonoBehaviour, IInteractable
{
    [SerializeField] private string levelToLoad;
    
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private AudioClip interactClip;
    
    private GlobalSFXPlayer _globalSfxPlayer;
    private PlayerInteractable _storedPlayerInteractable;
    private BlackFadeTransition _blackFadeTransition;

    void Start()
    {
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
        _blackFadeTransition = FindFirstObjectByType<BlackFadeTransition>();
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
        InputManager.PlayerInputActions.Disable();
        StartCoroutine(_blackFadeTransition.FadeToBlack(2.0f));
    }
    
    private void TriggerLevelLoad()
    {
        GameManager.Instance.LoadLevel(levelToLoad);
        enabled = false; // disable script not object
    }

    private void OnEnable()
    {
        BlackFadeTransition.OnFadeInEventComplete += TriggerLevelLoad;
    }

    private void OnDisable()
    {
        BlackFadeTransition.OnFadeInEventComplete -= TriggerLevelLoad;
    }
}
