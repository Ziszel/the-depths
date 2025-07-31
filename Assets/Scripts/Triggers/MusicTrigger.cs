using UnityEngine;

public class MusicTrigger : MonoBehaviour, IResettable
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
            _musicManager.PlayMusic(musicToPlay, 3.0f);
            gameObject.SetActive(false);
        }
    }

    public void ResetObjectState()
    {
        _musicManager.StopMusic();
        gameObject.SetActive(true);
    }
}
