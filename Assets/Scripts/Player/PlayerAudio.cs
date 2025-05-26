using System.Collections;
using UnityEngine;

public enum ActiveFootsteps
{
    Cement = 0,
    Wood = 1,
    SolidSteel = 2,
    Glass = 3
}

public class PlayerAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("FootstepAudio")]
    [SerializeField] private AudioClip[] footstepCementClips;
    [SerializeField] private AudioClip[] footstepWoodClips;
    [SerializeField] private AudioClip[] footstepSolidSteelClips;
    [SerializeField] private AudioClip[] footstepGlassClips;
    
    [Header("SoundTrigger audio volumes in meters")]
    [SerializeField] private float cementTriggerVolume;
    [SerializeField] private float woodTriggerVolume;
    [SerializeField] private float glassTriggerVolume;
    [SerializeField] private float solidSteelTriggerVolume;
    
    [Header("Event Audio")]
    [SerializeField] private AudioClip death;
    
    private SoundTrigger _soundTrigger;
    private ActiveFootsteps _activeFootstepClips;

    private void Start()
    {
        _activeFootstepClips = ActiveFootsteps.Cement;
        sfxSource.clip = GetRandomFootstepClip();
        _soundTrigger = GetComponent<SoundTrigger>();
    }

    private IEnumerator SwapFeetWithDelay()
    {
        while (sfxSource.isPlaying)
        {
            yield return null;
        }
        
        sfxSource.clip = GetRandomFootstepClip();
    }

    public void SwapFeet()
    {
        StartCoroutine(SwapFeetWithDelay());
    }
    
    public void PlaySfx()
    {
        if (!sfxSource.isPlaying)
        {
            _soundTrigger.TriggerSound();
            sfxSource.Play();
        }
    }

    public void PlayDeathSound()
    {
        sfxSource.PlayOneShot(death);
    }

    public AudioClip GetRandomFootstepClip()
    {
        AudioClip[] clips;

        switch (_activeFootstepClips)
        {
            case ActiveFootsteps.Cement:
                clips = footstepCementClips;
                break;
            case ActiveFootsteps.Wood:
                clips = footstepWoodClips;
                break;
            case ActiveFootsteps.SolidSteel:
                clips = footstepSolidSteelClips;
                break;
            case ActiveFootsteps.Glass:
                clips = footstepGlassClips;
                break;
            default:
                clips = footstepCementClips;
                break;
        }
        
        return clips[Random.Range(0, clips.Length)];
    }

    public void SetFootstepsToCement(bool isCrouching, bool isSprinting)
    {
        SetLocalTriggerVolumeFromPlayerState(isCrouching, isSprinting, cementTriggerVolume);
        _activeFootstepClips = ActiveFootsteps.Cement;
    }

    public void SetFootstepsToWood(bool isCrouching, bool isSprinting)
    {
        SetLocalTriggerVolumeFromPlayerState(isCrouching, isSprinting, woodTriggerVolume);
        _activeFootstepClips = ActiveFootsteps.Wood;
    }

    public void SetFootstepsToSolidSteel(bool isCrouching, bool isSprinting)
    {
        SetLocalTriggerVolumeFromPlayerState(isCrouching, isSprinting, solidSteelTriggerVolume);
        _activeFootstepClips = ActiveFootsteps.SolidSteel;
    }

    public void SetFootstepsToGlass(bool isCrouching, bool isSprinting)
    {
        SetLocalTriggerVolumeFromPlayerState(isCrouching, isSprinting, glassTriggerVolume);
        _activeFootstepClips = ActiveFootsteps.Glass;
    }

    // Sets the trigger volume based on the material volume itself and whether or not the player is crouching / sprinting
    private void SetLocalTriggerVolumeFromPlayerState(bool isCrouching, bool isSprinting, float materialVolume)
    {
        if (isCrouching)
        {
            _soundTrigger.SetTriggerVolume(0.0f);
        }
        else if (isSprinting)
        {
            _soundTrigger.SetTriggerVolume(materialVolume * 2);
        }
        else
        {
            _soundTrigger.SetTriggerVolume(materialVolume);
        }
    }
}
