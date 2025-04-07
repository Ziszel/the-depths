using UnityEngine;

/* The following class serves as the base for ALL descriptive interactable objects.
 Those scripts that need to do more than simply print text inherit this script.
 Note: It should always be put on an object with a DisplayDescriptiveText component.*/
public class DescriptiveInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] protected AudioClip audioClip;
    protected GlobalSFXPlayer GlobalSfxPlayer;
    [SerializeField] private Sprite activeSprite;
    protected DisplayDescriptiveText DisplayDescriptiveText;

    protected void Start()
    {
        DisplayDescriptiveText = GetComponent<DisplayDescriptiveText>();
        GlobalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
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
        if (other.TryGetComponent(out PlayerInteractable playerInteractable))
        {
            playerInteractable.enabled = false;
            playerInteractable.NoActivePickup();
        }
    }

    public void AttemptToInteract()
    {
        Debug.Log("DescriptiveInteractable AttemptToInteract()");
        if (audioClip)
        {
            GlobalSfxPlayer.PlaySfx(audioClip);
        }
        DisplayDescriptiveText.UpdateDescriptionText();
    }

    public Sprite GetInteractableSprite()
    {
        return activeSprite;
    }
}
