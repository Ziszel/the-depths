using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInputActions;
    public static event Action<InputActionMap> OnInputActionMapChanged;

    private void Awake()
    {
        PlayerInputActions = new PlayerInput();
    }
    
    void Start()
    {
        ToggleActionMap(PlayerInputActions.Player);
    }

    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled)
        {
            return;
        }
        
        PlayerInputActions.Disable();
        OnInputActionMapChanged?.Invoke(actionMap);
        actionMap.Enable();
    }
}
