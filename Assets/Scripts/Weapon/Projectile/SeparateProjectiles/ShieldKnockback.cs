using UnityEngine;

public class ShieldKnockback : MonoBehaviour
{
    [SerializeField] Vector2 force;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            Rigidbody2D rb = enemy.gameObject.GetComponent<Rigidbody2D>();
            rb.AddForce(force, ForceMode2D.Impulse);
            Debug.Log("Force Added");
        }
    }
}
