using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyInfoSO info;
    public Transform VFXPlayer;
    private int moveSpeed;
    private int currentHealth;

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
}
