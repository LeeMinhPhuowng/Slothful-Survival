using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    PlayerInfo playerInfo;
    Vector2 moveValue;
    Animator animator;
    Rigidbody2D rb;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Move();
    }

    void OnMove(InputValue value)
    {
        moveValue = value.Get<Vector2>();
        RunAnimationCheck();
        FlipSprite();
    }

    void Move()
    {
        Vector2 currentPosition = transform.position;
        Vector2 moveOffset = moveValue * (PlayerInfo.instance.MoveSpeed * Time.deltaTime);
        Vector2 targetPosition = currentPosition + moveOffset;
        rb.MovePosition(targetPosition);
    }

    void RunAnimationCheck()
    {
        if (moveValue.x == 0 && moveValue.y == 0)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            animator.SetBool("Run", true);
        }
    }

    void FlipSprite()
    {
        Vector3 scale = transform.localScale;  
        if(moveValue.x > 0)
        {
            scale.x = 1;
        }
        else if(moveValue.x < 0)
        {
            scale.x = -1;
        }
        transform.localScale = scale;
    }

}

