using System;
using System.Collections.Generic;

namespace Core.Foundation.FSM
{

    /// <summary>
    /// Central registry where state machines register and unregister themselves
    /// to expose their runtime data for debugging purposes.
    /// </summary>
    public class FSMDebugRegistry
    {
        private static readonly Dictionary<Type, List<IFsmDebugView>> _contextType2DebugViews = new();

        /// <summary>
        /// Read-only access to registered FSM debug views by context type,
        /// used by editor debug tools.
        /// </summary>
        public static IReadOnlyDictionary<Type, List<IFsmDebugView>> ContextType2DebugViews => _contextType2DebugViews;

        /// <summary>
        /// Registers an FSM debug view for editor debugging.
        /// </summary>
        public static void Register(IFsmDebugView fsmDebugView)
        {
            if (!_contextType2DebugViews.TryGetValue(fsmDebugView.ContextType, out var list))
            {
                list = new List<IFsmDebugView>();
                _contextType2DebugViews.Add(fsmDebugView.ContextType, list);
            }

            list.Add(fsmDebugView);
        }
        
        /// <summary>
        /// Unregisters an FSM debug view from editor debugging.
        /// </summary>
        public static void UnRegister(IFsmDebugView fsmDebugView)
        {
            if (!_contextType2DebugViews.TryGetValue(fsmDebugView.ContextType, out var list))
            {
                return;
            }

            list.Remove(fsmDebugView);

            if (list.Count == 0)
            {
                _contextType2DebugViews.Remove(fsmDebugView.ContextType);
            }
        }
    }
}
