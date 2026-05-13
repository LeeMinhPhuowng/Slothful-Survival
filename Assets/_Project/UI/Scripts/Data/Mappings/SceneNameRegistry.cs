using System.Collections.Generic;
using Game.UI.Data;
using UnityEngine;

namespace Game.UI.Core
{
    public class SceneNameRegistry : MonoBehaviour, ISceneNameRegistry
    {
        [SerializeField] private List<SceneNameEntry> sceneNameEntries = new();
        private readonly Dictionary<SceneId, string> _sceneId2Name = new();
        private bool _isBuilt;

        private void Awake()
        {
            BuildSceneMap();
        }

        public bool TryGetSceneName(SceneId sceneId, out string sceneName)
        {
            EnsureBuilt();
            return _sceneId2Name.TryGetValue(sceneId, out sceneName);
        }

        private void EnsureBuilt()
        {
            if (_isBuilt)
            {
                return;
            }

            BuildSceneMap();
        }

        private void BuildSceneMap()
        {
            _sceneId2Name.Clear();

            foreach (SceneNameEntry entry in sceneNameEntries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.SceneName))
                {
                    Debug.LogWarning("[SceneNameRegistry] Empty scene entry.");
                    continue;
                }

                if (_sceneId2Name.ContainsKey(entry.SceneId))
                {
                    Debug.LogError($"[SceneNameRegistry] Duplicate scene id: {entry.SceneId}");
                    continue;
                }

                _sceneId2Name.Add(entry.SceneId, entry.SceneName);
            }

            _isBuilt = true;
        }
    }
}
