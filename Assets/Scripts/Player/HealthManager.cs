using System;
using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static event Action<float> HealthChanged;
    
    [SerializeField] private int maxHealth;
    [SerializeField] private float damageCooldownTimer;
    [SerializeField] private float regenerationRate;
 
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

    IEnumerator RegenerateOverTime()
    {
        yield return new WaitForSeconds(regenerationRate);
        RestoreHealth(1);
    }

    public void TakeDamage(int damage)
    {
        if (_currentDamageCooldownTimer <= 0.0f)
        {
            _currentHealth -= damage;
            HealthChanged?.Invoke(_currentHealth);
            ResetCooldownTimer();
            
            // co-routine called using name of for reference to stopping specific co-routine later 
            StopCoroutine("RegenerateOverTime");
            StartCoroutine("RegenerateOverTime");
        }
    }

    public void RestoreHealth(int restoreHealth)
    {
        if (_currentHealth < maxHealth)
        {
            _currentHealth += restoreHealth;
            HealthChanged?.Invoke(_currentHealth);
            
            // Debug.Log($"Health changed to {_currentHealth}");

            if (_currentHealth < maxHealth)
            {
                // co-routine called using name of for reference to stopping specific co-routine later
                StopCoroutine("RegenerateOverTime");
                StartCoroutine("RegenerateOverTime");
            }
        }
    }

    public void ResetCooldownTimer()
    {
        _currentDamageCooldownTimer = damageCooldownTimer;
    }
}
