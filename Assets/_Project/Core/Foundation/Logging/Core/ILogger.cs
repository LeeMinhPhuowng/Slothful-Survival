using System;

namespace Core.Foundation.Logging
{

    /// <summary>
    /// Standard interface for logging, inspired by Microsoft.Extensions.Logging and Log4j.
    /// </summary>
    public interface ILogger
    {
        string Name { get; }

        bool IsTraceEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }

        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);

        void Error(Exception exception, string message);
        void Fatal(Exception exception, string message);
    }
}
