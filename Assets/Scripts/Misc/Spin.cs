using UnityEngine;
using UnityEngine.UIElements;

public class Spin : MonoBehaviour
{
    [SerializeField] float speed;
    void Update()
    {
        this.gameObject.transform.Rotate(0, 0, 1 * speed);         
    }
}
