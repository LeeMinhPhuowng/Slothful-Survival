# Universal Logging System

A high-performance, thread-safe logging framework designed for Unity projects. It provides a structured, zero-allocation architecture for managing logs across different environments (Editor, Development, Release).

## Dependencies

- **UnityEngine**: Core Unity API interaction (Debug.Log, Resources.Load).
- **System.Collections.Concurrent**: Thread-safe collections for managing logger instances.
- **System**: Lazy initialization support.

## Key Classes

### Core
- **`LogManager`**: The static entry point for the system. Use `LogManager.GetLogger<T>()` to retrieve a logger instance.
- **`Logger`**: A thread-safe logger wrapper that performs efficient log level checks before formatting messages.
- **`LoggerRegistry`**: The central singleton managing configuration, logger lifecycle, and output targets. It uses lazy initialization to ensure safety during Unity's domain reload and static initialization phases.
- **`LogConfig`**: Configuration container (ScriptableObject) defining log levels, category overrides, and global settings.

### Targets
- **`ILogTarget`**: Interface for log destinations. Allows implementing custom loggers (e.g., File, Network).
- **`UnityConsoleTarget`**: Dumps logs to the Unity Console with environment-specific formatting (Timestamped in Editor, Compact in Build).

## Usage Example

### Basic Usage
The recommended pattern is to declare a `readonly` logger field in your class. The system supports initialization in field declarators.

```csharp
using Core.Foundation.Logging;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    // Safe to initialize in field definition
    private readonly ILogger _logger = LogManager.GetLogger<MyComponent>();

    void Start()
    {
        _logger.Info("Component started successfully.");

        try
        {
            PerformCriticalAction();
        }
        catch (System.Exception ex)
        {
            _logger.Error(ex, "Critical action failed!");
        }
    }

    private void PerformCriticalAction()
    {
        if (_logger.IsDebugEnabled)
        {
            _logger.Debug($"Performing action at {Time.time}");
        }
        // ...
    }
}
```

### Configuration
1.  Create a `LogConfig` asset: Right-click in Project -> `Create/Core/Foundation/LogConfig`.
2.  Name it `LogConfig` and place it in a `Resources` folder (e.g., `Assets/_Project/Resources/LogConfig.asset`).
3.  Configure:
    *   **Minimum Log Level**: Global threshold (e.g., Info).
    *   **Category Overrides**: Set specific classes/namespaces to different levels (e.g., `Core.Network` -> Debug).

## Architecture Notes

### Safe Initialization
The system uses **Lazy Initialization**. `LoggerRegistry` does not load configuration (via `Resources.Load`) until the first log is actually written or `EnsureInitialized` is explicitly called. This prevents `ArgumentException` or `UnityException` when creating loggers in static constructors or field initializers.

### Performance
-   **Zero-Allocation Checks**: Calls like `_logger.Info(...)` check `IsInfoEnabled` internally before formatting the string or performing any other work.
-   **Structured Categories**: Loggers are cached by category name (typically the fully qualified class name) in a thread-safe dictionary.

### Extensibility
You can add custom log targets at runtime:
```csharp
LoggerRegistry.Instance.AddTarget(new FileLogTarget("application.log"));
```
