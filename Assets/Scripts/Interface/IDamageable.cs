using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);
    void Die();
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }   
}
