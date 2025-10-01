using UnityEngine;

public class AOEProjectile : Projectile
{
    void Start()
    {
        MoveForward();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
