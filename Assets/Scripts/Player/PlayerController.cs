using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public enum playerActionState
{
    None = 0,
    Crouching = 1,
    Uncrouching = 2,
    Standing = 3,
    Walking = 4,
    Sprinting = 5,
    InInventory = 6,
    Hiding = 7
}

public class PlayerController : MonoBehaviour
{
    // Event delegates
    public Action OnPlayerDeath;
    public Action OnFlashlightActivated;
    public Action OnFlashlightDeActivated;
    public Action OnCrouchEnabled;
    public Action OnCrouchDisabled;
    public static Action OnLeavingCupboard;
    
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
    
    [Header("Camera targets")]
    [SerializeField] private GameObject crouch; // update OnPlayerKill and look to remove this
    [SerializeField] private Transform head;

    [Header("Footstep play rates (Audio)")] 
    [SerializeField] private float walkingRate = 1.0f;
    [SerializeField] private float sprintingRate = 0.5f;
    
    // This does NOT dictate player height. It is used for ray calculation
    [SerializeField] private float playerHeightRay = 1.0f;
    private playerActionState _currentplayerActionState = playerActionState.Standing;
    private playerActionState _oldPlayerActionState = playerActionState.None;
    
    private float _timeUntilFootstep;
    private float _currentFootstepRate;
    Transform _playerHidingExitTransform; // Used for exiting hiding place. Not the best solution but easiest.
    
    // -- Components --
    public CapsuleCollider walkCollider;
    public CapsuleCollider crouchCollider;
    
    // new input action stuff
    private InputAction _movement;
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
        
        _movement = InputManager.PlayerInputActions.Player.Move;
        
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

