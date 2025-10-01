using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerLevelManager : MonoBehaviour
{

    public List<int> expNeeded;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider expBar;

    public static PlayerLevelManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    { 

    }

    private void Update()
    {
        levelText.text = PlayerEXP.instance.GetPlayerLevel().ToString();
        expBar.value = PlayerEXP.instance.GetEXPProgress();
    }
}
