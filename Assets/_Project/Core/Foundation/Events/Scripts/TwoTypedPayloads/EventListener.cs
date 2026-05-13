using UnityEngine;
using UnityEngine.Events;

namespace Core.Foundation.Events
{

    public abstract class EventListener<T1, T2> : MonoBehaviour
    {
        [SerializeField] private EventChannelSO<T1, T2> eventChannelSo;
        [SerializeField] private UnityEvent<T1, T2> unityEvent;
        public void Raise(T1 value1, T2 value2)
        {
            unityEvent?.Invoke(value1, value2);
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
