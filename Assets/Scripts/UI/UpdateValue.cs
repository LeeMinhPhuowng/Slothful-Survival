using UnityEngine;
using UnityEngine.UI;

public class UpdateValue : MonoBehaviour
{
    Slider slider;
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    public void SetHealth(float value)
    {
        slider.value = value;
    }
}
