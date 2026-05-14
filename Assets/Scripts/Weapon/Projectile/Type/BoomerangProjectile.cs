using UnityEngine;

public class BoomerangProjectile : PiercingProjectile
{
    private float _travelDist;
    private Vector3 _startPos;
    private bool _isReturning = false;
    private Rigidbody2D _rb;

    public void SetupBoomerang(float dist)
    {
        _travelDist = dist;
        _startPos = transform.position;
        _isReturning = false;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Rotate the boomerang continuously
        transform.Rotate(0, 0, -720f * Time.deltaTime);

        if (!_isReturning)
        {
            // Phase 1: Go forward
            if (Vector3.Distance(_startPos, transform.position) >= _travelDist)
            {
                _isReturning = true;
            }
        }
        else
        {
            // Phase 2: Return to player
            if (PlayerInfo.instance != null)
            {
                Vector3 playerPos = PlayerInfo.instance.transform.position;
                Vector2 direction = (playerPos - transform.position).normalized;
                
                // Update velocity to head back
                _rb.linearVelocity = direction * 15f; // Hardcoded speed for return, or use moveSpeed

                // Check if reached player
                if (Vector3.Distance(transform.position, playerPos) < 0.5f)
                {
                    BackToPoolImmediately();
                }
            }
            else
            {
                BackToPoolImmediately();
            }
        }
    }
}
