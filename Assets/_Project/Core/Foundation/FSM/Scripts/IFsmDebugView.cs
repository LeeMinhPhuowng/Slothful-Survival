using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Foundation.FSM
{

    /// <summary>
    /// Provides editor-facing debug data of a state machine.
    /// Implemented by components that expose FSM state information
    /// for editor debug tools and visualizers.
    /// </summary>
    public interface IFsmDebugView
    {   
        /// <summary>
        /// The GameObject that owns the state machine.
        /// Used by the editor to identify and select the FSM instance.
        /// </summary>
        GameObject Owner { get; }

        /// <summary>
        /// The context type associated with the state machine.
        /// Used to group FSMs in editor debug views.
        /// </summary>
        Type ContextType { get; }

        /// <summary>
        /// Debug representation of the state machine history.
        /// </summary>
        IReadOnlyCollection<Type> History { get; }

        /// <summary>
        /// Debug representation of available or recent state transitions.
        /// </summary>
        string Transitions { get; }
    }
}
