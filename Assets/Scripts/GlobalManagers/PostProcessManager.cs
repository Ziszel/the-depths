using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessManager : MonoBehaviour
{
    public Volume[] volumes;

    public Volume gameplayVolume;
    public Volume pauseVolume;

    public Volume initialVolume;
    private Volume _currentVolume;

    private void Start()
    {
        _currentVolume = initialVolume;
        
        for (int i = 0; i < volumes.Length; i++)
        {
            if (volumes[i] == _currentVolume)
            {
                volumes[i].weight = 1.0f;
            }
            else
            {
                volumes[i].weight = 0.0f;
            }
        }
    }
    
    public void SwitchVolume(Volume newVolume)
    {
        _currentVolume.weight = 0.0f;
        _currentVolume = newVolume;
        _currentVolume.weight = 1.0f;
    }
}
