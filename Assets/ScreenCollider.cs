using UnityEngine;
using UnityEngine.UIElements;

public class ScreenCollider : MonoBehaviour
{
    EdgeCollider2D edgeCollider;

    private void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();   
    }

    void LateUpdate()
    {
        CreateEdges();
    }

    void CreateEdges()
    {
        Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        Vector2[] edgePoints = new Vector2[5];
        edgePoints[0] = bottomLeft;
        edgePoints[1] = new Vector2(bottomLeft.x, topRight.y);
        edgePoints[2] = topRight;
        edgePoints[3] = new Vector2(topRight.x, bottomLeft.y);
        edgePoints[4] = bottomLeft;
        edgeCollider.points = edgePoints;
    }

    Vector2 GetClosestPoint(Vector2 position)
    {
        Vector2[] edgePoints = edgeCollider.points;
        float nearestDistance = Vector2.Distance(position, edgePoints[0]);
        Vector2 closestPoint = edgePoints[0];
        for(int i = 1; i < edgePoints.Length; i++)
        {
            float currentDistance = Vector2.Distance(position, edgePoints[i]);
            if (currentDistance < nearestDistance)
            {
                closestPoint = edgePoints[i];
                nearestDistance = currentDistance;
            }
        }    
        return closestPoint;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if(!collision.gameObject.CompareTag("BouncingObject")) { return; }
        Rigidbody2D projectileRB = collision.gameObject.GetComponent<Rigidbody2D>();
        RaycastHit2D[] hit2D = Physics2D.RaycastAll(collision.transform.position, projectileRB.linearVelocity);
        Vector2 contactPoint = hit2D[1].point;
        Vector2 normal = Vector2.Perpendicular((contactPoint - GetClosestPoint(collision.transform.position)).normalized);
        projectileRB.linearVelocity = Vector2.Reflect(projectileRB.linearVelocity, normal);
        Debug.Log("Triggered");
    }
    
}
