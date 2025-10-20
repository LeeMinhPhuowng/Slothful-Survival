using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BloodSlash : MonoBehaviour
{
    [SerializeField] WeaponInfoSO info;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;
    private Sprite lastSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    private void LateUpdate()
    {
        if(lastSprite != spriteRenderer.sprite)
        {
            UpdateCollider();
            lastSprite = spriteRenderer.sprite;
        }
    }

    void UpdateCollider()
    {
        if (spriteRenderer.sprite == null) return;

        polygonCollider.pathCount = spriteRenderer.sprite.GetPhysicsShapeCount();
        List<Vector2> path = new List<Vector2>();
        for(int i = 0; i < polygonCollider.pathCount; i++)
        {
            path.Clear();
            spriteRenderer.sprite.GetPhysicsShape(i, path);
            polygonCollider.SetPath(i, path);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OK");
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(info.attackDamage);
        }
        else Debug.Log("Nah");
    }
}
