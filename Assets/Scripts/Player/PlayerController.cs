using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

enum playerCrouchState
{
    None = 0,
    Crouching = 1,
    Uncrouching = 2,
    Standing = 3,
}

public class PlayerController : MonoBehaviour
{
    // Event delegates
    public Action OnPlayerDeath;
    public Action OnFlashlightActivated;
    public Action OnFlashlightDeActivated;
    public Action OnCrouchEnabled;
    public Action OnCrouchDisabled;
    
    [Header("Movement velocity")]
    [SerializeField] private float movementVelocity = 5.0f;
    [SerializeField] private float walkVelocity = 5.0f;
    [SerializeField] private float crouchVelocity = 2.0f;
    
    [Header("Maximum velocities")]
    [SerializeField] private float maxMovementVelocity = 5.0f;
    [SerializeField] private float maxCrouchVelocity = 5.0f;
    [SerializeField] private float maxWalkVelocity = 5.0f;
    [SerializeField] private float maxSprintVelocity = 10.0f;
    [SerializeField] private Transform cameraTransform;
    
    [SerializeField] private float fieldOfViewAngle = 60; // player's cone of vision
    [Header("Camera targets")]
    [SerializeField] private GameObject crouch; // update OnPlayerKill and look to remove this

    [Header("Footstep play rates (Audio)")] 
    [SerializeField] private float walkingRate = 1.0f;
    [SerializeField] private float sprintingRate = 0.5f;
    
    // This does NOT dictate player height. It is used for ray calculation
    [SerializeField] private float playerHeightRay = 1.0f;
    private playerCrouchState _playerCrouchState = playerCrouchState.Standing;
    
    private float _timeUntilFootstep;
    private float _currentFootstepRate;
    private bool _isPlayerWalking;
    private bool _isCrouching; // this is old and should be replaced via state at somepoint
    
    // Components
    public CapsuleCollider walkCollider;
    public CapsuleCollider crouchCollider;
    private PlayerInput _inputActions;
    private Vector2 _moveInput;
    private Rigidbody _rb;
    private Camera _mainCamera;
    private CinemachineCamera _fpsCamera;
    private Monster _monster;
    private FloorCollider _floorCollider;
    private PlayerAudio _playerAudio;
    private Inventory _inventory;
    private Stamina _stamina;
    private HealthManager _healthManager;
    private PlayerHUDUI _playerHUDUI;
    private PlayerInteractable _playerInteractable;
    private CameraManager _cameraManager;
    
    // DEBUG
    [Header("DEBUG")]
    [SerializeField] private bool isDebug;
    
    private void Awake()
    {
        _inputActions = new PlayerInput();
        _inputActions.Enable();
    }

    private void Start()
    {
        _cameraManager = FindAnyObjectByType<CameraManager>();
        _playerHUDUI = GetComponentInChildren<PlayerHUDUI>();
        _healthManager = GetComponent<HealthManager>();
        _rb = GetComponent<Rigidbody>();
        _inventory = GetComponent<Inventory>();
        _stamina = GetComponent<Stamina>();
        _mainCamera = Camera.main;
        _monster = FindAnyObjectByType<Monster>();
        _floorCollider = GetComponentInChildren<FloorCollider>();
        _playerInteractable = GetComponentInChildren<PlayerInteractable>();
        _playerAudio = GetComponentInChildren<PlayerAudio>();
        _fpsCamera = GameObject.Find("FPSCamera").GetComponent<CinemachineCamera>();
        _timeUntilFootstep = 0.0f; // stops it playing immediately or causing error
        _currentFootstepRate = walkingRate;
        _isPlayerWalking = false;
        _isCrouching = false;
        
        if (isDebug)
        {
            // Force the cursor to hide to make game playable in test levels without level manager.
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false;
        }
        
        // Hook up events
        if (_monster)
        {
            _monster.OnPlayerWithinDamageDistance += DamagePlayer;
        }

        _stamina.OnStaminaReachedZero += SetPlayerSprintToWalk;
    }

