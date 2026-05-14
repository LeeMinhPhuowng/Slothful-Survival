using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZoneDamage : MonoBehaviour
{
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] WeaponInfoSO info;
    private float timeElapsed = 0f;

    List<Enemy> enemies = new List<Enemy>();

    private void OnEnable()
    {
        StartCoroutine(DealDamage());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemies.Add(enemy);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemies.Contains(enemy))
            {
                enemies.Remove(enemy);
            }    
        }
    }

    IEnumerator DealDamage()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeBetweenAttacks);

            if (info == null) continue;

            // Use a list to store enemies that need to be removed
            List<Enemy> toRemove = new List<Enemy>();

            foreach (Enemy enemy in enemies)
            {
                // Check if enemy still exists and is active
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    enemy.TakeDamage(info.attackDamage);
                }
                else
                {
                    toRemove.Add(enemy);
                }
            }

            // Clean up the list
            foreach (Enemy remove in toRemove)
            {
                enemies.Remove(remove);
            }
        }
    }

}
