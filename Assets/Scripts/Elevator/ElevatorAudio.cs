using UnityEngine;

public class ElevatorAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource elevatorSource;
    
    [Header("Audio clips")]
    [SerializeField] private AudioClip _elevatorClip;
    
    private void Start()
    {
        elevatorSource.clip = _elevatorClip;
    }

    public void PlaySfx()
    {
        if (!elevatorSource.isPlaying)
        {
            elevatorSource.Play();
        }
    }

    public void StopSfx()
    {
        if (elevatorSource.isPlaying)
        {
            elevatorSource.Stop();
        }
    }
}
