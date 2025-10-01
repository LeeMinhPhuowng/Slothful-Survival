using UnityEngine;

public abstract class AWeaponBehaviour : MonoBehaviour
{
    public LayerMask enemyLayer;
    public abstract void Attack(Transform castPosition);
}

