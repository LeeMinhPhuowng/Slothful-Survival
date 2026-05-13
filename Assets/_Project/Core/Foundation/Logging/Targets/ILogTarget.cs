using System;

namespace Core.Foundation.Logging
{

    /// <summary>
    /// Abstraction for log output destinations.
    /// Allows multiple backends (console, file, remote analytics, etc.)
    ///
    /// Implementations must be thread-safe if used in multithreaded contexts.
    /// </summary>
    public interface ILogTarget
    {
        void Write(LogLevel level, string category, string message);

        void Write(LogLevel level, string category, string message, Exception exception);

        bool IsEnabled { get; }
    }
}
