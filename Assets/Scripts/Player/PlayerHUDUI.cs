using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDUI : MonoBehaviour
{
    [SerializeField] private Sprite lightWounds;
    [SerializeField] private Sprite heavyWounds;
    [SerializeField] private Image activeInteractable;
    
    private Image _damageImage;
    private HealthManager _healthManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _damageImage = GetComponent<Image>();
    }

    public void SetImageFromHP(float health)
    {
        switch (health)
        {
            case 0: // Player is dead
                DisableImage();
                break;
            case 1:
                SetHeavyWounds();
                break;
            case 2:
                SetLightWounds();
                break;
            case 3: // Player has max health
                DisableImage();
                break;
            default:
                break;
        }
    }

    // Health UI
    private void DisableImage()
    {
        _damageImage.enabled = false;
    }

    private void SetLightWounds()
    {
        _damageImage.enabled = true;
        _damageImage.sprite = lightWounds;
    }

    private void SetHeavyWounds()
    {
        _damageImage.enabled = true;
        _damageImage.sprite = heavyWounds;
    }
    
    // Interactable UI
    public void SetActiveInteractable(Sprite sprite)
    {
        activeInteractable.enabled = true;
        activeInteractable.sprite = sprite;
    }
    
    public void DisableActiveInteractable()
    {
        activeInteractable.enabled = false;
    }

    private void OnEnable()
    {
        HealthManager.HealthChanged += SetImageFromHP;
    }

    private void OnDisable()
    {
        HealthManager.HealthChanged -= SetImageFromHP;
    }
}
