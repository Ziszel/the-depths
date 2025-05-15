using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static Action OnInventoryClosed;
    public static Action OnInventoryOpened;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseOverlay;
    
    [Header("Spawn controls")]
    [SerializeField] private Transform playerSpawnPoint;
    
    public Transform checkpointTransform;
    private static float _timer;
    private bool _isPaused;

    private GameManager _gameManager;
    private PathNodeManager _pathNodeManager;
    private FPSCamera _fpsCamera;
    private PostProcessManager _postProcessManager;
    private static PlayerController _player;
    private Monster _monster;
    private SetMonsterStateTrigger[] _monsterStateTriggers;
    /* UI */
    private static InventoryManagerUI _inventoryUI;
    private static FileReader _fileReader;
    
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
        _postProcessManager = FindAnyObjectByType<PostProcessManager>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _player = FindAnyObjectByType<PlayerController>();
        _monster = FindAnyObjectByType<Monster>();
        _fpsCamera = GameObject.Find("FPSCamera").GetComponent<FPSCamera>();

        if (TryGetComponent(out PathNodeManager pathNodeManager))
        {
            _pathNodeManager = pathNodeManager;
        }
            
        _timer = 0;
        _isPaused = false;
        _playerDead = false;

        _monsterStateTriggers = FindObjectsByType<SetMonsterStateTrigger>(FindObjectsSortMode.None);
        _inventoryUI = GameObject.Find("InventoryUI").GetComponent<InventoryManagerUI>();
        _fileReader = GameObject.Find("FileReaderUI").GetComponent<FileReader>();
        
        // Set location of objects at run-time
        _player.transform.position = playerSpawnPoint.position;
        CameraManager.ForceCurrentCameraRotation(Quaternion.LookRotation(playerSpawnPoint.forward, Vector3.up));
        
        // Events
        _player.OnPlayerDeath += PrepareForRespawn;
        _player.OnPausePressed += HandlePause;
        OnInventoryClosed += ApplySettingsToGame; // Gets around making ApplySettingsToGame static
        
        // Apply settings values to objects in game
        ApplySettingsToGame();
        
        // Potential code to run after everything else is loaded.
        // Update the status text if applicable.
        if (gameObject.TryGetComponent(out UpdatePlayerGoalText updatePlayerGoalText))
        {
            updatePlayerGoalText.UpdateStatusText();
        }
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
        }
    }

    // This function MUST also open the inventory for the game to function as per design
    public static void ShowFileReaderUIImmediately(FileData? fileData)
    {
        _player.SetOldPlayerState(_player.GetPlayerActionState());
        _player.SetPlayerState(playerActionState.InInventory);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inventoryUI.ShowOnOpenFileReader(GetInventoryFromPlayer(), fileData);
        OnInventoryOpened?.Invoke();
    }
    
    public void ShowInventory()
    {
        _player.SetOldPlayerState(_player.GetPlayerActionState());
        _player.SetPlayerState(playerActionState.InInventory);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _inventoryUI.ShowOnOpen(GetInventoryFromPlayer());
        OnInventoryOpened?.Invoke();
    }

    public void HideInventory()
    {
        _player.SetPlayerState(_player.GetOldPlayerActionState());
        _fileReader.DisableFileReaderUI();
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
        checkpointTransform.position = newCheckpoint.position;
        checkpointTransform.rotation = _player.transform.rotation;
    }

    public void RespawnPlayer()
    {
        // Old game jam code. We now ALWAYS want to attempt to load the player
        // if (checkpointTransform.position == new Vector3(0.0f, 0.0f, 0.0f))
        // {
        //     _gameManager.LoadLevel("MainLevel");
        // }
        // else
        // {
        //     _player.transform.position = checkpointTransform.position;
        //     _player.transform.rotation = checkpointTransform.rotation;
        // }
        // Make sure of no unexpected behaviour
        _playerDead = false;
        _player.EnableInputActions();
        _player.GetCinemachineCamera().Lens.Dutch = 0.0f;
        _player.ResetFPSCameraPositionRelativeToPlayer();
        
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
        Time.timeScale = 0.0f;
        pauseOverlay.SetActive(true);
        _postProcessManager.SwitchVolume(_postProcessManager.pauseVolume);
    }

    private void ClosePauseMenu()
    {
        Time.timeScale = 1.0f;
        pauseOverlay.SetActive(false);
        _postProcessManager.SwitchVolume(_postProcessManager.gameplayVolume);
    }

    public static Inventory GetInventoryFromPlayer()
    {
        return _player.GetPlayerInventory();
    }

    public List<Vector3> UpdatePathNodes(int numberOfNodes)
    {
        return _pathNodeManager.GetClosestPathNodesToPlayer(numberOfNodes);
    }

    private void ApplySettingsToGame()
    {
        _fpsCamera.SetGain(PlayerPrefs.GetFloat("MouseSensitivity"));
        _gameManager.SetMusicMixerValue();
        _gameManager.SetSFXMixerValue();
    }

    private void OnDisable()
    {
        _player.OnPlayerDeath -= PrepareForRespawn;
        _player.OnPausePressed -= HandlePause;
        OnInventoryClosed -= ApplySettingsToGame;
    }
}
