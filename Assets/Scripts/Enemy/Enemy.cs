using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyInfoSO info;
    public Transform VFXPlayer;

    private EnemyHealth enemyHealth;
    private int moveSpeed;
    private int currentHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        InitializeFromStat();
    }

    private void InitializeFromStat()
    {
        currentHealth = info.maxHealth;
        moveSpeed = info.moveSpeed;
    }

    public int GetMoveSpeed()
    {
        return moveSpeed;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void DecreaseCurrentHealth(int value)
    {
        currentHealth -= value;
    }

    public void ReceiveDamage(int damage)
    {
        enemyHealth.TakeDamage(damage);
    }
    
}
