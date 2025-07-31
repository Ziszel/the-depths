using UnityEngine;

public class ScareSFX : MonoBehaviour, IScareEvent
{
    [SerializeField] private AudioClip clip;
    
    private GlobalSFXPlayer _globalSfxPlayer;

    private void Start()
    {
        _globalSfxPlayer = FindFirstObjectByType<GlobalSFXPlayer>();
    }
    
    public void TriggerScareEvent()
    {
        _globalSfxPlayer.PlaySfx(clip);
    }
}
