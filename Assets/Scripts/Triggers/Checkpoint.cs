using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Vector3 spawnLocation;
    [SerializeField] private Vector3 respawnRotation;
    [SerializeField] private float stopMusicFadeTime;
    private LevelManager _levelManager;
    private BoxCollider _collider;
    private bool _stopMusic;
    private MusicManager _musicManager;

    private void Start()
    {
        _levelManager = FindAnyObjectByType<LevelManager>();
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            _levelManager.SetCheckpoint(spawnLocation, respawnRotation);
            // TODO: Add UI to tell the player they have reached a checkpoint
            // Debug.Log("checkpoint reached");
            _collider.enabled = false;
        }

        if (_stopMusic)
        {
            _musicManager.StopMusicWithDelay(stopMusicFadeTime);
        }
    }
}
