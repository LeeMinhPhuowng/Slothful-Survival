using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PB28A : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] Vector2[] directions;
    [SerializeField] float timeBetweenFires;

    bool isFiring = false;
    public override void Attack(Transform castPosition)
    {
        if (isFiring) { return; }
        StartCoroutine(FireLasers());
    }
    
    IEnumerator FireLasers()
    {
        isFiring = true;
        for(int i = 0; i < directions.Length; i++)
        {
            Vector2 direction = (directions[i]).normalized;
            Quaternion rotation = Quaternion.FromToRotation(Vector2.right, direction);
            var projectileObj = ObjectPool.instance.SpawnFromPool(ObjectType.Laser, PlayerInfo.instance.gameObject.transform.position + (Vector3)directions[i], rotation, (o) => { o.GetComponent<Projectile>().Init(info.attackDamage, info.attackCooldown); });
            projectileObj.GetComponent<Projectile>().MoveForward();

            yield return new WaitForSeconds(timeBetweenFires);
        }
        isFiring = false;
    }
}
