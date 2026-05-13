using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Foundation.Events
{

    public abstract class EventListener<T> : MonoBehaviour
    {
        [SerializeField] private EventChannelSO<T> eventChannelSo;
        [SerializeField] private UnityEvent<T> unityEvent;

        public void Raise(T value)
        {
            unityEvent?.Invoke(value);
        }

        private void OnEnable()
        {
            if (eventChannelSo == null) return;
            eventChannelSo.AddListener(this);
        }

        private void OnDisable()
        {
            if (eventChannelSo == null) return;
            eventChannelSo.RemoveListener(this);
        }
    }
}
