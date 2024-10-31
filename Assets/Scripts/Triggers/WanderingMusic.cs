using UnityEngine;

public class WanderingMusic : MonoBehaviour
{
    private MusicManager _musicManager;
    [SerializeField] private bool _turnOn;

    private void Awake()
    {
        _musicManager = GameObject.Find("MusicAudioSource").GetComponentInChildren<MusicManager>();
    }

    // Acts like a toggle for turning the wandering music on/off
    private void OnTriggerEnter(Collider other)
    {
        // Turn off music
        if (!_turnOn)
        {
            _musicManager.TriggerFadeOutMusic();
        }
        // Turn on music
        else
        {
            // No need to assign if already assigned
            if (!_musicManager.IsAssignedWanderingMusic())
            {
                _musicManager.AssignWanderingMusic();
            }
            // No need to play if already playing
            if (!_musicManager.IsPlaying())
            {
                _musicManager.Play();
            }
            
        }
    }
}
