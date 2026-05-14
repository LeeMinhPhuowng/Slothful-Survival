using UnityEngine;

public abstract class AWeaponBehaviour : MonoBehaviour
{
    public LayerMask enemyLayer;
    public abstract bool Attack(Transform castPosition);
}