        _stamina.OnStaminaReachedZero += SetPlayerValuesToWalk;
    }

    private void Update()
    {
        //Debug.Log(_currentplayerActionState);
        
        switch (_currentplayerActionState)
        {
            case playerActionState.Crouching:
                _timeUntilFootstep = _currentFootstepRate;
                RegenStamBasedOnRate();
                RotatePlayertoFaceCamera();
                break;
            case playerActionState.Uncrouching:
                _timeUntilFootstep = _currentFootstepRate;
                RegenStamBasedOnRate();
                
                if (!DetectObstacleDirectlyAbove())
                {
                    SetPlayerCrouching(false);
                }
                RotatePlayertoFaceCamera();
                break;
            case playerActionState.Standing:
                _timeUntilFootstep = _currentFootstepRate;
                RegenStamBasedOnRate();
                RotatePlayertoFaceCamera();
                break;
            case playerActionState.Walking:
                CheckIfShouldImmediatelyPlayFootstep();
                ReduceTimeUntilFootstep();
                RegenStamBasedOnRate();
                RotatePlayertoFaceCamera();
                break;
            case playerActionState.Sprinting:
                CheckIfShouldImmediatelyPlayFootstep();
                ReduceTimeUntilFootstep();
                _stamina.DepleteStamina();
                RotatePlayertoFaceCamera();
                break;
            case playerActionState.Hiding:
                _timeUntilFootstep = _currentFootstepRate;
                RegenStamBasedOnRate();
                RotatePlayertoFaceCamera();
                break;
            case playerActionState.InInventory:
                break;
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
        switch (_currentplayerActionState)
        {
            case playerActionState.Crouching:
                Move();
                break;
            case playerActionState.Uncrouching:
                Move();
                break;
            case playerActionState.Standing:
                Move();
                break;
            case playerActionState.Walking:
                Move();
                break;
            case playerActionState.Sprinting:
                Move();
                break;
            case playerActionState.Hiding:
                break;
            case playerActionState.InInventory:
                break;
        }

        SetVelocityWhenOnGround();
    }

    private void SetVelocityWhenOnGround()
    {
        if (_floorCollider.IsOnGround())
        {
            _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, maxMovementVelocity);
        }
    }
    
    private void ReduceTimeUntilFootstep()
    {
        _timeUntilFootstep -= Time.deltaTime;
        if (_timeUntilFootstep < 0.0f)
        {
            _playerAudio.PlaySfx();
            _playerAudio.SwapFeet();
            _timeUntilFootstep = _currentFootstepRate;
        }
    }

    private void CheckIfShouldImmediatelyPlayFootstep()
    {
        // Handles if the player pressed a walk button multiple times per frame
        if (_movement.WasPerformedThisFrame())
        {
            if (_floorCollider.IsOnGround())
            {
                _playerAudio.PlaySfx();
            }
        }
    }

    private void RegenStamBasedOnRate()
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

    private void RotatePlayertoFaceCamera()
    {
        // Rotate the player to face the direction the camera is looking at
        transform.rotation = Quaternion.AngleAxis(_mainCamera.transform.eulerAngles.y, Vector3.up);
    }
    
    // Input events
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && _currentplayerActionState == playerActionState.Hiding)
        {
            // TODO: Hard-coded value for moving the player out of the hiding box
            CameraManager.ForceCurrentCameraRotation(Quaternion.LookRotation(_playerHidingExitTransform.forward, Vector3.up));
            transform.position = _playerHidingExitTransform.position;
            _currentplayerActionState = playerActionState.Standing;
            OnLeavingCupboard?.Invoke();
            return;
        }
        
        if (context.started && _currentplayerActionState != playerActionState.InInventory)
        {
            _playerInteractable.UseInteractable();
        }
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (!_currentplayerActionState.Equals(playerActionState.InInventory))
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
        if (_currentplayerActionState.Equals(playerActionState.Standing) ||
            _currentplayerActionState.Equals(playerActionState.Walking))
        {
            if (context.started)
            {
                _cameraManager.SwitchCamera(_cameraManager.leftLeanCamera);
            }
        }

        if (context.canceled)
        {
            _cameraManager.SwitchCamera(_cameraManager.fpsCamera);
        }
    }
    
    public void OnLeanRight(InputAction.CallbackContext context)
    {
        if (_currentplayerActionState.Equals(playerActionState.Standing) ||
            _currentplayerActionState.Equals(playerActionState.Walking))
        {
            if (context.started)
            {
                _cameraManager.SwitchCamera(_cameraManager.rightLeanCamera);
            }
        }

        if (context.canceled)
        {
            _cameraManager.SwitchCamera(_cameraManager.fpsCamera);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started && !_currentplayerActionState.Equals(playerActionState.InInventory) &&
            !_currentplayerActionState.Equals(playerActionState.Hiding))
        {
            if (_floorCollider.IsOnGround() && 
                _stamina.GetCurrentStamina() > 0.0f
                && !_stamina.GetRegeneratingFromZero() && !DetectObstacleDirectlyAbove())
            {
                SetPlayerCrouching(false);
                SetPlayerValuesToSprinting();
                _currentplayerActionState = playerActionState.Sprinting;
            }
        } 

        if (context.canceled && !_currentplayerActionState.Equals(playerActionState.InInventory) &&
            !_currentplayerActionState.Equals(playerActionState.Hiding))
        {
            SetPlayerValuesToWalk();
            _currentplayerActionState = playerActionState.Walking;
        }
    }

    private void SetPlayerValuesToSprinting()
    {
        _currentFootstepRate = sprintingRate;
        maxMovementVelocity = maxSprintVelocity;
    }

    private void SetPlayerValuesToWalk()
    {
        _currentFootstepRate = walkingRate;
        maxMovementVelocity = maxWalkVelocity;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started && !_currentplayerActionState.Equals(playerActionState.InInventory) &&
            !_currentplayerActionState.Equals(playerActionState.Hiding))
        {
            if (_floorCollider.IsOnGround() && _currentplayerActionState.Equals(playerActionState.Standing)
                || _currentplayerActionState.Equals(playerActionState.Walking) ||
                _currentplayerActionState.Equals(playerActionState.Uncrouching) ||
                _currentplayerActionState.Equals(playerActionState.Sprinting))
            {
                SetPlayerCrouching(true);
            }
        }

        if (context.canceled && !_currentplayerActionState.Equals(playerActionState.InInventory)
            && !_currentplayerActionState.Equals(playerActionState.Hiding) 
            && !_currentplayerActionState.Equals(playerActionState.Sprinting))
        {
            {
                _currentplayerActionState = playerActionState.Uncrouching;
            }
        }
    }

    public void OnInventoryOpen(InputAction.CallbackContext context)
    {
        if (context.started && _currentplayerActionState != playerActionState.InInventory)
        {
            GameManager.Instance.ShowInventory(_inventory);
            _oldPlayerActionState = _currentplayerActionState;
            _currentplayerActionState = playerActionState.InInventory;
            // InputManager.ToggleActionMap(InputManager.PlayerInputActions.UI);
        }
        else if (context.started && _currentplayerActionState == playerActionState.InInventory)
        {
            GameManager.Instance.HideInventory();
            _currentplayerActionState = _oldPlayerActionState;
            // InputManager.ToggleActionMap(InputManager.PlayerInputActions.Player);
        }
    }

    // public void OnInventoryClosed(InputAction.CallbackContext context)
    // {
    //     if (context.started)
    //     {
    //         Debug.Log("OnInventoryClosed");
    //         GameManager.Instance.HideInventory();
    //         _currentplayerActionState = _oldPlayerActionState;
    //         InputManager.ToggleActionMap(InputManager.PlayerInputActions.Player);
    //     }
    // }

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

    // Was used for AcitonMap (Hiding) but due to ActionMap bugs this has been removed.
    // Look to re-implement later
    // public void OnLeave(InputAction.CallbackContext context)
    // {
    //     if (context.started && _currentplayerActionState == playerActionState.Hiding)
    //     {
    //         transform.position = _exitHidingPlaceLocation;
    //         InputManager.ToggleActionMap(InputManager.PlayerInputActions.Player);
    //     }
    // }
    
    private void SetPlayerCrouching(bool crouching)
    {
        if (crouching)
        {
            walkCollider.enabled = false;
            crouchCollider.enabled = true;
            OnCrouchEnabled?.Invoke();
            _cameraManager.SwitchCamera(_cameraManager.crouchCamera);
            movementVelocity = crouchVelocity;
            maxMovementVelocity = maxCrouchVelocity;
            _currentplayerActionState = playerActionState.Crouching;
        }
        else
        {
            walkCollider.enabled = true;
            crouchCollider.enabled = false;
            OnCrouchDisabled?.Invoke();
            _cameraManager.SwitchCamera(_cameraManager.fpsCamera);
            movementVelocity = walkVelocity;
            maxMovementVelocity = maxWalkVelocity;
            _currentFootstepRate = walkingRate;
            _currentplayerActionState = playerActionState.Standing;
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
            _moveInput = _movement.ReadValue<Vector2>();
            Vector3 move = cameraTransform.forward * _moveInput.y + cameraTransform.right * _moveInput.x;
            move.y = 0.0f;
            _rb.AddForce(move.normalized * movementVelocity, ForceMode.VelocityChange);

            if (move.x == 0.0f && move.y == 0.0f)
            {
                if (_currentplayerActionState != playerActionState.Crouching &&
                    _currentplayerActionState != playerActionState.Uncrouching)
                {
                    _currentplayerActionState = playerActionState.Standing;
                }
            }
            else
            {
                if (!_currentplayerActionState.Equals(playerActionState.Sprinting) && 
                    !_currentplayerActionState.Equals(playerActionState.Crouching)
                    && !_currentplayerActionState.Equals(playerActionState.Uncrouching))
                {
                    _currentplayerActionState = playerActionState.Walking;
                }
                
            }
        }
        else
        {
            _currentplayerActionState = playerActionState.Standing;
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

    public void SetPlayerState(playerActionState ps)
    {
        _currentplayerActionState = ps;
    }

    public void SetPlayerExitPosition(Transform playerExit)
    {
        _playerHidingExitTransform = playerExit;
    }

    public void EnableInputActions()
    {
        // _inputActions.Enable();
    }
    public void DisableInputActions()
    {
        // _inputActions.Disable();
    }

    private void OnDisable()
    {
        // _inputActions.Player.Disable();
        _monster.OnPlayerWithinDamageDistance -= OnKillPlayer;
    }
}
