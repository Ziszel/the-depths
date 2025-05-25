using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public Action OnPlayerWithinDamageDistance;
    
    public enum MonsterState
    {
        None, // no state, do nothing
        ChasePath, // Follow a path but allow for chasing the player (maybe rename this Patrol)
        Chase,
        SafePath, // Follow a path but do NOT allow for chasing the player (story elements)
        Investigate // Move to a specific location, then chase OR decide new path. (ChasePath state)
    }

    [Header("How far the monster can see")]
    [SerializeField] private float maxViewDistance;
    
    // Not convinced this is the best location for this, but for now it's ok
    [Header("Monster arena music")]
    [SerializeField] private AudioClip chaseMusic;
    [SerializeField] private AudioClip huntMusic;

    // Components
    private PlayerController _player;
    private NavMeshAgent _agent;
    private MonsterState _monsterState;
    private LevelManager _levelManager;
    private MusicManager _musicManager;
    private MonsterAudio _monsterAudio;
    private MonsterAnimation _monsterAnimation;
    
    // path node logic
    [Header("Path data")] // how often the monster looks for the closest nodes to the player
    [SerializeField] private float updatePathNodeDelta = 10.0f; 
    private int _currentNodeIndicator;
    private int _previousNodeIndicator;
    private List<Vector3> _pathNodes;
    private List<Vector3> _previousPathNodes;
    private readonly float _minimumDistanceToNode = 2.0f;
    private float _timeUntilUpdateNodes;
    
    // Chase helpers
    [SerializeField] private float minimumChaseTime = 2.0f;
    private float _currentChaseTime;

    [Header("Monster values")] 
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
        _musicManager = FindFirstObjectByType<MusicManager>();
        _agent = GetComponent<NavMeshAgent>();
        _player = FindAnyObjectByType<PlayerController>();
        _monsterAudio = GetComponentInChildren<MonsterAudio>();
        _currentChaseTime = 0.0f;
        _timeUntilUpdateNodes = 0.0f;

        // Animation
        _monsterAnimation = GetComponent<MonsterAnimation>();
        _monsterAnimation.SetStateToWalk();
    }

    IEnumerator AttackAnimation(float animationTime)
    {
        yield return new WaitForSeconds(animationTime);
        SetMonsterState(MonsterState.Chase, _pathNodes, transform.position, Vector3.zero);
    }
    
    private void Update()
    {
        // Regardless of state, if the player walks up to the monster, they take damage
        if (!_levelManager.IsPlayerDead())
        {
            // Debug.Log(Vector3.Distance(transform.position, _player.transform.position));
            if (Vector3.Distance(transform.position, _player.transform.position) < _killRange)
            {
                OnPlayerWithinDamageDistance?.Invoke();
                _agent.isStopped = true;
                // Play monster animation
                StartCoroutine(AttackAnimation(2.0f));
            }
        }

        switch (_monsterState)
        {
            case MonsterState.None:
                _agent.speed = _pathSpeed;
                break;
            case MonsterState.ChasePath:
                _agent.speed = _pathSpeed;
                if (CanMonsterSeePlayer())
                {
                    _agent.destination = _player.transform.position;
                    SetMonsterState(MonsterState.Chase, _pathNodes, 
                        transform.position, Vector3.zero);
                }
                else
                {
                    _timeUntilUpdateNodes -= Time.deltaTime;
                    // Depending on scenario this may need additional checks later
                    if (_timeUntilUpdateNodes <= 0.0f)
                    {
                        // Currently UpdatePathNodes will get the closest X path nodes to the player
                        // Eventually, this function may be able to do more with the path nodes specified
                        // LevelManager itself does no work, the PathNodeManager component updates if it exists
                        _pathNodes = _levelManager.UpdatePathNodes(3);
                        _timeUntilUpdateNodes = updatePathNodeDelta;
                    }
                    
                    if (Vector3.Distance(transform.position, _agent.destination) < _minimumDistanceToNode)
                    {
                        RandomlySetNextNode();
                    }
                }
                _timeUntilScreech -= Time.deltaTime;
                break;
            case MonsterState.Chase:
                _agent.speed = _chaseSpeed;
                
                Debug.Log("currentChaseTime: " + _currentChaseTime);

                if (_currentChaseTime > minimumChaseTime)
                {
                    if (CanMonsterSeePlayer())
                    {
                        _currentChaseTime = 0.0f;
                    }
                    else // Monster is exiting chase and returning to patrol
                    {
                        // We can safely set the same _pathNodes as before
                        SetMonsterState(MonsterState.ChasePath, _pathNodes, 
                            transform.position, Vector3.zero);
                    }
                }
                _agent.destination = _player.transform.position;
                _currentChaseTime += Time.deltaTime;
                _timeUntilScreech -= Time.deltaTime;
                break;
            case MonsterState.SafePath:
                _agent.speed = _pathSpeed;

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
                    SetMonsterState(MonsterState.Chase, _pathNodes, 
                        transform.position, Vector3.zero);
                }

                // Reached destination without seeing player, reset to hunting
                if (Vector3.Distance(transform.position, _agent.destination) < _minimumDistanceToNode)
                {
                    // _pathNodes = _previousPathNodes;
                    SetMonsterState(MonsterState.ChasePath, _pathNodes, 
                        transform.position, Vector3.zero);
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
        // Check if player is hiding
        if (_player.GetPlayerActionState() == playerActionState.Hiding)
        {
            // TODO: If the monster sees the player run into a hiding place, they should still kill the player.
            return false;
        }
        
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
            if (hit.collider.CompareTag("PlayerMonsterCollider"))
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

    // Only used for when monster state is set to none (no path nodes required)
    public void SetMonsterState(MonsterState monsterState)
    {
        _agent.destination = transform.position;
        _monsterState = monsterState;
        _agent.enabled = false;
        transform.position = new Vector3(0.0f, -100.0f, 0.0f);
        _currentNodeIndicator = 0;
        _monsterAnimation.SetStateToWalk();
    }

    // Only used for when monster state is set to Investigate
    public void SetMonsterState(List<Vector3> newPathNodes, Vector3 newPosition,
        Vector3 newDestination)
    {
        // Disable agent to allow for teleporting to new position
        _agent.enabled = false;
        transform.position = newPosition;
        _agent.enabled = true;
        
        _monsterState = MonsterState.Investigate;
        _currentNodeIndicator = 0;
        _pathNodes = newPathNodes;
        _agent.destination = newDestination;
        
    }

    public void SetMonsterState(MonsterState monsterState, List<Vector3> newPathNodes, Vector3 newPosition,
        Vector3 newDestination)
    {
        // Disable agent to allow for teleporting to new position
        _agent.enabled = false;
        transform.position = newPosition;
        _agent.enabled = true;
        
        _currentNodeIndicator = 0;
        _pathNodes = newPathNodes;
        
        switch (monsterState)
        {
            case MonsterState.Investigate:
                _agent.destination = newDestination;
                _musicManager.PlayMusic(huntMusic, 2.0f);
                _monsterAnimation.SetStateToWalk();
                break;
            case MonsterState.Chase:
                _agent.destination = _player.transform.position;
                _monsterAnimation.SetStateToSprint();
                _musicManager.PlayMusic(chaseMusic, 0.2f);
                _currentChaseTime = 0.0f;
                break;
            case MonsterState.ChasePath:
                _agent.destination = _pathNodes[_currentNodeIndicator];
                _musicManager.PlayMusic(huntMusic, 2.0f);
                _monsterAnimation.SetStateToWalk();
                break;
        }

        // Always plays an SFX when state changes | (is this desired?)
        _monsterAudio.PlaySFX();
        _timeUntilScreech = SetMonsterScreamTimer();
        _monsterState = monsterState;
    }

    public MonsterState GetMonsterState()
    {
        return _monsterState;
    }

    public void ListenForSound(Vector3 soundPosition, float soundVolume)
    {
        // sound based tracking is only relevant if the monster is hunting player
        if (_monsterState == MonsterState.ChasePath || _monsterState == MonsterState.Investigate)
        {
            if (Vector3.Distance(soundPosition, transform.position) <= soundVolume)
            {
                // Store current path nodes for resetting after investigation
                _previousPathNodes = _pathNodes;
                // re-using a list but may be better to overwrite SetMonsterState
                // again in the future
                // List<Vector3> investigationPoints = new List<Vector3>();
                // investigationPoints.Add(soundPosition);
                SetMonsterState(MonsterState.Investigate, _pathNodes, transform.position,
                    soundPosition);
            }
        }
    }

    private void OnEnable()
    {
        SoundTrigger.OnSoundTriggered += ListenForSound;
        LightMonsterTrigger.OnLightHitMonster += () =>
            SetMonsterState(MonsterState.Chase, _pathNodes, transform.position,
            _player.transform.position);
    }

    private void OnDisable()
    {
        SoundTrigger.OnSoundTriggered -= ListenForSound;
        LightMonsterTrigger.OnLightHitMonster -= () =>
            SetMonsterState(MonsterState.Chase, _pathNodes, transform.position,
                _player.transform.position);
    }
}
