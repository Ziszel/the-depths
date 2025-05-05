using UnityEngine;

public class GlobalSFXPlayer : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioClip _clip;
    
    [Header("UI Audio")]
    [SerializeField] public AudioClip menuForward;
    [SerializeField] public AudioClip menuBackward;
    [SerializeField] public AudioClip horizontalMenu;
    [SerializeField] public AudioClip inventoryOpen;
    [SerializeField] public AudioClip fileReaderPageChange;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void PlaySfx(AudioClip audioClip)
    {
        // Debug.Log("playing audio sfx clip");
        _clip = audioClip;
        _audioSource.clip = _clip;
        _audioSource.Play();
    }
}
