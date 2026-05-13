using UnityEngine;

namespace Game.UI.Installer
{
    public sealed class GlobalRuntimeRoot : MonoBehaviour
    {
        private static GlobalRuntimeRoot _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
