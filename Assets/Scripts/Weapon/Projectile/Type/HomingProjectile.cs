using UnityEngine;

public class HomingProjectile : Projectile
{
    [SerializeField] float rotateSpeed;
    GameObject target;
    bool hasHit;

    private void OnEnable()
    {
        hasHit = false;
    }
    private void Update()
    {
        CurlyMoveTowards(target, rotateSpeed);
    }
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}
