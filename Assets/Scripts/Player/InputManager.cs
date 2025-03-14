using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInputActions;
    public static event Action<InputActionMap> OnInputActionMapChanged;

    private void Start()
    {
        PlayerInputActions = new PlayerInput();
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
        
        // Testing if action maps are enabling or disabling correctly (They are)
        // Debug.Log("Player: " + PlayerInputActions.Player.enabled);
        // Debug.Log("UI: " + PlayerInputActions.UI.enabled);
        // Debug.Log("Hiding: " + PlayerInputActions.Hiding.enabled);
        // Debug.Log("Death: " + PlayerInputActions.Death.enabled);
    }
}
