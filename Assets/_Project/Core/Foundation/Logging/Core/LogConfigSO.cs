using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Foundation.Logging
{

    [CreateAssetMenu(fileName = "SO_LogConfig", menuName = "Core/Foundation/Log Config")]
    public class LogConfigSO : ScriptableObject
    {
        [System.Serializable]
        public struct CategoryConfig
        {
            [FormerlySerializedAs("categoryName")]
            [Tooltip("e.g. 'Core.Combat' or 'Network'")]
            public string CategoryName;

            [FormerlySerializedAs("minLevel")]
            public LogLevel MinLevel;
        }

        #region Fields

        [Header("Global Settings")]
        [Tooltip("Minimum log level to display. Logs below this level will be filtered out.")]
        [SerializeField] private LogLevel minimumLogLevel = LogLevel.Info;

        [Tooltip("Enable or disable all logging globally")]
        [SerializeField] private bool loggingEnabled = true;

        [Header("Category Overrides")]
        [Tooltip("Specific log levels for different categories/namespaces.")]
        [SerializeField] private System.Collections.Generic.List<CategoryConfig> categoryOverrides = new();

        [Header("Output Settings")]
        [Tooltip("Write logs to Unity console")]
        [SerializeField] private bool logToConsole = true;

        #endregion

        public event System.Action OnConfigChanged;

        #region Properties

        public LogLevel MinimumLogLevel
        {
            get => minimumLogLevel;
            set => minimumLogLevel = value;
        }

        public bool LoggingEnabled
        {
            get => loggingEnabled;
            set => loggingEnabled = value;
        }

        public bool LogToConsole
        {
            get => logToConsole;
            set => logToConsole = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Public accessor for category overrides.
        /// Used by LogConfig.FromScriptableObject() to copy settings.
        /// </summary>
        public System.Collections.Generic.List<CategoryConfig> GetCategoryOverrides()
        {
            return categoryOverrides;
        }

        /// <summary>
        /// Checks if a specific log level should be logged based on the minimum level
        /// </summary>
        public bool IsLevelEnabled(string category, LogLevel level)
        {
            if (!loggingEnabled) return false;
            if (level == LogLevel.Off) return false;

            var threshold = minimumLogLevel;

            if (categoryOverrides == null)
            {
                return level >= threshold;
            }

            foreach (var overrideConfig in categoryOverrides)
            {
                if (string.IsNullOrEmpty(overrideConfig.CategoryName))
                {
                    continue;
                }

                if (category != overrideConfig.CategoryName &&
                    !category.StartsWith(overrideConfig.CategoryName + "."))
                {
                    continue;
                }

                threshold = overrideConfig.MinLevel;
                break;
            }

            return level >= threshold;
        }

        /// <summary>
        /// Notifies listeners that logging settings have changed.
        /// </summary>
        public void NotifyConfigChanged()
        {
            OnConfigChanged?.Invoke();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                NotifyConfigChanged();
            }
        }

        #endregion

        #region Editor Helper Methods

    #if UNITY_EDITOR

        [ContextMenu("Reset to Defaults")]
        public void ResetToDefaults()
        {
            minimumLogLevel = LogLevel.Info;
            loggingEnabled = true;
            logToConsole = true;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Disable All Logging")]
        public void DisableAllLogging()
        {
            loggingEnabled = false;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Set to Development Mode")]
        public void SetDevelopmentMode()
        {
            minimumLogLevel = LogLevel.Debug;
            loggingEnabled = true;
            logToConsole = true;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Set to Production Mode")]
        public void SetProductionMode()
        {
            minimumLogLevel = LogLevel.Warn;
            loggingEnabled = true;
            logToConsole = true;
            UnityEditor.EditorUtility.SetDirty(this);
        }
    #endif

        #endregion
    }
}
