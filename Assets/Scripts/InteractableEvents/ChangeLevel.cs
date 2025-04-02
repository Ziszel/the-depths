using UnityEngine;

public class ChangeLevel : MonoBehaviour, IInteractable
{
    [SerializeField] private string levelToLoad;
    
    [SerializeField] private Sprite interactableSprite;
    [SerializeField] private AudioClip interactClip;
    
    private GlobalSFXPlayer _globalSfxPlayer;

    void Start()
    {
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
        if (other.TryGetComponent<PlayerInteractable>(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = true;
            playerInteractable.SetActivePickup(this.gameObject);
        }
    }
    
    // IInteractable
    public Sprite GetInteractableSprite()
    {
        return interactableSprite;
    }

    public void AttemptToInteract()
    {
        
    }
}
