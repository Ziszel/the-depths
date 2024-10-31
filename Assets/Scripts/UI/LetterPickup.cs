using System;
using UnityEngine;

public class LetterPickup : MonoBehaviour, IInteractable
{

    public string _letterContents; // letter contents
    
    // Components
    private PlayerController _player;
    private LetterManager _letterManager;
    private LevelManager _levelManager;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
        _letterManager = FindAnyObjectByType<LetterManager>();
        _levelManager = FindAnyObjectByType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _levelManager.ActivateInteractUI();
            _player.SetCurrentInteractable(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _levelManager.DeActivateInteractUI();
            _player.SetCurrentInteractable(null);
        }
    }
    
    public void AttemptToInteract()
    {
        _letterManager.SetLetterContents(_letterContents);
        if (GameManager.instance.IsLetterActive())
        {
            GameManager.instance.DeactivateLetter();
        }
        else
        {
            GameManager.instance.ActivateLetter();
        }
        // Figure out a way to hide buttons (or hide one and edit text of other) if we're not in initial starting letter
    }
}
