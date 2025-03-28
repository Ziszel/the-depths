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
    
    [Header("lerp properties")]
    [SerializeField] private float lerpDuration; // in seconds
    private Vector3 _defaultPosition;
    private Vector3 _crouchedPosition;
    
    // Helper constants
    [SerializeField] private float crouchFlashlightAnchorOffset;
    
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
        
        _defaultPosition = flashlightAnchorTransform.localPosition;
        _crouchedPosition = _defaultPosition;
        _crouchedPosition.y = _defaultPosition.y + crouchFlashlightAnchorOffset;
        
        // Hook up external events
        playerController.OnFlashlightActivated += ActivateFlashlight;
        playerController.OnFlashlightDeActivated += DeactivateFlashlight;
        playerController.OnCrouchEnabled += MoveFlashlightToCrouchPosition;
        playerController.OnCrouchDisabled += MoveFlashlightToStandPosition;

        LevelManager.OnInventoryClosed += DeactivateFlashlight;
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
    
    private IEnumerator LightDeActivationRoutine()
    {
        yield return new WaitForSeconds(windupTime);
        lightSource.enabled = false;
    }

    private void ActivateFlashlight()
    {
        _flashlightAudio.PlaySfx();
        StopCoroutine("LightDeActivationRoutine");
        StartCoroutine("LightActivationRoutine");
    }

    private void DeactivateFlashlight()
    {
        StopCoroutine("LightActivationRoutine");
        StartCoroutine("LightDeActivationRoutine");
        _flashlightAudio.StopSfx();
    }

    private void MoveFlashlightToCrouchPosition()
    {
        StopCoroutine("LerpFlashlightToPosition");
        StartCoroutine("LerpFlashlightToPosition", true);
    }

    private void MoveFlashlightToStandPosition()
    {
        StopCoroutine("LerpFlashlightToPosition");
        StartCoroutine(LerpFlashlightToPosition(false));
    }

    IEnumerator LerpFlashlightToPosition(bool isCrouched)
    {
        float elapsedTime = 0.0f;
        Vector3 endPosition;
        if (isCrouched)
        {
            endPosition = _crouchedPosition;
        }
        else
        {
            endPosition = _defaultPosition;
        }
        
        while (elapsedTime < lerpDuration)
        {
            flashlightAnchorTransform.localPosition = Vector3.Lerp(flashlightAnchorTransform.localPosition, endPosition, 
                elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (isCrouched)
        {
            flashlightAnchorTransform.localPosition = _crouchedPosition;
        }
        else
        {
            flashlightAnchorTransform.localPosition = _defaultPosition;
        }
        
    }
}
