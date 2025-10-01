using UnityEngine;

public class HealthBarCanvas : MonoBehaviour
{
    public static HealthBarCanvas Instance;
    private void Awake()
    {
        Instance = this;
    }
}
