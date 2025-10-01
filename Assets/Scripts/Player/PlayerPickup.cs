using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public LayerMask pickupsLayer;
    void Update()
    {
        Collider2D[] expObj = Physics2D.OverlapCircleAll(this.transform.position, PlayerInfo.instance.PickupRange, pickupsLayer);
        foreach (var exp in expObj)
        {
            EXP expComponent = exp.gameObject.GetComponent<EXP>();
            expComponent.MoveTowardsTarget(gameObject.transform.position, expComponent.Speed);
        }
    }
}
