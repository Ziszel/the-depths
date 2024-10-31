using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform checkpointTransform;
    public InteractHelpUI _InteractHelpUI;
    private static float _timer;

    private GameManager _gameManager;
    private PlayerController _player;
    private Monster _monster;
    private SetMonsterStateTrigger[] _monsterStateTriggers;

    // Player death handling
    [SerializeField] private float respawnTime = 3.0f;
    private float _timeUntilRespawn;
    private bool _playerDead;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _timer = 0;
        // _InteractHelpUI = FindAnyObjectByType<InteractHelpUI>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _player = FindAnyObjectByType<PlayerController>();
        _monster = FindAnyObjectByType<Monster>();
        _playerDead = false;

        _monsterStateTriggers = FindObjectsByType<SetMonsterStateTrigger>(FindObjectsSortMode.None);
        
        // Events
        _player.OnPlayerDeath += PrepareForRespawn;
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
        else
        {
            _timer += Time.deltaTime;
        }
    }

    public static float GetTimer()
    {
        return _timer;
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
        _timeUntilRespawn = respawnTime;
        _playerDead = true;
    }
    
    // Global UI level utilities
    public void ActivateInteractUI()
    {
        _InteractHelpUI.SetWhetherChildrenActive(true);
    }
    
    public void DeActivateInteractUI()
    {
        _InteractHelpUI.SetWhetherChildrenActive(false);
    }
}
