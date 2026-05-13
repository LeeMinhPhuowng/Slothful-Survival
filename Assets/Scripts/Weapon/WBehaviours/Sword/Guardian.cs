using System.Collections;
using UnityEngine;

public class Guardian : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject blades;
    private GameObject bladesObject;
    private void Start()
    {
        bladesObject = Instantiate(blades, WeaponManager.Instance.gameObject.transform.position, Quaternion.identity, WeaponManager.Instance.gameObject.transform);
    }
    public override void Attack(Transform castPosition)
    {
        Debug.Log("Guardian Attack Called");
        StartCoroutine(EnableBlades());
    }

    private IEnumerator EnableBlades()
    {
        bladesObject.SetActive(false);
        yield return new WaitForSeconds(info.attackCooldown / 2);
        bladesObject.SetActive(true);
    }
}
