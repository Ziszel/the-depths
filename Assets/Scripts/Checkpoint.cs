using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform spawnLocation;
    private LevelManager _levelManager;
    private BoxCollider _collider;

    private void Start()
    {
        _levelManager = FindAnyObjectByType<LevelManager>();
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _levelManager.SetCheckpoint(spawnLocation);
            // TODO: Add UI to tell the player they have reached a checkpoint
            Debug.Log("checkpoint reached");
            _collider.enabled = false;
        }
    }
}
