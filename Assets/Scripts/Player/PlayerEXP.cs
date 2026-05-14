using Unity.VisualScripting;
using UnityEngine;

public class PlayerEXP : MonoBehaviour
{
    public int currentEXP;
    PlayerLevelManager levelManager;
    PlayerInfo playerInfo;

    public static PlayerEXP instance;

    private void Awake()
    {
        instance = this;
        levelManager = PlayerLevelManager.Instance;
        playerInfo = GetComponent<PlayerInfo>();
    }

    private void Update()
    {
        UpdatePlayerLevel();
    }

    public float GetEXPProgress()
    {
        int playerLevel = playerInfo.CurrentLevel;
        return (float)currentEXP / (float)levelManager.expNeeded[playerLevel];
    }

    public float GetPlayerLevel()
    {
        return playerInfo.CurrentLevel;
    }

    void UpdatePlayerLevel()
    {
        int playerLevel = playerInfo.CurrentLevel;
        if (currentEXP >= levelManager.expNeeded[playerLevel])
        {
            currentEXP = currentEXP - levelManager.expNeeded[playerLevel];
            playerInfo.CurrentLevel++;
            AugmentManager.Instance.OnPlayerLevelUp();
        }
    }
}
