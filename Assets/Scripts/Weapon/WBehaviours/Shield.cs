using System.Collections;
using UnityEngine;

public class Shield : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject shields;
    private GameObject shieldsObject;

    private void Start()
    {
        shieldsObject = Instantiate(shields, WeaponManager.Instance.gameObject.transform.position, Quaternion.identity, WeaponManager.Instance.gameObject.transform);
    }

    public override void Attack(Transform castPosition)
    {
        Debug.Log("Shield Called");
        StartCoroutine(EnableShields());
    }

    IEnumerator EnableShields()
    {
        shieldsObject.SetActive(true);
        yield return new WaitForSeconds(info.attackCooldown / 2);
        shieldsObject.SetActive(false);
    }
}
