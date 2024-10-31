using UnityEngine;
using Random = UnityEngine.Random;

public class LightController : MonoBehaviour, ISwitchable, IMonsterInteractable
{
    // Note: The actual light, it doesn't have to be flickering
    private Light _flickeringLight;
    private float _timer = 0;

    [Header("Flicker Settings (input -1 for random flickering)")]
    [SerializeField] private float _customFlickerInterval = 0.1f;
    [SerializeField] private bool _isFlickering = true;

    void Start()
    {
        _flickeringLight = GetComponentInChildren<Light>();
        Debug.Log(_flickeringLight);
    }

    void Update()
    {
        if (_isFlickering) 
        { 
            // Check if -1 was input for random flickering, if not, use custom flicker interval input
            if (_customFlickerInterval == -1) { HandleFlicker(Random.Range(0.05f, 2.0f));  }
            else { HandleFlicker(_customFlickerInterval); }
        }
    }
    private void HandleFlicker(float flickerInterval)
    {
        _timer += Time.deltaTime;
        if (_timer > flickerInterval)
        {
            ToggleLight();
            _timer = 0;
        }
    }

    public void StartFlicker()
    {
        _isFlickering = true;
    }

    public void StopFlicker()
    {
        _isFlickering = false;
    }
    public void TurnOnLight()
    {
        _flickeringLight.enabled = true;
    }

    public void TurnOffLight()
    {
        _flickeringLight.enabled = false;
    }

    public void ToggleLight()
    {
        _flickeringLight.enabled = !_flickeringLight.enabled;
    }

    public void Toggle()
    {
        if (_isFlickering)
        {
            _isFlickering = false;
            _flickeringLight.gameObject.SetActive(false);
        }
        else
        {
            _flickeringLight.gameObject.SetActive(true);
            _isFlickering = true;
        }
    }
    
    public void AffectedByMonster()
    {
        Debug.Log("disabling light");
        _isFlickering = false;
        _flickeringLight.enabled = false;
    }

    public void StopAffectedByMonster()
    {
        Debug.Log("enabling light");
        _flickeringLight.enabled = true;
    }
}
