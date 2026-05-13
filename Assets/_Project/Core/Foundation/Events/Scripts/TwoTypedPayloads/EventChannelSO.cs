using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Foundation.Logging;
using ILogger = Core.Foundation.Logging.ILogger;

namespace Core.Foundation.Events
{

    public abstract class EventChannelSO<T1,T2> : ScriptableObject
    {
        private static ILogger _sLazyLogger;
        private static ILogger _logger => _sLazyLogger ??= LogManager.GetLogger<EventChannelSO<T1, T2>>();
        private readonly List<EventListener<T1,T2>> eventRaised = new List<EventListener<T1,T2>>();
        public event Action<T1, T2> OnEventRaised;
        private bool _isRunning;
        
        public void EventRaise(T1 value1, T2 value2)
        {
            if (_isRunning)
            {
                _logger.Warn("RaiseEvent called while event is already running (reentrancy blocked).");
                return;
            }
            try
            {
                _isRunning = true;
                if (eventRaised.Count == 0 && OnEventRaised == null)
                {
                    _logger.Debug("EventRaise called but no listeners or subscribers.");
                    return;
                }
                if (eventRaised != null)
                {
                    List<EventListener<T1, T2>> snapshot = new List<EventListener<T1, T2>>(eventRaised);
                    foreach (var listener in snapshot)
                    {
                        try
                        {
                            listener?.Raise(value1, value2);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("RaiseEvent called but exception " + ex.Message);
                        }
                    }
                }
                var handlers = OnEventRaised;
                if (handlers != null)
                {
                    Delegate[] handlerList = handlers.GetInvocationList();
                    foreach (var handler in handlerList)
                    {
                        try
                        {
                            ((Action<T1, T2>)handler)?.Invoke(value1, value2);
                        }
                        catch (Exception e)
                        {
                            _logger.Error("RaiseEvent called but exception: " + e.Message);
                        }
                    }
                }
            }
            finally
            {
                _isRunning = false;
            }
        }

        public void AddListener(EventListener<T1, T2> listener)
        {
            if (listener == null)
            {
                _logger?.Warn("AddListener called but no listeners are registered.");
                return;
            }
            if (!eventRaised.Contains(listener))
            {
                eventRaised.Add(listener);
                _logger.Debug("Listener added: " + listener);
            }
        }

        public void RemoveListener(EventListener<T1, T2> listener)
        {
            if (listener == null)
            {
                _logger?.Warn("RemoveListener called but no listeners are registered.");
                return;
            }
            if (eventRaised.Contains(listener))
            {
                _logger.Debug("Listener removed: " + listener);
                eventRaised.Remove(listener);
            }
        }
    }
}
