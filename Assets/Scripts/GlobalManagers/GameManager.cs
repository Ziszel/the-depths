using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string gameVersion;
    
    public static GameManager Instance;
    
    public AudioMixer musicMixer;
    public AudioMixer SFXMixer;

    private float _bestTime;
    private int _deathCount;
    private int _saveCount;
    
    // Used to persist data between scenes. This may need to be cleaned-up later.
    private List<InventoryItem> _persistedPlayerItems;
    private List<FileData> _persistedFileData;
    
    /* STATE */
    // HACK: not a fan of this approach to stopping other elements activating during inventory, easy to miss something
    // lots of changes required, etc... Used to stop flashlight playing from PC (separate input action had no effect)
    private bool _inventoryOpen; 

    /* Audio */
    private MusicManager _musicManager;

    /* Monster */ // (this DEFINITELY should not be here. Look to clean-up when implementing Level2)
    Monster monster;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        gameVersion = "0.1.5"; // Major, Minor, Patch

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // When the game first starts up set all values to initial state, changes can be made
    // when loading save file from disk later
    private void Start()
    {
        InitialiseGame();
    }

    public void LoadLevel(string levelName)
    {
        // We have a valid Inventory that we need to save
        if (levelName.Contains("Level"))
        {
            Inventory playerInv = FindAnyObjectByType<PlayerController>().GetComponent<Inventory>();
            _persistedPlayerItems = playerInv.GetItems();
            _persistedFileData = playerInv.GetFileData();
        }
        SceneManager.LoadScene(levelName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // We're in a game level or testing level TODO: Remove. GameManager should not know about the Monster
        if (scene.name != "MainMenu")
        {
            //monster = GameObject.Find("Monster").GetComponentInChildren<Monster>(); // Get the monster stored so we're able to play chasing/wandering music
            if (monster != null)
            {
                monster.OnChaseStateEntered += HandleMonsterEnterChaseState;
                monster.OnChaseStateExited += HandleMonsterExitChaseState;
            }
        }
    }

    private void HandleMonsterEnterChaseState()
    {
        // Need an if here otherwise this will get assigned chase and play every frame from the monster delegate
        if (!_musicManager.IsPlaying())
        {
            _musicManager.AssignChaseMusic();
            _musicManager.Play();
        }
    }

    private void HandleMonsterExitChaseState()
    {
        _musicManager.TriggerFadeOutMusic(1.5f);
    }

    public int GetDeathCount()
    {
        return _deathCount;
    }

    public void IncrementDeathCount()
    {
        _deathCount++;
    }

    public int GetSaveCount()
    {
        return _saveCount;
    }

    public void IncrementSaveCount()
    {
        _saveCount++;
    }

    public bool IsInventoryOpen()
    {
        return _inventoryOpen;
    }

    public List<InventoryItem> GetSavedPlayerItems()
    {
        if (_persistedPlayerItems == null)
        {
            _persistedPlayerItems = new List<InventoryItem>();
        }
        return _persistedPlayerItems;
    }

    public List<FileData> GetSavedFileData()
    {
        if (_persistedFileData == null)
        {
            _persistedFileData = new List<FileData>();
        }
        return _persistedFileData;
    }
    
    public void SetBestTime(float newBestTime)
    {
        if (newBestTime < _bestTime)
        {
            _bestTime = newBestTime;
        }
    }

    public float GetBestTime()
    {
        return _bestTime;
    }

    public string GetVersionText()
    {
        return gameVersion;
    }
    
    // Settings and backend values (Level manager is responsible for some, game manager for others)
    public void SetMusicMixerValue()
    {
        musicMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 0.5f));
    }

    public void SetSFXMixerValue()
    {
        SFXMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume", 0.5f));
    }

    private void InitialiseGame()
    {
        _deathCount = 0;
        _saveCount = 0;
        // TODO: Attempt to load from disk a best time, if it fails put the default value here
        _bestTime = 999999;
        _inventoryOpen = false;
    }
    
}
