using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrioBeginning : AWeaponBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] WeaponInfoSO info;
    [SerializeField] ObjectType[] projectileTypes;

    [Header("Burst Settings")]
    [SerializeField] int projectilesPerWave = 10;
    [SerializeField] int totalWaves = 3;
    [SerializeField] float timeBetweenWaves = 0.15f;
    [SerializeField] float angleOffsetPerWave = 20f;
    [SerializeField] float spriteAngleOffset = 0f; // New: Offset to fix sprite orientation

    private bool isFiring = false;

    public override bool Attack(Transform castPosition)
    {
        if (isFiring) return false;
        
        if (projectileTypes == null || projectileTypes.Length == 0)
        {
            Debug.LogWarning($"[TrioBeginning] No projectile types assigned on {gameObject.name}!");
            return false;
        }

        if (ObjectPool.instance == null)
        {
            Debug.LogError("[TrioBeginning] ObjectPool instance is missing!");
            return false;
        }

        StartCoroutine(FireTrioWaves(castPosition));
        return true;
    }

    IEnumerator FireTrioWaves(Transform castPosition)
    {
        isFiring = true;

        for (int w = 0; w < totalWaves; w++)
        {
            ObjectType currentType = projectileTypes[w % projectileTypes.Length];

            float startAngle = w * angleOffsetPerWave;
            float angleStep = 360f / projectilesPerWave;

            for (int i = 0; i < projectilesPerWave; i++)
            {
                float currentAngle = startAngle + (i * angleStep);
                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle + spriteAngleOffset);

                var projectileObj = ObjectPool.instance.SpawnFromPool(
                    currentType,
                    PlayerInfo.instance.transform.position,
                    rotation,
                    (o) => {
                        var proj = o.GetComponent<Projectile>();
                        if (proj != null)
                        {
                            proj.Init(info.attackDamage, info.attackCooldown);
                            proj.enemyLayer = enemyLayer;
                            proj.transform.rotation = rotation;
                        }
                    }
                );

                if (projectileObj != null)
                {
                    var pComp = projectileObj.GetComponent<Projectile>();
                    if (pComp != null)
                    {
                        // Force Wake up Rigidbody
                        Rigidbody2D rb = projectileObj.GetComponent<Rigidbody2D>();
                        if (rb != null)
                        {
                            rb.simulated = true;
                            rb.WakeUp();
                        }
                        
                        pComp.MoveForward();
                    }
                }
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        isFiring = false;
    }
}