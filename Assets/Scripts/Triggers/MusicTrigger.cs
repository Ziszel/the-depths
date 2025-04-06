using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip musicToPlay;
    
    private MusicManager _musicManager;

    private void Start()
    {
        _musicManager = FindFirstObjectByType<MusicManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerTrigger"))
        {
            _musicManager.PlayMusic(musicToPlay);
            Destroy(gameObject);
        }
    }
}
