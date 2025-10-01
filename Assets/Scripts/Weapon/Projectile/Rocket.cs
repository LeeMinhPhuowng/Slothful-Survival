using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEditor.FilePathAttribute;

public class Rocket : AOEProjectile
{
    Rigidbody2D rigidbody;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (rigidbody.linearVelocity.sqrMagnitude > 0.01f) // tránh lỗi khi velocity = 0
        {
            float angle = Mathf.Atan2(rigidbody.linearVelocity.y, rigidbody.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
