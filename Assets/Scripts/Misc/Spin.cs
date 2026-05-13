using UnityEngine;
using UnityEngine.UIElements;

public class Spin : MonoBehaviour
{
    [SerializeField] float speed;
    
    void Update()
    {
        gameObject.transform.Rotate(0, 0, 1 * speed);         
    }
}
