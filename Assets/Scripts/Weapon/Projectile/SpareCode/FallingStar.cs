using System.Collections;
using UnityEngine;

public class FallingStar : PiercingProjectile
{
    public void OnActivate(int damage, int lifetime)
    {
        Init(damage, lifetime);
        MoveDownward();
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
