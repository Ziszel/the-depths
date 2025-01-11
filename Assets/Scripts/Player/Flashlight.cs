using System.Collections;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public PlayerController playerController;
    public Transform flashlightAnchorTransform;
    
    [Header("light Properties")]
    [SerializeField] public Light lightSource;
    [SerializeField] private float flashlightIntensity;
    [SerializeField] private float flashlightRange;
    [SerializeField] private float windupTime; // in seconds
    [SerializeField] private float rotationSpeed;
    
    // Components
    private FlashlightAudio _flashlightAudio;
    // private Vector3 _offset;
    private Camera _cameraToFollow;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Setup the light source
        lightSource = GetComponentInChildren<Light>();
        _flashlightAudio = GetComponentInChildren<FlashlightAudio>();
        lightSource.intensity = flashlightIntensity;
        lightSource.range = flashlightRange;
        
        // Camera setup
        _cameraToFollow = Camera.main;
        // Offsetting by player or camera does not work when rotating with mouse
        // anchor point does cause some shake but is the preferred option for now
        // _offset = transform.position - _cameraToFollow.transform.position;
        
        // Hook up external events
        playerController.OnFlashlightActivated += ActivateFlashlight;
        playerController.OnFlashlightDeActivated += DeactivateFlashlight;
    }

    private void LateUpdate()
    {
        // transform.position = _cameraToFollow.transform.position + _offset;
        transform.position = flashlightAnchorTransform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, _cameraToFollow.transform.rotation, 
            Time.deltaTime * rotationSpeed);
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
