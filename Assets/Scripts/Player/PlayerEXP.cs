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
    }

    private void Start()
    {
        levelManager = PlayerLevelManager.Instance;
        playerInfo = GetComponent<PlayerInfo>();
    }
    private void Update()
    {
        UpdatePlayerLevel();
    }

    public float GetEXPProgress()
    {
        if (playerInfo == null) return 0f;
        int playerLevel = playerInfo.CurrentLevel;
        if (levelManager == null || levelManager.expNeeded == null || playerLevel >= levelManager.expNeeded.Count) return 0f;
        return (float)currentEXP / (float)levelManager.expNeeded[playerLevel];
    }

    public float GetPlayerLevel()
    {
        return playerInfo != null ? playerInfo.CurrentLevel : 0;
    }

    void UpdatePlayerLevel()
    {
        if (playerInfo == null || levelManager == null || levelManager.expNeeded == null) return;
        int playerLevel = playerInfo.CurrentLevel;
        if (playerLevel >= levelManager.expNeeded.Count) return;

        if (currentEXP >= levelManager.expNeeded[playerLevel])
        {
            currentEXP = currentEXP - levelManager.expNeeded[playerLevel];
            playerInfo.CurrentLevel++;

            // Trigger Visual Effects
            LevelUpVFX vfx = GetComponent<LevelUpVFX>();
            if (vfx != null)
            {
                vfx.PlayVFX();
            }

            if (AugmentManager.Instance != null)
            {
                AugmentManager.Instance.OnPlayerLevelUp();
            }
        }
    }
}
