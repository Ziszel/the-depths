using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public Action OnPlayerWithinKillDistance;
    
    public enum MonsterState
    {
        None, // no state, do nothing
        ChasePath, // Follow a path but allow for chasing the player
        Chase,
        SafePath, // Follow a path but do NOT allow for chasing the player (story elements)
        Investigate // Move to a specific location, then chase OR decide new path (ChasePath state)
    }

    [Header("How far the monster can see")]
    [SerializeField] private float maxViewDistance;
    
    private bool isChasing = false;

    // Delegates
    public delegate void ChaseStateEnterHandler();
    public event ChaseStateEnterHandler OnChaseStateEntered;
    public delegate void ChaseStateExitHandler();
    public event ChaseStateExitHandler OnChaseStateExited;

    // Components
    private PlayerController _player;
    private NavMeshAgent _agent;
    private MonsterState _monsterState;
    private LevelManager _levelManager;
    private MonsterAudio _monsterAudio;
    private MonsterAnimation _monsterAnimation;
    
    // path node logic
    private int _currentNodeIndicator;
    private int _previousNodeIndicator;
    private List<Vector3> _pathNodes;
    private List<Vector3> _previousPathNodes;
    private readonly float _minimumDistanceToNode = 2.0f;
    
    // Chase helpers
    [SerializeField] private float minimumChaseTime = 2.0f;
    private float _currentChaseTime;

    [Header("Control values")] 
    // Speed of the monster when walking a path
    [SerializeField] private float _pathSpeed = 2.0f;
    // Speed of the monster when chasing
    [SerializeField] private float _chaseSpeed = 4.0f;
    // Proximity to the player in units until they kill them
    [SerializeField] private float _killRange = 1.5f;
    // monster audio timers including how often it screeches based on those timers
    [SerializeField] private float minMonsterScreechRate = 7.5f; // seconds
    [SerializeField] private float maxMonsterScreechRate = 12.0f; // seconds
    private float _timeUntilScreech;
    
    private void Start()
    {
        _monsterState = MonsterState.None;
        _levelManager = FindAnyObjectByType<LevelManager>();
        _agent = GetComponent<NavMeshAgent>();
        _player = FindAnyObjectByType<PlayerController>();
        _monsterAudio = GetComponentInChildren<MonsterAudio>();
        _currentChaseTime = 0.0f;

        // subscribe to events
        _monsterAnimation = GetComponent<MonsterAnimation>();
        _monsterAnimation.SetStateToWalk();
    }
    
    private void Update()
    {
        // Regardless of state, if the player walks up to the monster, they die
        if (!_levelManager.IsPlayerDead())
        {
            if (Vector3.Distance(transform.position, _player.transform.position) < _killRange)
            {
                OnPlayerWithinKillDistance?.Invoke();
            }
        }

        switch (_monsterState)
        {
            case MonsterState.None:
                if (isChasing)
                {
                    OnChaseStateExited?.Invoke();
                }
                isChasing = false;
                _agent.speed = _pathSpeed;
                break;
            case MonsterState.ChasePath:
                _agent.speed = _pathSpeed;
                if (CanMonsterSeePlayer())
                {
                    _agent.destination = _player.transform.position;
                    SetMonsterState(MonsterState.Chase, _pathNodes, transform.position);
                }
                else
                {
                    if (Vector3.Distance(transform.position, _agent.destination) < _minimumDistanceToNode)
                    {
                        RandomlySetNextNode();
                    }
                }
                _timeUntilScreech -= Time.deltaTime;
                break;
            case MonsterState.Chase:
                _agent.speed = _chaseSpeed;
                if (!isChasing)
                {
                    OnChaseStateEntered?.Invoke();
                }
                isChasing = true;

                if (_currentChaseTime > minimumChaseTime)
                {
                    if (CanMonsterSeePlayer())
                    {
                        _currentChaseTime = minimumChaseTime;
                    }
                    else
                    {
                        // We can safely set the same _pathNodes as before
                        SetMonsterState(MonsterState.ChasePath, _pathNodes, transform.position);
                    }
                }
                _agent.destination = _player.transform.position;
                _currentChaseTime += Time.deltaTime;
                _timeUntilScreech -= Time.deltaTime;
                break;
            case MonsterState.SafePath:
                _agent.speed = _pathSpeed;
                if (isChasing)
                {
                    OnChaseStateExited?.Invoke();
                }
                isChasing = false;

                // Traverse a path
                if (Vector3.Distance(transform.position, _pathNodes[_currentNodeIndicator]) 
                    < _minimumDistanceToNode)
                {
                    // If we have reached the end of the safe path, teleport the monster out of the map
                    if ((_currentNodeIndicator + 1) > _pathNodes.Count - 1)
                    {
                        SetMonsterState(MonsterState.None);
                    }
                    else
                    {
                        _currentNodeIndicator++;
                        _agent.destination = _pathNodes[_currentNodeIndicator];
                    }
                }
                _timeUntilScreech -= Time.deltaTime;
                break;
            case MonsterState.Investigate:
                _agent.speed = _pathSpeed;
                
                if (CanMonsterSeePlayer())
                {
                    _agent.destination = _player.transform.position;
                    SetMonsterState(MonsterState.Chase, _pathNodes, transform.position);
                }

                // Reached destination without seeing player, reset to hunting
                if (Vector3.Distance(transform.position, _agent.destination) < _minimumDistanceToNode)
                {
                    _pathNodes = _previousPathNodes;
                    SetMonsterState(MonsterState.ChasePath, _pathNodes, transform.position);
                }
                break;
        }
        if (_timeUntilScreech < 0.0f)
        {
            _monsterAudio.PlaySFX();
            _timeUntilScreech = SetMonsterScreamTimer();
        }
    }

    private bool CanMonsterSeePlayer()
    {
        // Check if player is very close to monster
        if (Vector3.Distance(_player.transform.position, transform.position) < 5)
        {
            return true;
        }
        
        // Check if player is in line of sight of the monster
        Vector3 directionOfRay = (_player.transform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, directionOfRay);
        if (Physics.Raycast(ray, out RaycastHit hit, maxViewDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
    
    private void RandomlySetNextNode()
    {
        int oldNodeIndicator = _currentNodeIndicator;
        
        // Avoid selecting the same point
        while (_currentNodeIndicator == _previousNodeIndicator)
        {
            _currentNodeIndicator = Random.Range(0, _pathNodes.Count);   
        }

        _agent.destination = _pathNodes[_currentNodeIndicator];
        _previousNodeIndicator = oldNodeIndicator;
    }

    private float SetMonsterScreamTimer()
    {
        return Random.Range(minMonsterScreechRate, maxMonsterScreechRate);
    }

    // Only used for when monster state is setup to none (no path nodes required)
    public void SetMonsterState(MonsterState monsterState)
    {
        _agent.destination = transform.position;
        _monsterState = monsterState;
        _agent.enabled = false;
        transform.position = new Vector3(0.0f, -100.0f, 0.0f);
        _currentNodeIndicator = 0;
        _monsterAnimation.SetStateToWalk();
    }

    public void SetMonsterState(MonsterState monsterState, List<Vector3> newPathNodes, Vector3 newPosition)
    {
        // Disable agent to allow for teleporting to new position
        _agent.enabled = false;
        transform.position = newPosition;
        _agent.enabled = true;
        
        _monsterState = monsterState;
        _currentNodeIndicator = 0;
        _pathNodes = newPathNodes;
        _agent.destination = _pathNodes[_currentNodeIndicator];

        if (monsterState == MonsterState.Chase)
        {
            _monsterAnimation.SetStateToSprint();
        }
        else
        {
            _monsterAnimation.SetStateToWalk();
        }

        // Always plays an SFX when state changes | (is this desired?)
        _monsterAudio.PlaySFX();
        _timeUntilScreech = SetMonsterScreamTimer();
    }

    public MonsterState GetMonsterState()
    {
        return _monsterState;
    }

    public void ListenForSound(Vector3 soundPosition, float soundVolume)
    {
        // sound based tracking is only relevant if the monster is hunting player
        if (_monsterState == MonsterState.ChasePath)
        {
            if (Vector3.Distance(soundPosition, transform.position) <= soundVolume)
            {
                // Store current path nodes for resetting after investigation
                _previousPathNodes = _pathNodes;
                // re-using a list but may be better to overwrite SetMonsterState
                // again in the future
                List<Vector3> investigationPoints = new List<Vector3>();
                investigationPoints.Add(soundPosition);
                SetMonsterState(MonsterState.Investigate, investigationPoints, transform.position);
            }
        }
    }

    private void OnEnable()
    {
        SoundTrigger.OnSoundTriggered += ListenForSound;
    }

    private void OnDisable()
    {
        SoundTrigger.OnSoundTriggered -= ListenForSound;
    }
}
