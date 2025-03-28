using System;
using System.Globalization;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static Action OnInventoryClosed;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseOverlay;
    
    public Transform checkpointTransform;
    private static float _timer;
    private bool _isPaused;

    private GameManager _gameManager;
    private FPSCamera _fpsCamera;
    private PostProcessManager _postProcessManager;
    private PlayerController _player;
    private Monster _monster;
    private SetMonsterStateTrigger[] _monsterStateTriggers;
    /* UI */
    private static InventoryManagerUI _inventoryUI;
    
    // Player death handling
    [SerializeField] private float respawnTime = 3.0f;
    private float _timeUntilRespawn;
    private bool _playerDead;

    private void Awake()
    {
        // When LevelManager loads in a scene, de-activate the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Start()
    {
        _timer = 0;
        _isPaused = false;
        _postProcessManager = FindAnyObjectByType<PostProcessManager>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _player = FindAnyObjectByType<PlayerController>();
        _monster = FindAnyObjectByType<Monster>();
        _fpsCamera = GameObject.Find("FPSCamera").GetComponent<FPSCamera>();
        _playerDead = false;

        _monsterStateTriggers = FindObjectsByType<SetMonsterStateTrigger>(FindObjectsSortMode.None);
        _inventoryUI = GameObject.Find("InventoryUI").GetComponent<InventoryManagerUI>();
        
        // Events
        _player.OnPlayerDeath += PrepareForRespawn;
        _player.OnPausePressed += HandlePause;
        OnInventoryClosed += ApplySettingsToGame; // Gets around making ApplySettingsToGame static
        
        // Apply settings values to objects in game
        ApplySettingsToGame();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_playerDead)
        {
            if (_timeUntilRespawn > 0)
            {
                // TODO: We could add a fade to black animation here if we get time
                _timeUntilRespawn -= Time.deltaTime;
            }
            else
            {
                RespawnPlayer();
            }
        }
        else if (!_isPaused)
        {
            _timer += Time.unscaledDeltaTime;
            Debug.Log(_timer);
        }
    }
    
    public static void ShowInventory(Inventory inventory)
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inventoryUI.ShowOnOpen(inventory);
    }

    public static void HideInventory()
    {
        _inventoryUI.CloseInventory();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        OnInventoryClosed?.Invoke();
    }

    public static float GetTimer()
    {
        return _timer;
    }

    public static string GetTimerAsString()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_timer);
        return timeSpan.ToString("hh':'mm':'ss", new CultureInfo("en-GB"));
    }

    public bool IsPlayerDead()
    {
        return _playerDead;
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        // If this is being called twice, disable the eyes on the player. They are tagged player too
        checkpointTransform.position = newCheckpoint.position;
        checkpointTransform.rotation = _player.transform.rotation;
    }

    public void RespawnPlayer()
    {
        if (checkpointTransform.position == new Vector3(0.0f, 0.0f, 0.0f))
        {
            _gameManager.LoadLevel("MainLevel");
        }
        else
        {
            _player.transform.position = checkpointTransform.position;
            _player.transform.rotation = checkpointTransform.rotation;
        }
        // Make sure of no unexpected behaviour
        _playerDead = false;
        _player.EnableInputActions();
        _player.GetCinemachineCamera().Lens.Dutch = 0.0f;
        _player.GetCinemachineCamera().Target.TrackingTarget = _player.GetCrouchTransform().transform;
        
        // Reset health
        if (_player.TryGetComponent(out HealthManager healthManager))
        {
            healthManager.SetHealth(healthManager.GetMaxHealth());
            healthManager.ResetCooldownTimer();
        }
        
        ResetMonster();

        // Re-enable each of the monster state triggers so that the same setup can occur
        foreach (var ms in _monsterStateTriggers)
        {
            ms.SetColliderOn();
        }
    }

    public void ResetMonster()
    {
        _monster.SetMonsterState(Monster.MonsterState.None);
    }

    private void PrepareForRespawn()
    {
        GameManager.Instance.IncrementDeathCount();
        _timeUntilRespawn = respawnTime;
        _playerDead = true;
    }

    private void HandlePause(bool isOpening)
    {
        if (isOpening)
        {
            _isPaused = true;
            OpenPauseMenu();
        }
        else
        {
            _isPaused = false;
            ClosePauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        Debug.Log("Game is paused");
        Time.timeScale = 0.0f;
        pauseOverlay.SetActive(true);
        _postProcessManager.SwitchVolume(_postProcessManager.pauseVolume);
    }

    private void ClosePauseMenu()
    {
        Time.timeScale = 1.0f;
        pauseOverlay.SetActive(false);
        _postProcessManager.SwitchVolume(_postProcessManager.gameplayVolume);
        PlayerPrefs.Save();
        ApplySettingsToGame();
    }

    private void ApplySettingsToGame()
    {
        _fpsCamera.SetGain(PlayerPrefs.GetFloat("MouseSensitivity"));
        _gameManager.SetMusicMixerValue();
        _gameManager.SetSFXMixerValue();
    }
}
