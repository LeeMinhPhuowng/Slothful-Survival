using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject[] lasers;
    [SerializeField] float timeBetweenProjectiles;
    private void Awake()
    {
        foreach (var laser in lasers)
        {
            laser.GetComponent<Projectile>().Init(info.attackDamage, info.attackCooldown);
        }
    }
    private void OnEnable()
    {
        foreach(GameObject laser in lasers)
        { 
            laser.SetActive(false); 
        }
        StartCoroutine(FireLasers());       
    }

    IEnumerator FireLasers()
    {
        foreach (GameObject laser in lasers)
        {
            laser.SetActive(true);
            yield return new WaitForSeconds(timeBetweenProjectiles);
        }
    }    
}
