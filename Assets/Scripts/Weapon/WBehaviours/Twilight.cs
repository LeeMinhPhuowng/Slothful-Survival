    using System.Collections;
    using UnityEngine;

    public class Twilight : AWeaponBehaviour
    {
        [SerializeField] int projectileAmount;
        [SerializeField] WeaponInfoSO info;
        [SerializeField] float timeBetweenProjectiles;

        bool isFiring = false;

        public override void Attack(Transform castPosition)
        {
            if(isFiring)
            {
                return;
            }
            StartCoroutine(Fire());
        }

        IEnumerator Fire()
        {
            isFiring = true;
            var enemies = Physics2D.OverlapCircleAll(this.gameObject.transform.parent.position, info.attackRange, enemyLayer);
            if (enemies.Length == 0)
            {
                isFiring = false;
                yield break;
            }
            for(int i = 0; i < projectileAmount; i++)
            {
                Collider2D enemy = enemies[Random.Range(0, enemies.Length)];
                if(enemy != null)
                {
                    Debug.Log("Attacking" + enemy.name);
                    int tmp = Random.Range(0, 2);
                    if(tmp == 0)
                    {
                        var projectile = ObjectPool.instance.SpawnFromPool(ObjectType.Dark, this.gameObject.transform.parent.position);
                        projectile.GetComponent<Dark>().OnActivate(info.attackDamage, info.attackCooldown, enemy.gameObject);
                        yield return new WaitForSeconds(timeBetweenProjectiles);
                    } 
                    else
                    {
                        var projectile = ObjectPool.instance.SpawnFromPool(ObjectType.Light, this.gameObject.transform.parent.position);
                        projectile.GetComponent<Light>().OnActivate(info.attackDamage, info.attackCooldown, enemy.gameObject);
                        yield return new WaitForSeconds(timeBetweenProjectiles);
                    }    
                
                }    
            }
            isFiring = false;
        }
    }
