#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.Foundation.Logging
{

    /// <summary>
    /// Editor window for viewing and modifying runtime and asset logging configuration.
    /// </summary>
    public class LogControlWindow : EditorWindow
    {
        /// <summary>
        /// Default location for newly created LogConfig assets.
        /// Existing assets are located via AssetDatabase regardless of path.
        /// </summary>
        private const string DefaultConfigFolder = "Assets/_Project/Core/Foundation/Logging/Configs";
        private const string DefaultConfigFileName = "SO_LogConfig.asset";

        [MenuItem("Window/Core/Log Control")]
        public static void ShowWindow()
        {
            var window = GetWindow<LogControlWindow>("Log Control");
            window.minSize = new Vector2(300, 250);
            window.Show();
        }

        private void OnGUI()
        {
            LogConfigSO currentConfig = ResolveConfig();

            if (!currentConfig)
            {
                DrawNoConfigWarning();
                return;
            }

            DrawHeader(currentConfig);

            EditorGUILayout.Space(10);

            DrawMainControls(currentConfig);

            EditorGUILayout.Space(10);

            DrawQuickProfiles(currentConfig);

            if (Application.isPlaying)
            {
                Repaint();
            }
        }

        /// <summary>
        /// Resolves the active logging configuration via AssetDatabase (no Resources.Load).
        /// Returns the first LogConfigSO asset found in the project.
        /// </summary>
        private LogConfigSO ResolveConfig()
        {
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(LogConfigSO));
            if (guids == null || guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<LogConfigSO>(path);
        }

        /// <summary>
        /// Draws current logging status and mode (Editor vs Runtime).
        /// </summary>
        private void DrawHeader(LogConfigSO config)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Log System Controller", EditorStyles.boldLabel);

            if (Application.isPlaying)
            {
                GUI.color = Color.green;
                EditorGUILayout.LabelField("MODE: RUNTIME (Editing Live Instance)", EditorStyles.miniBoldLabel);
            }
            else
            {
                GUI.color = new Color(0.7f, 0.7f, 1f); // Light blue
                EditorGUILayout.LabelField("MODE: EDITOR (Editing Asset File)", EditorStyles.miniBoldLabel);
            }
            GUI.color = Color.white;

            EditorGUILayout.Separator();

            string status = config.LoggingEnabled ? "ACTIVE" : "DISABLED";
            GUI.color = config.LoggingEnabled ? Color.green : Color.red;
            EditorGUILayout.LabelField($"Current Status: {status}", EditorStyles.boldLabel);
            GUI.color = Color.white;

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Renders primary logging configuration controls.
        /// </summary>
        private void DrawMainControls(LogConfigSO config)
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            bool newEnabled = EditorGUILayout.Toggle("Logging Enabled", config.LoggingEnabled);

            LogLevel newLevel = (LogLevel)EditorGUILayout.EnumPopup("Min Log Level", config.MinimumLogLevel);

            bool newConsole = EditorGUILayout.Toggle("Log To Console", config.LogToConsole);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(config, "Change Log Settings");
                config.LoggingEnabled = newEnabled;
                config.MinimumLogLevel = newLevel;
                config.LogToConsole = newConsole;
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(config);
                }
                config.NotifyConfigChanged();
            }
        }

        /// <summary>
        /// Displays preset buttons for fast configuration switching.
        /// </summary>
        private void DrawQuickProfiles(LogConfigSO config)
        {
            GUILayout.Label("Quick Profiles", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("DEVELOPMENT\n(Debug + All)", GUILayout.Height(40)))
            {
                config.SetDevelopmentMode();
            }

            if (GUILayout.Button("PRODUCTION\n(Warning Only)", GUILayout.Height(40)))
            {
                config.SetProductionMode();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("MUTE ALL", GUILayout.Height(25)))
            {
                config.DisableAllLogging();
            }
        }

        /// <summary>
        /// Shows a warning UI when no LogConfigSO asset is found anywhere in the project.
        /// </summary>
        private void DrawNoConfigWarning()
        {
            EditorGUILayout.HelpBox(
                $"No LogConfigSO asset found in the project.\nA new one will be created at:\n{DefaultConfigFolder}/{DefaultConfigFileName}",
                MessageType.Error);

            if (GUILayout.Button("Create Config Asset"))
            {
                CreateConfigAsset();
            }
        }

        /// <summary>
        /// Creates a default LogConfig asset at the canonical Configs folder.
        /// </summary>
        private void CreateConfigAsset()
        {
            string absoluteFolder = Path.Combine(Application.dataPath,
                DefaultConfigFolder.Substring("Assets/".Length));

            if (!Directory.Exists(absoluteFolder))
            {
                Directory.CreateDirectory(absoluteFolder);
            }

            var asset = CreateInstance<LogConfigSO>();
            string assetPath = $"{DefaultConfigFolder}/{DefaultConfigFileName}";
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    #endif
}
