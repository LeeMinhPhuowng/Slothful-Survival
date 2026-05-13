using Core.Foundation.Logging;
using System;
using System.Collections.Generic;

namespace Core.Foundation.Events
{

    public class EventBus : IDisposable
    {
        // Dictionary contain Event
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        // Lock for Thread-safety
        private readonly object _lock = new object();

        private readonly ILogger _logger;
        public EventBus(ILogger logger)
        {
            _logger = logger;
            _logger.Info("EventBus initialized.");
        }

        public void Subscribe<T>(Action<T> handler)
        {
            lock (_lock)
            {
                var type = typeof(T);
                if (!_subscribers.ContainsKey(type))
                {
                    _subscribers[type] = new List<Delegate>();
                }

                _subscribers[type].Add(handler);
                _logger.Info($"[EventBus] Subscribed: {handler.Method.Name} to {type.Name}");
            }
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            lock (_lock)
            {
                var type = typeof(T);
                if (_subscribers.TryGetValue(type, out var handlers))
                {
                    if (handlers.Remove(handler))
                    {
                        _logger.Info($"[EventBus] Unsubscribed: {handler.Method.Name} from {type.Name}");
                    }
                }
            }
        }

        public void Publish<T>(T eventData)
        {
            List<Delegate> handlersCopy;
            var type = typeof(T);

            lock (_lock)
            {
                if (!_subscribers.TryGetValue(type, out var handlers) || handlers.Count == 0)
                {
                    return;
                }

                handlersCopy = new List<Delegate>(handlers);
            }

            _logger.Info($"[EventBus] Publishing: {type.Name}");

            foreach (var handler in handlersCopy)
            {
                try
                {
                    (handler as Action<T>)?.Invoke(eventData);
                }
                catch (Exception ex)
                {
                    _logger.Error($"[EventBus] Error invoking {handler.Method.Name}: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _subscribers.Clear();
                _logger.Info("[EventBus] Disposed and cleared all subscribers.");
            }
        }
    }
}
