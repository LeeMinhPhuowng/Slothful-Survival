using System.Collections;
using UnityEngine;

public class Dark : HomingProjectile
{
    public void OnActivate(int damage, int lifetime, GameObject target)
    {
        Init(damage, lifetime);
        SetTarget(target);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
