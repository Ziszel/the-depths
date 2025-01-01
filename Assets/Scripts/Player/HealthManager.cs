using System;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static event Action<float> HealthChanged;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private float damageCooldownTimer;
 
    private int _currentHealth;
    private float _currentDamageCooldownTimer;
    
    private void Start()
    {
        _currentHealth = maxHealth;
        _currentDamageCooldownTimer = 0.0f;
    }

    private void Update()
    {
        if (_currentDamageCooldownTimer > 0.0f)
        {
            _currentDamageCooldownTimer -= Time.deltaTime;
        }
    }

    // useful when resetting by teleporting or if hurting the player via story, etc...
    public void SetHealth(int newHealth)
    {
        _currentHealth = newHealth;
    }

    public int GetHealth()
    {
        return _currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (_currentDamageCooldownTimer <= 0.0f)
        {
            _currentHealth -= damage;
            HealthChanged?.Invoke(_currentHealth);
            ResetCooldownTimer();
        }
    }

    public void RestoreHealth(int restoreHealth)
    {
        _currentHealth += restoreHealth;
        HealthChanged?.Invoke(_currentHealth);
    }

    public void ResetCooldownTimer()
    {
        _currentDamageCooldownTimer = damageCooldownTimer;
    }
}
