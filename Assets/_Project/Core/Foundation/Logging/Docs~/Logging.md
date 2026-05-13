# Logging System - Usage Guide

## Purpose

This document describes how to configure and use the Foundation logging system, including runtime configuration, DI binding, lifecycle management, and expected usage patterns.

---

## Core Components

| Component         | Responsibility                            |
| ----------------- | ----------------------------------------- |
| `ILogger`         | Logging abstraction (implements IDisposable) |
| `UnityLogger`     | Unity Console logger implementation       |
| `LogConfigSO`     | ScriptableObject storing logging settings |
| `LogBootstrapper` | Initializes runtime logging config        |
| `LogConfigEditor` | Editor UI for adjusting log settings      |

---

## Configuration Asset

A `LogConfig` asset must exist under:

```
Assets/Resources/LogConfig.asset
```

If missing, the Editor tool can create it automatically.

This asset defines:

* Minimum log level (Debug, Info, Warning, Error, None)
* Global logging enable/disable
* Console output toggle

---

## Runtime Initialization

At startup, `LogBootstrapper`:

1. Loads `LogConfig` from Resources
2. Creates a **runtime clone** (`ActiveConfig`)
3. Applies environment defaults:
    * **Editor Mode** → Uses asset settings as-is (no override)
    * **Debug Build** (Standalone) → Debug level enabled
    * **Release Build** → Warning+ level enabled
4. Notifies subscribed loggers via `OnConfigChanged` event

This prevents modifying on-disk assets at runtime.

**Important:** The runtime config is applied only to standalone builds. In Editor, you control logging via the asset settings directly.

---

## Logger Lifecycle Management

**Critical:** All loggers implement `IDisposable` and **must be disposed** to prevent event subscription leaks.

### Pattern 1: MonoBehaviour (Standard)
```csharp
public class GameController : MonoBehaviour
{
    private ILogger _logger;

    void Awake()
    {
        _logger = new UnityLogger(LogBootstrapper.ActiveConfig);
    }

    void OnDestroy()
    {
        _logger?.Dispose(); // Required!
    }
}
```

### Pattern 2: Dependency Injection (Testable)
```csharp
public class DataService : IDisposable
{
    private readonly ILogger _logger;
    private readonly bool _ownsLogger;

    public DataService(ILogger logger = null)
    {
        if (logger != null)
        {
            _logger = logger;
            _ownsLogger = false; // Injected - caller owns
        }
        else
        {
            _logger = new UnityLogger(LogBootstrapper.ActiveConfig);
            _ownsLogger = true; // Created - we own
        }
    }

    public void Dispose()
    {
        if (_ownsLogger)
        {
            _logger?.Dispose();
        }
    }
}
```

### Pattern 3: Short-Lived (Temporary)
```csharp
public void EditorUtility()
{
    using (var logger = new UnityLogger(LogBootstrapper.ActiveConfig))
    {
        logger.LogInfo("Task started");
        // ... work ...
    } // Auto-disposed
}
```

---

## Dependency Injection (Reflex Example)

Bind the runtime config and logger factory:

```csharp
public class LoggingInstaller : MonoInstaller
{
    public override void InstallBindings(ContainerBuilder builder)
    {
        var config = LogBootstrapper.ActiveConfig;
        
        builder.AddSingleton(config);
        builder.AddSingleton<ILogger>(container => 
            new UnityLogger(container.Resolve<LogConfigSO>()));
    }
}
```

**Important:** When using DI, ensure the container manages logger disposal, or use the ownership pattern (Pattern 2).

Consumers should depend on `ILogger`, not concrete logger types.

---

## Logging Usage Examples

### Basic Usage
```csharp
public class ExampleSystem
{
    private readonly ILogger _logger;

    public ExampleSystem(ILogger logger)
    {
        _logger = logger;
    }

    public void Initialize()
    {
        _logger.LogInfo("System initialized");
        _logger.LogDebug("Debug details");
        _logger.LogWarning("Fallback mode enabled");
        _logger.LogError("Critical failure");
    }
}
```

### Performance-Aware Logging
```csharp
public class PathfindingSystem
{
    private readonly ILogger _logger;
    private readonly LogConfigSO _config;

    public PathfindingSystem(ILogger logger, LogConfigSO config)
    {
        _logger = logger;
        _config = config;
    }

    public void FindPath()
    {
        // Only compute expensive data if Debug is enabled
        if (_config.ShouldLog(LogLevel.Debug))
        {
            string details = ComputeExpensivePathData();
            _logger.LogDebug(details);
        }
        
        _logger.LogInfo("Path found");
    }

    private string ComputeExpensivePathData() => "...";
}
```

---

## Runtime Configuration Editing

Logging settings can be adjusted at runtime via:

* **LogConfig Inspector** (select the asset)
* **Log Control Editor Window** (`Window > Core > Log Control`)

Changes trigger `OnConfigChanged` event, which automatically updates all subscribed loggers' cached values.

### Quick Profiles Available:
* **Development Mode** → Debug level, all enabled
* **Production Mode** → Warning level, selective output
* **Mute All** → Disable all logging

---

## Thread Safety Contract

* Loggers **must not read ScriptableObject fields directly** from background threads
* `UnityLogger` caches primitive config values (`_cachedMinLevel`, `_cachedLoggingEnabled`, `_cachedLogToConsole`)
* Config changes refresh cache via `OnConfigChanged` event
* Logging calls are thread-safe as long as they rely on cached values
* **Do not** call Unity API methods (Debug.Log) from background threads

---

## Recommended Usage Rules

| Rule                                           | Reason                    |
| ---------------------------------------------- | ------------------------- |
| Always dispose loggers                         | Prevent event leaks       |
| Depend on `ILogger` only                       | Avoid coupling            |
| Check `ShouldLog()` before expensive operations| Performance               |
| Avoid logging in tight loops (Update/FixedUpdate)| Prevent spam           |
| Use Debug level only for development           | Reduce production noise   |
| Use ownership pattern with DI                  | Clear disposal responsibility |

---

## Common Mistakes

| Mistake | Solution |
|---------|----------|
| Forgetting to dispose logger | Always dispose in `OnDestroy()` or use `using` |
| Disposing injected logger | Only dispose if you created it (ownership pattern) |
| Computing expensive data unconditionally | Check `ShouldLog()` first |
| Reading config fields from background threads | Use cached values only |
| Creating logger per frame | Reuse logger as field/singleton |

---

## Scope & Limitations
This logging layer **does not** provide:
* File logging (use external solution)
* Remote logging / crash analytics
* Log persistence
* elemetry pipelines
* Log aggregation