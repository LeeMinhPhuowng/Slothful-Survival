using System.Collections.Generic;

namespace Core.Foundation.Logging
{

    public class LogConfig
    {
        public struct CategoryOverride
        {
            public string CategoryName;
            public LogLevel MinLevel;

            public CategoryOverride(string categoryName, LogLevel minLevel)
            {
                CategoryName = categoryName;
                MinLevel = minLevel;
            }
        }

        public LogLevel MinimumLogLevel { get; set; }
        public bool LoggingEnabled { get; set; }
        public bool LogToConsole { get; set; }
        public List<CategoryOverride> CategoryOverrides { get; set; }

        public LogConfig()
        {
            CategoryOverrides = new List<CategoryOverride>();
        }

        public static LogConfig FromScriptableObject(LogConfigSO asset)
        {
            if (asset == null)
            {
                UnityEngine.Debug.LogWarning("[LogConfig] Null asset provided, returning default config");

                return new LogConfig
                {
                    MinimumLogLevel = LogLevel.Info,
                    LoggingEnabled = true,
                    LogToConsole = true
                };
            }

            var config = new LogConfig
            {
                MinimumLogLevel = asset.MinimumLogLevel,
                LoggingEnabled = asset.LoggingEnabled,
                LogToConsole = asset.LogToConsole
            };

            // Copy category overrides (deep copy, no Unity object references)
            // This prevents destroyed reference issues when SO persists across sessions
            var categoryOverrides = asset.GetCategoryOverrides();
            if (categoryOverrides != null)
            {
                foreach (var so in categoryOverrides)
                {
                    config.CategoryOverrides.Add(new CategoryOverride
                    {
                        CategoryName = so.CategoryName,
                        MinLevel = so.MinLevel
                    });
                }
            }

            return config;
        }

        public LogConfig Clone()
        {
            var clone = new LogConfig
            {
                MinimumLogLevel = MinimumLogLevel,
                LoggingEnabled = LoggingEnabled,
                LogToConsole = LogToConsole,
                CategoryOverrides = new List<CategoryOverride>(CategoryOverrides)
            };
            return clone;
        }
    }
}
