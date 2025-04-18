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
    
    [Header("Event Audio")]
    [SerializeField] private AudioClip death;
    
    private ActiveFootsteps _activeFootstepClips;

    private void Start()
    {
        _activeFootstepClips = ActiveFootsteps.Cement;
        sfxSource.clip = GetRandomFootstepClip();
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

    public void SetFootstepsToCement()
    {
        _activeFootstepClips = ActiveFootsteps.Cement;
    }

    public void SetFootstepsToWood()
    {
        _activeFootstepClips = ActiveFootsteps.Wood;
    }

    public void SetFootstepsToSolidSteel()
    {
        _activeFootstepClips = ActiveFootsteps.SolidSteel;
    }

    public void SetFootstepsToGlass()
    {
        _activeFootstepClips = ActiveFootsteps.Glass;
    }
}
