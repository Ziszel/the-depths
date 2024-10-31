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
        SafePath // Follow a path but do NOT allow for chasing the player
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
    private readonly float _minimumDistanceToNode = 2.0f;
    private bool _isPlayerLookingAtMe;
    
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
    [SerializeField] private float monsterScreechRate = 7.5f; 
    private float _timeUntilScreech;
    
    private void Start()
    {
        _monsterState = MonsterState.None;
        _isPlayerLookingAtMe = false;
        _levelManager = FindAnyObjectByType<LevelManager>();
        _agent = GetComponent<NavMeshAgent>();
        _player = FindAnyObjectByType<PlayerController>();
        _monsterAudio = GetComponentInChildren<MonsterAudio>();
        _currentChaseTime = 0.0f;

        // subscribe to events
        _player.OnPlayerLookingAtMonster += SetIsPlayerLooking;
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
        }
        if (_timeUntilScreech < 0.0f)
        {
            _monsterAudio.PlaySFX();
            _timeUntilScreech = monsterScreechRate;
        }
    }

    private bool CanMonsterSeePlayer()
    {
        // Check if player is very close to monster
        if (Vector3.Distance(_player.transform.position, transform.position) < 5)
        {
            return true;
        }

        // Check if the player is looking at the monster
        if (_isPlayerLookingAtMe)
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

    private void SetIsPlayerLooking(bool newIsPlayerLooking)
    {
        _isPlayerLookingAtMe = newIsPlayerLooking;
    }

    private void OnDisable()
    {
        _player.OnPlayerLookingAtMonster -= SetIsPlayerLooking;
    }

    // Only used for when monster state is setup to none
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
        _agent.enabled = false;
        Debug.Log("monster position: " + transform.position);
        transform.position = newPosition;
        Debug.Log("monster position: " + transform.position);
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

        // Always plays an SFX when state changes
        Debug.Log("monster screeching on state change");
        _monsterAudio.PlaySFX();
        _timeUntilScreech = monsterScreechRate;
    }

    public MonsterState GetMonsterState()
    {
        return _monsterState;
    }
}
