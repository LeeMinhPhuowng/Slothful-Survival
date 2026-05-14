using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
    void Die();
    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }   
}
