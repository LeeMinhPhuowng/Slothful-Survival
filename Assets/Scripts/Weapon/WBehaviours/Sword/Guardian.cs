using System.Collections;
using UnityEngine;

public class Guardian : AWeaponBehaviour
{
    [SerializeField] WeaponInfoSO info;
    [SerializeField] GameObject blades;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float existTime;
    private GameObject bladesObject;
    private bool _isActive = false;

    private void Start()
    {
        if (PlayerInfo.instance != null)
        {
            bladesObject = Instantiate(blades, PlayerInfo.instance.transform.position, Quaternion.identity, PlayerInfo.instance.transform);
            bladesObject.SetActive(false);
        }
    }

    public override bool Attack(Transform castPosition)
    {
        // If blades are already out, wait.
        if (_isActive) return false;

        if (PlayerInfo.instance == null) return false;

        var enemies = Physics2D.OverlapCircleAll(PlayerInfo.instance.transform.position, info.attackRange, enemyLayer);
        if (enemies.Length == 0) return false;

        StartCoroutine(EnableBlades());
        return true;
    }

    private IEnumerator EnableBlades()
    {
        _isActive = true;
        bladesObject.SetActive(true);
        
        yield return new WaitForSeconds(existTime);
        
        bladesObject.SetActive(false);
        _isActive = false;
    }

    private void OnDestroy()
    {
        if (bladesObject != null)
        {
            Destroy(bladesObject);
        }
    }
}
