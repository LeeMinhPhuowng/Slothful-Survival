using System;
using System.Collections.Concurrent;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Core.Foundation.Logging
{

    /// <summary>
    /// Manages logger instances, configuration, and log targets.
    /// Non-singleton — instantiate via DI or use <see cref="LogManager.Default"/> for the static facade path.
    /// Configure with a <see cref="LogConfigSO"/> via <see cref="Configure(LogConfigSO)"/> at bootstrap.
    /// </summary>
    public sealed class LoggerFactory
    {
        private readonly ConcurrentDictionary<string, ILogger> _name2Logger = new();
        private readonly object _configLock = new();

        private LogConfig _config;
        private ILogTarget[] _targets;

        public LoggerFactory()
        {
            _config = CreateDefaultConfig();
            _targets = new ILogTarget[] { new UnityConsoleTarget() };

    #if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += HandleDomainReload;
    #endif

            var mode = GetEnvironmentMode();
            Debug.Log($"[LoggerFactory] Initialized (defaults). Mode: {mode}, Level: {_config.MinimumLogLevel}, Enabled: {_config.LoggingEnabled}");
        }

    #if UNITY_EDITOR
        ~LoggerFactory()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= HandleDomainReload;
        }
    #endif

        /// <summary>
        /// Replaces the active configuration with one derived from a ScriptableObject.
        /// Safe to call at any time after construction.
        /// </summary>
        public void Configure(LogConfigSO so)
        {
            if (so == null)
            {
                Debug.LogWarning("[LoggerFactory] Configure called with null SO; keeping existing config.");
                return;
            }

            Configure(LogConfig.FromScriptableObject(so));
        }

        /// <summary>
        /// Replaces the active configuration directly.
        /// </summary>
        public void Configure(LogConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            lock (_configLock)
            {
                _config = config;
                Debug.Log($"[LoggerFactory] Configured. Level: {_config.MinimumLogLevel}, Enabled: {_config.LoggingEnabled}");
            }
        }

        public ILogger GetLogger(string category)
        {
            return _name2Logger.GetOrAdd(category, key => new Logger(key, this));
        }

        public ILogger GetLogger<T>() => GetLogger(typeof(T).FullName);

        public bool IsLevelEnabled(string category, LogLevel level)
        {
            if (!_config.LoggingEnabled) return false;
            if (level == LogLevel.Off) return false;

            var threshold = _config.MinimumLogLevel;

            if (_config.CategoryOverrides == null)
            {
                return level >= threshold;
            }

            foreach (var overrideConfig in _config.CategoryOverrides)
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
        /// Writes a log message to all enabled targets.
        /// Called by Logger after level checks pass.
        /// </summary>
        public void WriteLog(LogLevel level, string category, string message)
        {
            foreach (var target in _targets)
            {
                if (!target.IsEnabled) continue;

                try
                {
                    target.Write(level, category, message);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[LoggerFactory] Target {target.GetType().Name} failed: {ex}");
                }
            }
        }

        /// <summary>
        /// Writes a log message with exception to all enabled targets.
        /// </summary>
        public void WriteLog(LogLevel level, string category, string message, Exception exception)
        {
            foreach (var target in _targets)
            {
                if (!target.IsEnabled) continue;

                try
                {
                    target.Write(level, category, message, exception);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[LoggerFactory] Target {target.GetType().Name} failed: {ex}");
                }
            }
        }

        public void AddTarget(ILogTarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            lock (_configLock)
            {
                var newTargets = new ILogTarget[_targets.Length + 1];
                Array.Copy(_targets, newTargets, _targets.Length);
                newTargets[_targets.Length] = target;
                _targets = newTargets;
            }
        }

    #if UNITY_EDITOR || UNITY_INCLUDE_TESTS
        /// <summary>
        /// Resets the factory state for unit tests.
        /// DO NOT CALL IN PRODUCTION CODE.
        /// </summary>
        internal void Reset()
        {
            lock (_configLock)
            {
                _name2Logger.Clear();
                _config = CreateDefaultConfig();
                _targets = new ILogTarget[] { new UnityConsoleTarget() };
                Debug.Log("[LoggerFactory] Reset for testing");
            }
        }
    #endif

    #if UNITY_EDITOR
        private void HandleDomainReload()
        {
            _name2Logger.Clear();
            Debug.Log("[LoggerFactory] Logger cache cleared for domain reload");
        }
    #endif

        private static LogConfig CreateDefaultConfig()
        {
            var config = new LogConfig
            {
                LoggingEnabled = true,
                LogToConsole = true
            };

    #if UNITY_EDITOR
            config.MinimumLogLevel = LogLevel.Debug;
    #elif DEBUG
            config.MinimumLogLevel = LogLevel.Debug;
    #else
            config.MinimumLogLevel = LogLevel.Warn;
    #endif

            return config;
        }

        private static string GetEnvironmentMode()
        {
            if (Application.isEditor) return "Editor";
            return Debug.isDebugBuild ? "Development" : "Release";
        }
    }
}
