using System;
using System.Collections.Generic;
using Core.Foundation.Logging;
using ILogger = Core.Foundation.Logging.ILogger;

namespace Core.Foundation.Pool
{

    /// <summary>
    /// Registry of named object pools. Plain C# class — instantiate via DI (Reflex).
    /// Implements <see cref="IDisposable"/> so the container disposes pooled wrappers on teardown.
    /// </summary>
    public sealed class PoolManager : IDisposable
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, object> _name2Pool = new();
        private readonly Dictionary<string, PoolStats> _name2Stats = new();

        public class PoolStats
        {
            public string PoolName { get; set; }
            public int TotalCreated { get; set; }
            public int MaxActive { get; set; }
            public int CurrentActive { get; set; }
            public int CurrentAvailable { get; set; }
            public long TotalGets { get; set; }
            public long TotalReleases { get; set; }
        }

        public PoolManager(LoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.GetLogger<PoolManager>();
            _logger.Info("[PoolManager] Initialized");
        }

        public void RegisterPool<T>(IPool<T> pool, string name) where T : class
        {
            _name2Pool[name] = pool;
            _name2Stats[name] = new PoolStats { PoolName = name };
            _logger.Info($"[PoolManager] Registered pool: {name}");
        }

        public T Get<T>(string poolName) where T : class
        {
            if (_name2Pool.TryGetValue(poolName, out var poolObj))
            {
                var pool = poolObj as IPool<T>;
                if (pool != null)
                {
                    _name2Stats[poolName].TotalGets++;
                    var item = pool.Get();
                    UpdateActiveStats(poolName, pool);
                    return item;
                }
                _logger.Error($"[PoolManager] Pool '{poolName}' exists but is not of type IPool<{typeof(T).Name}>!");
                return null;
            }
            _logger.Error($"[PoolManager] Pool '{poolName}' not found!");
            return null;
        }

        public void Release<T>(string poolName, T item) where T : class
        {
            if (_name2Pool.TryGetValue(poolName, out var poolObj))
            {
                var pool = poolObj as IPool<T>;
                if (pool != null)
                {
                    _name2Stats[poolName].TotalReleases++;
                    pool.Release(item);
                    UpdateActiveStats(poolName, pool);
                }
                else
                {
                    _logger.Error($"[PoolManager] Pool '{poolName}' exists but is not of type IPool<{typeof(T).Name}>!");
                }
            }
            else
            {
                _logger.Error($"[PoolManager] Pool '{poolName}' not found!");
            }
        }

        private void UpdateActiveStats<T>(string poolName, IPool<T> pool) where T : class
        {
            if (_name2Stats.TryGetValue(poolName, out var stats))
            {
                stats.CurrentActive = pool.ActiveCount;
                stats.CurrentAvailable = pool.AvailableCount;
                if (stats.CurrentActive > stats.MaxActive)
                {
                    stats.MaxActive = stats.CurrentActive;
                }
            }
        }

        public PoolStats GetStats(string poolName)
        {
            if (_name2Stats.TryGetValue(poolName, out var stats))
            {
                if (_name2Pool.TryGetValue(poolName, out var poolObj))
                {
                    var poolType = poolObj.GetType();
                    var activeCountProp = poolType.GetProperty("ActiveCount");
                    var availableCountProp = poolType.GetProperty("AvailableCount");

                    if (activeCountProp != null && availableCountProp != null)
                    {
                        stats.CurrentActive = (int)activeCountProp.GetValue(poolObj);
                        stats.CurrentAvailable = (int)availableCountProp.GetValue(poolObj);
                    }
                }
                return stats;
            }
            return null;
        }

        public Dictionary<string, PoolStats> GetAllStats()
        {
            foreach (var kvp in _name2Pool)
            {
                if (_name2Stats.TryGetValue(kvp.Key, out var stats))
                {
                    var poolType = kvp.Value.GetType();
                    var activeCountProp = poolType.GetProperty("ActiveCount");
                    var availableCountProp = poolType.GetProperty("AvailableCount");

                    if (activeCountProp != null && availableCountProp != null)
                    {
                        stats.CurrentActive = (int)activeCountProp.GetValue(kvp.Value);
                        stats.CurrentAvailable = (int)availableCountProp.GetValue(kvp.Value);
                    }
                }
            }
            return new Dictionary<string, PoolStats>(_name2Stats);
        }

        public void Dispose()
        {
            foreach (var pool in _name2Pool.Values)
            {
                if (pool is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"[PoolManager] Disposing pool failed: {ex}");
                    }
                }
            }
            _name2Pool.Clear();
            _name2Stats.Clear();
        }
    }
}
