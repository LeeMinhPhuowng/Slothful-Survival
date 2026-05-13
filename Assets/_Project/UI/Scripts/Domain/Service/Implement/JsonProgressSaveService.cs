using System.IO;
using Game.UI.Save;
using UnityEngine;

namespace Game.UI.Service
{
    public sealed class JsonProgressSaveService : IProgressSaveService
    {
        private readonly string _savePath;

        public JsonProgressSaveService(string saveFileName = "player_progress.json")
        {
            _savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        }

        public bool HasSave()
        {
            return File.Exists(_savePath);
        }

        public PlayerProgressData Load()
        {
            if (!HasSave())
            {
                return null;
            }

            string json = File.ReadAllText(_savePath);
            return JsonUtility.FromJson<PlayerProgressData>(json);
        }

        public void Save(PlayerProgressData data)
        {
            if (data == null)
            {
                return;
            }

            string directory = Path.GetDirectoryName(_savePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(_savePath, json);
        }

        public void Delete()
        {
            if (HasSave())
            {
                File.Delete(_savePath);
            }
        }
    }
}