    private void Update()
    {
        // Handle timers for footsteps
        if (_isPlayerWalking && !_isCrouching)
        {
            _timeUntilFootstep -= Time.deltaTime;
            if (_timeUntilFootstep < 0.0f)
            {
                _playerAudio.PlaySfx();
                _playerAudio.SwapFeet();
                _timeUntilFootstep = _currentFootstepRate;
            }
        }
        else
        {
            _timeUntilFootstep = _currentFootstepRate;
        }

        // Handles if the player pressed a walk button multiple times per frame
        if (_inputActions.Player.Move.WasPerformedThisFrame())
        {
            if (_floorCollider.IsOnGround() && !_isCrouching)
            {
                _playerAudio.PlaySfx();
            }
        }
        
        // stamina
        if (Mathf.Approximately(maxMovementVelocity, maxSprintVelocity))
        {
            _stamina.DepleteStamina();
        }
        else
        {
            if (!_stamina.GetRegeneratingFromZero())
            {
                _stamina.RegenerateStamina(_stamina.GetRegenerationRate());
            }
            else
            {
                _stamina.RegenerateStamina(_stamina.GetBottomOutRegenerationRate());
            }
        }
        
        // Rotate the player to face the direction the camera is looking at
        transform.rotation = Quaternion.AngleAxis(_mainCamera.transform.eulerAngles.y, Vector3.up);

        if (_playerCrouchState == playerCrouchState.Uncrouching)
        {
            if (!DetectObstacleDirectlyAbove())
            {
                SetPlayerCrouching(false);
            }
        }
        
        // DEBUG CONTROLS
        if (isDebug)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                HealPlayerToFull();
            }
        }
    }

    private void FixedUpdate()
    {
        Move();

        if (_floorCollider.IsOnGround())
        {
            _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, maxMovementVelocity);
        }
    }
    
    // Input events
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _playerInteractable.UseInteractable();
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.IsInventoryOpen())
        {
            if (context.started)
            {
                OnFlashlightActivated?.Invoke();
            }

            if (context.canceled)
            {
                OnFlashlightDeActivated?.Invoke();
            }
        }
    }

    public void OnLeanLeft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _cameraManager.SwitchCamera(_cameraManager.leftLeanCamera);
        }

        if (context.canceled)
        {
            _cameraManager.SwitchCamera(_cameraManager.fpsCamera);
        }
    }
    
    public void OnLeanRight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _cameraManager.SwitchCamera(_cameraManager.rightLeanCamera);
        }

        if (context.canceled)
        {
            _cameraManager.SwitchCamera(_cameraManager.fpsCamera);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_floorCollider.IsOnGround() && !_isCrouching && 
                _stamina.GetCurrentStamina() > 0.0f
                && !_stamina.GetRegeneratingFromZero())
            {
                _currentFootstepRate = sprintingRate;
                maxMovementVelocity = maxSprintVelocity;
            }
        } ;

        if (context.canceled)
        {
            SetPlayerSprintToWalk();
        }
    }

    private void SetPlayerSprintToWalk()
    {
        _currentFootstepRate = walkingRate;
        maxMovementVelocity = maxWalkVelocity;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_floorCollider.IsOnGround())
            {
                SetPlayerCrouching(true);
            }
        }

        if (context.canceled)
        {
            _playerCrouchState = playerCrouchState.Uncrouching;
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Mathf.Approximately(Time.timeScale, 1.0f))
            {
                Debug.Log("Open inventory");
                GameManager.Instance.ShowInventory(_inventory);
            }
            else
            {
                Debug.Log("Close inventory");
                GameManager.Instance.HideInventory();
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Pause the game if we're not paused
            if (Mathf.Approximately(Time.timeScale, 1.0f))
            {
                GameManager.Instance.Pause();
            }
            // Unpause the game if we are paused
            else
            {
                GameManager.Instance.Unpause();
            }
            
        }
    }
    
    private void SetPlayerCrouching(bool crouching)
    {
        if (crouching)
        {
            _playerCrouchState = playerCrouchState.Crouching;
            walkCollider.enabled = false;
            crouchCollider.enabled = true;
            OnCrouchEnabled?.Invoke();
            _cameraManager.SwitchCamera(_cameraManager.crouchCamera);
            movementVelocity = crouchVelocity;
            maxMovementVelocity = maxCrouchVelocity;
            _isCrouching = true;
        }
        else
        {
            _playerCrouchState = playerCrouchState.Standing;
            walkCollider.enabled = true;
            crouchCollider.enabled = false;
            OnCrouchDisabled?.Invoke();
            _cameraManager.SwitchCamera(_cameraManager.fpsCamera);
            movementVelocity = walkVelocity;
            maxMovementVelocity = maxWalkVelocity;
            _currentFootstepRate = walkingRate;
            _isCrouching = false;
        }
    }

    // Restoring health will currently make the player fully healthy again
    // NOTE: This may change based on how design progresses.
    private void HealPlayerToFull()
    {
        _healthManager.SetHealth(_healthManager.GetMaxHealth());
        _playerHUDUI.SetImageFromHP(_healthManager.GetHealth());
        _healthManager.ResetCooldownTimer();
    }

    public void DamagePlayer()
    {
        // 1 for now, no idea if this will ever change
        // External classes such as UI should be listening for the event called by
        // this method call so do not implement such things on the player
        _healthManager.TakeDamage(1);

        if (_healthManager.GetHealth() <= 0)
        {
            OnKillPlayer();
        }
    }
    
    public void OnKillPlayer()
    {
        DisableInputActions();
        _playerAudio.PlayDeathSound();
        _fpsCamera.Lens.Dutch = 90.0f;
        _fpsCamera.Target.TrackingTarget = crouch.transform;
        OnPlayerDeath?.Invoke();
    }

    private void Move()
    {
        if (_floorCollider.IsOnGround())
        {
            // Move the player relative to the camera's rotation and clamp the movement velocity
            Vector3 move = cameraTransform.forward * _moveInput.y + cameraTransform.right * _moveInput.x;
            _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
            move.y = 0.0f;
            _rb.AddForce(move.normalized * movementVelocity, ForceMode.VelocityChange);

            if (move.x == 0.0f && move.y == 0.0f)
            {
                _isPlayerWalking = false;
            }
            else
            {
                _isPlayerWalking = true;
            }
        }
        else
        {
            _isPlayerWalking = false;
        }
    }

    private bool DetectObstacleDirectlyAbove()
    {
        Vector3 dir = walkCollider.transform.TransformDirection(Vector3.up);
        // Debug.DrawRay(walkCollider.transform.position, dir.normalized * playerHeightRay, Color.red);
        if (Physics.Raycast(walkCollider.transform.position, dir,
                out RaycastHit hit, playerHeightRay))
        {
            return true;
        }

        return false;
    }

    public Rigidbody GetRigidBody()
    {
        return _rb;
    }

    public CinemachineCamera GetCinemachineCamera()
    {
        return _fpsCamera;
    }

    public GameObject GetCrouchTransform()
    {
        return crouch;
    }

    public void EnableInputActions()
    {
        _inputActions.Enable();
    }
    public void DisableInputActions()
    {
        _inputActions.Disable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        _monster.OnPlayerWithinDamageDistance -= OnKillPlayer;
    }
}
