using System;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    public Action OnStaminaReachedZero;
    
    [Header("Stamina Values")]
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float regenerationRate; // every tick
    [SerializeField] private float bottomOutRegenerationRate; // every tick
    [SerializeField] private float depletionRate; // every tick
    
    private float _currentStamina;
    private bool _isRegeneratingFromZero;

    private void Start()
    {
        _isRegeneratingFromZero = false;
        _currentStamina = maxStamina;
    }

    // this function is called whenever the player is in a Sprinting 'state'. (holding down sprint modifier)
    public void DepleteStamina()
    {
        // Debug.Log("Depleting stamina: " + _currentStamina);
        _currentStamina -= depletionRate * Time.deltaTime;
        
        if (_currentStamina < 0)
        {
            _currentStamina = 0;
            _isRegeneratingFromZero = true;
            OnStaminaReachedZero?.Invoke();
        }
    }

    public void RegenerateStamina(float currentRegenerationRate)
    {
        // Debug.Log("Regenerating stamina: " + _currentStamina);
        _currentStamina += currentRegenerationRate * Time.deltaTime;
        
        if (_currentStamina > maxStamina)
        {
            _currentStamina = maxStamina;
            _isRegeneratingFromZero = false;
        }
    }

    public float GetCurrentStamina()
    {
        return _currentStamina;
    }

    public float GetRegenerationRate()
    {
        return regenerationRate;
    }

    public float GetBottomOutRegenerationRate()
    {
        return bottomOutRegenerationRate;
    }

    public bool GetRegeneratingFromZero()
    {
        return _isRegeneratingFromZero;
    }

    // forces current stamina to specific value (if required, not sure if it will be)
    public void SetCurrentStamina(float newValue)
    {
        _currentStamina = newValue;
    }
}
