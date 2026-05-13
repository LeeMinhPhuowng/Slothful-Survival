using System;
using UnityEngine;

namespace Core.Foundation.Logging
{

    /// <summary>
    /// Default log target that writes to Unity Console.
    /// Zero-allocation wrapper around UnityEngine.Debug.
    ///
    /// Format:
    /// - Editor: [HH:mm:ss.fff] [LEVEL] [Category] Message
    /// - Build:  [L] [Category] Message  (compact to reduce string allocation)
    /// </summary>
    public class UnityConsoleTarget : ILogTarget
    {
        private readonly bool _includeTimestamp;

        public bool IsEnabled => true;

        public UnityConsoleTarget(bool includeTimestamp = true)
        {
            _includeTimestamp = includeTimestamp;
        }

        public void Write(LogLevel level, string category, string message)
        {
            var formatted = FormatMessage(level, category, message);
            WriteToUnity(level, formatted);
        }

        public void Write(LogLevel level, string category, string message, Exception exception)
        {
            var formatted = FormatMessage(level, category, $"{message}\n{exception}");
            WriteToUnity(level, formatted);
        }

        private string FormatMessage(LogLevel level, string category, string message)
        {
    #if UNITY_EDITOR
            // Verbose format in editor for debugging
            if (_includeTimestamp)
            {
                return $"[{DateTime.UtcNow:HH:mm:ss.fff}] [{GetLevelString(level)}] [{category}] {message}";
            }

            return $"[{GetLevelString(level)}] [{category}] {message}";
    #else
            // Compact format in builds to reduce GC
            return $"[{GetLevelChar(level)}] [{category}] {message}";
    #endif
        }

        private void WriteToUnity(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Error:
                case LogLevel.Fatal:
                    Debug.LogError(message);
                    break;
                case LogLevel.Warn:
                    Debug.LogWarning(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }

        private string GetLevelString(LogLevel level)
        {
            return level.ToString().ToUpper();
        }

        private char GetLevelChar(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace: return 'T';
                case LogLevel.Debug: return 'D';
                case LogLevel.Info: return 'I';
                case LogLevel.Warn: return 'W';
                case LogLevel.Error: return 'E';
                case LogLevel.Fatal: return 'F';
                default: return '?';
            }
        }
    }
}
