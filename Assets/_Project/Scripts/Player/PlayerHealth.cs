using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;
    public float HealthRatio => (float)CurrentHealth / maxHealth;

    public event Action Damaged;
    public event Action Died;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        Debug.Log($"Vida del jugador: {CurrentHealth}/{maxHealth}");

        Damaged?.Invoke();

        if (IsDead)
        {
            Debug.Log("DERROTA: el jugador murió");
            Died?.Invoke();
        }
    }
}
