using System.Collections;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("light Properties")]
    [SerializeField] public Light lightSource;
    [SerializeField] private float flashlightIntensity;
    [SerializeField] private float flashlightRange;
    [SerializeField] private float windupTime; // in seconds
    
    // Components
    private FlashlightAudio _flashlightAudio;
    public PlayerController _playerController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        lightSource = GetComponentInChildren<Light>();
        _flashlightAudio = GetComponentInChildren<FlashlightAudio>();
        lightSource.intensity = flashlightIntensity;
        lightSource.range = flashlightRange;
        
        // Hook up external events
        _playerController.OnFlashlightActivated += ActivateFlashlight;
        _playerController.OnFlashlightDeActivated += DeactivateFlashlight;
    }

    private IEnumerator LightActivationRoutine()
    {
        yield return new WaitForSeconds(windupTime);
        lightSource.enabled = true;
    }

    private void ActivateFlashlight()
    {
        _flashlightAudio.PlaySfx();
        StartCoroutine("LightActivationRoutine");
    }

    private void DeactivateFlashlight()
    {
        StopCoroutine("LightActivationRoutine");
        lightSource.enabled = false;
        _flashlightAudio.StopSfx();
    }
}
