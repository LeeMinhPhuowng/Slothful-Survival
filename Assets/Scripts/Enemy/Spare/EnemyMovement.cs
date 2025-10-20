/*using Pathfinding;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    AIPath aiPath;
    AIDestinationSetter destinationSetter;
    Enemy enemy;
    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        enemy = GetComponent<Enemy>(); 
    }

    private void OnEnable()
    {
        destinationSetter.target = GameObject.FindGameObjectWithTag("Player").transform;
        if (aiPath != null)
        {
            aiPath.maxSpeed = enemy.MoveSpeed;
        }
    }

    void Update()
    {
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (aiPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
*/