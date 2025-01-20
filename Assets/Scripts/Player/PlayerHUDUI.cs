using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDUI : MonoBehaviour
{
    [SerializeField] private Sprite lightWounds;
    [SerializeField] private Sprite heavyWounds;
    
    private Image _image;
    private HealthManager _healthManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _image = GetComponent<Image>();
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

    private void DisableImage()
    {
        _image.enabled = false;
    }

    private void SetLightWounds()
    {
        _image.enabled = true;
        _image.sprite = lightWounds;
    }

    private void SetHeavyWounds()
    {
        _image.enabled = true;
        _image.sprite = heavyWounds;
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
