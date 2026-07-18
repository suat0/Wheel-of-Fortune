using System;
using System.Collections.Generic;
using UnityEngine;

namespace VertigoCase.Core.Events
{
    /// <summary>
    /// Minimal typed event bus: publishers and listeners only share the event
    /// struct, never a reference to each other. Payloads are structs so raising
    /// an event allocates nothing.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> listeners = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(Action<T> listener) where T : struct
        {
            if (listener == null)
                return;

            if (listeners.TryGetValue(typeof(T), out Delegate existing))
                listeners[typeof(T)] = (Action<T>)existing + listener;
            else
                listeners[typeof(T)] = listener;
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : struct
        {
            if (listener == null)
                return;

            if (!listeners.TryGetValue(typeof(T), out Delegate existing))
                return;

            Action<T> updated = (Action<T>)existing - listener;
            if (updated == null)
                listeners.Remove(typeof(T));
            else
                listeners[typeof(T)] = updated;
        }

        public static void Raise<T>(T payload) where T : struct
        {
            if (listeners.TryGetValue(typeof(T), out Delegate existing))
                ((Action<T>)existing).Invoke(payload);
        }

        // Static state survives play-mode sessions when domain reload is
        // disabled in the editor; clearing here guarantees no stale listeners
        // leak from a previous run.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetForNewSession()
        {
            listeners.Clear();
        }
    }
}
