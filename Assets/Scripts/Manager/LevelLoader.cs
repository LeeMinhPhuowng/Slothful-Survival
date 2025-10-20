using TMPro;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] Transform mapContainer;
    private LevelSO level;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        level = CoverFlow.instance.GetLevelSO();
        levelName.text = level.levelName;
    }

    public void OnPlayButtonClicked()
    {
        Spawner.Instance.InitializeEnemyWaves(level.enemyWaves);
        Instantiate(level.tilemapPrefab, mapContainer);
        this.gameObject.SetActive(false);
    }
}
