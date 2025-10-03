using UnityEngine;

public class EXP : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    private int amount;
    
    [SerializeField] Sprite exp;
    [SerializeField] Sprite bigExp;
    [SerializeField] float doubleRate;
    [SerializeField] float speed;
    public float Speed { get { return speed; } }
    public int Amount { get { return amount; } set { amount = value; } }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerEXP playerEXP = collision.gameObject.GetComponent<PlayerEXP>();
            if(playerEXP != null)
            {
                playerEXP.currentEXP += amount;
                ObjectPool.instance.BackToPool(this.gameObject, ObjectType.EXP);
            }
        }
    }

    public void AmountModifier()
    {
        float value = Random.Range(0f, 1f);
        if (value < doubleRate)
        {
            this.amount *= 2;
            ModifySprite(bigExp);
        }
        else
        {
            ModifySprite(exp);
        }
    }

    private void ModifySprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void MoveTowardsTarget(Vector3 target, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
