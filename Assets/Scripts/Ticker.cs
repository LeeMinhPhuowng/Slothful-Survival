using UnityEngine;

public class Ticker : MonoBehaviour
{
    public float tickTime = 0.2f;

    private float tickerTimer;
    public delegate void TickAction();

    public static event TickAction OnTickAction;

    private void Update()
    {
        tickerTimer += Time.deltaTime;
        if (tickerTimer >= tickTime)
        {
            tickerTimer = 0;
            TickEvent();
        }
    }

    private void TickEvent()
    {
        OnTickAction?.Invoke();
    }
}
