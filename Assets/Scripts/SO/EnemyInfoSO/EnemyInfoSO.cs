using UnityEngine;
[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/New Enemy Info")]
public class EnemyInfoSO : ScriptableObject
{
    public float maxHealth;
    public float moveSpeed;
    public int expDrop;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public ObjectType type;
}
