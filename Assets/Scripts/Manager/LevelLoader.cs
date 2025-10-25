using TMPro;
using UnityEngine;
using Pathfinding;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] Transform mapContainer;
    private LevelSO level;

    void Start()
    {
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        level = CoverFlow.instance.GetLevelSO();
        levelName.text = level.levelName;
    }

    public void OnPlayButtonClicked()
    {
        Spawner.Instance.InitializeEnemyWaves(level.enemyWaves);
        Instantiate(level.tilemapPrefab, mapContainer);
        AstarPath.active.Scan();
        this.gameObject.SetActive(false);
    }
}
