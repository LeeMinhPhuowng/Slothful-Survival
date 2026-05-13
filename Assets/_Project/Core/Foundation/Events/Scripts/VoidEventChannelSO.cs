using System;
using Core.Foundation.Logging;
using UnityEngine;

namespace Core.Foundation.Events
{

    [CreateAssetMenu(fileName = "SO_VoidEventChannel", menuName = "EventChannelSO/VoidEventChannelSO")]
    public class VoidEventChannelSO : ScriptableObject
    {
        
        //-----Reentrancy Safe-----
        private bool _isRunning;
        
        //-----Event-----
        public delegate void OnHandler();
        private event OnHandler EventRaised;
        public event Action OnEventRaised;

        private static Logging.ILogger _sLazyLogger;
        private static Logging.ILogger _logger => _sLazyLogger ??= LogManager.GetLogger<VoidEventChannelSO>();

        public void EventRaise()
        {
            if (_isRunning)
            {
                _logger?.Warn("RaiseEvent called while event is already running (reentrancy blocked).");
                return;
            }
            try
            {
                _isRunning = true;
                if (EventRaised != null)
                {
                    Delegate[] listener = EventRaised.GetInvocationList();
                    foreach (var lis in listener)
                    {
                        try
                        {
                            ((OnHandler)lis)?.Invoke();
                        }
                        catch (Exception e)
                        {
                            _logger?.Error("RaiseEvent called but exception: " + e.Message);
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
                            ((Action)handler)?.Invoke();
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

        public void AddListener(OnHandler listener)
        {
            if (listener == null)
            {
                _logger?.Warn("AddListener called with null listener argument.");
                return;
            }
            EventRaised += listener;
            _logger?.Debug("Listener added.");
        }

        public void RemoveListener(OnHandler listener)
        {
            if (listener == null)
            {
                _logger?.Warn("RemoveListener called with null listener argument.");
                return;
            }
            EventRaised -= listener;
            _logger?.Debug("Listener removed.");
        }
        
    }
}
