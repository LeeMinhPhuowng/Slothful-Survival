// using TMPro;
// using UnityEngine;
// using Pathfinding;
// using Game.UI.Data;

// public class LevelLoader : MonoBehaviour
// {
//     [SerializeField] private TextMeshProUGUI levelName;
//     [SerializeField] Transform mapContainer;
//     private LevelSO level;

//     void Start()
//     {
//         this.gameObject.SetActive(true);
//     }

//     void Update()
//     {
//         if (GameplayLaunchContext.MapConfig != null)
//         {
//             level = GameplayLaunchContext.MapConfig;
//         }
//         // else if (CoverFlow.instance != null)
//         // {
//         //     level = CoverFlow.instance.GetLevelSO();
//         // }

//         if (level != null && levelName != null)
//         {
//             levelName.text = level.levelName;
//         }
//     }

//     public void OnPlayButtonClicked()
//     {
//         if (level == null)
//         {
//             Debug.LogError("[LevelLoader] Cannot start gameplay because LevelSO is missing.");
//             return;
//         }

        
//         AstarPath.active.Scan();
//         this.gameObject.SetActive(false);
//     }
// }
