using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private float _bestTime;
    private float _totalPlayTime;
    private int _deathCount;
    private int _saveCount;
    
    /* UI */
    private InventoryManagerUI _inventoryUI;

    /* Audio */
    private MusicManager _musicManager;

    /* Monster */
    Monster monster;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // When the game first starts up set all values to initial state, changes can be made
    // when loading save file from disk later
    private void Start()
    {
        InitialiseGame();
    }

    private void Update()
    {
        _totalPlayTime += Time.unscaledDeltaTime;
    }

    public void LoadLevel(string levelName) //music calls commented out are called befopre thjis
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cursor will be ALWAYS be shown and not locked at the start
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // We're in a game level or testing level
        if (scene.name != "MainMenu")
        {
            _musicManager = GameObject.Find("MusicAudioSource").GetComponentInChildren<MusicManager>();
            _inventoryUI = GameObject.Find("InventoryUI").GetComponent<InventoryManagerUI>();
            monster = GameObject.Find("Monster").GetComponentInChildren<Monster>(); // Get the monster stored so we're able to play chasing/wandering music
            if (monster != null)
            {
                monster.OnChaseStateEntered += HandleMonsterEnterChaseState;
                monster.OnChaseStateExited += HandleMonsterExitChaseState;
            }
        }
    }

    public void ShowInventory(Inventory inventory)
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inventoryUI.ShowOnOpen(inventory);
    }

    public void HideInventory()
    {
        _inventoryUI.CloseInventory();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UIManager.instance.ShowOptionsCanvas(true);
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIManager.instance.UnshowOptionsCanvas();
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

    public string GetCurrentPlayTimeAsString()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_totalPlayTime);
        return timeSpan.ToString("hh':'mm':'ss", new CultureInfo("en-GB"));
    }

    public float GetCurrentPlayTime()
    {
        return _totalPlayTime;
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

    private void InitialiseGame()
    {
        _totalPlayTime = 0;
        _deathCount = 0;
        _saveCount = 0;
        // TODO: Attempt to load from disk a best time, if it fails put the default value here
        _bestTime = 999999;
    }
    
}
