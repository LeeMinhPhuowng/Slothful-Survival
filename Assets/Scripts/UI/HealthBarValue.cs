using UnityEngine;
using UnityEngine.UI;

public class HealthBarValue : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (PlayerInfo.instance != null)
        {
            float healthPercent = PlayerInfo.instance.CurrentHealth / PlayerInfo.instance.MaxHealth;
            // Smoothly transition health bar or set it directly
            slider.value = healthPercent;
        }
    }
}
