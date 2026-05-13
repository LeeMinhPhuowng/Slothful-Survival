using System;
using System.Reflection;
using UnityEngine.Events;
using Core.Foundation.Events;

namespace Core.Foundation.Tests.TestHelpers
{

    // ==========================================
    // --- Reflection Helpers ---
    // ==========================================

    /// <summary>
    /// Helper class to access private fields/methods for testing purposes.
    /// <para>
    /// <b>WARNING: HIGHLY FRAGILE CODE</b>
    /// <br/>This class relies on <b>hardcoded strings</b> to access private members via Reflection.
    /// <br/>If you rename fields or methods in the source classes (e.g., EventListener), 
    /// you <b>MUST</b> update the strings here manually, otherwise tests will fail at runtime.
    /// </para>
    /// </summary>
    public static class TestReflection
    {
        public static void SetChannel<T>(this EventListener<T> listener, EventChannelSO<T> channel)
        {
            // WARNING: "eventChannelSo" is hardcoded.
            // If the field in EventListener<T> is renamed, update this string explicitly.
            const string FIELD_NAME = "eventChannelSo";

            var field = typeof(EventListener<T>)
                .GetField(FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null) 
                throw new Exception($"[TestReflection] Field '{FIELD_NAME}' not found in {typeof(EventListener<T>).Name}. Did you rename it?");

            field.SetValue(listener, channel);
        }

        public static void AddCallback<T>(this EventListener<T> listener, UnityAction<T> action)
        {
            // WARNING: "unityEvent" is hardcoded.
            // If the field in EventListener<T> is renamed, update this string explicitly.
            const string FIELD_NAME = "unityEvent";

            var field = typeof(EventListener<T>)
                .GetField(FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null) 
                throw new Exception($"[TestReflection] Field '{FIELD_NAME}' not found in {typeof(EventListener<T>).Name}. Did you rename it?");

            var unityEvent = (UnityEvent<T>)field.GetValue(listener);
            
            if (unityEvent == null)
            {
                unityEvent = new UnityEvent<T>();
                field.SetValue(listener, unityEvent);
            }

            unityEvent.AddListener(action);
        }

        public static void InvokePrivate(this object obj, string methodName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            // WARNING: Since 'methodName' is passed as a string, automated refactoring won't catch it.
            // Ensure the method name matches exactly with the private method in the class hierarchy.
            var type = obj.GetType();
            MethodInfo method = null;

            while (type != null)
            {
                method = type.GetMethod(methodName, 
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                if (method != null)
                {
                    break;
                }
                type = type.BaseType;
            }

            if (method == null)
            {
                throw new Exception($"[TestReflection] Method '{methodName}' not found in hierarchy of {obj.GetType().Name}. Check for typos or renamed methods.");
            }

            try
            {
                method.Invoke(obj, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
    }
}
