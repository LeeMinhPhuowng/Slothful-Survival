using UnityEngine;
[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/New Enemy Info")]
public class EnemyInfoSO : ScriptableObject
{
    public int maxHealth;
    public int moveSpeed;
    public int expDrop;
    public int damage;
    public float attackCooldown;
    public ObjectType type;
}
