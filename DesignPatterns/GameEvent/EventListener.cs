using System;
using UnityEngine;
using UnityEngine.Events;

namespace HIEU_NL.DesignPatterns.GameEvent
{
    public abstract class EventListener<T> : RyoMonoBehaviour
    {
        [SerializeField] private EventChannel<T> _eventChannel;
        [SerializeField] private UnityEvent<T> _unityEvent;

        #region UNITY

        protected override void Awake()
        {
            _eventChannel.Register(this);
        }

        protected override void OnDestroy()
        {
            _eventChannel.Deregister(this);
        }

        #endregion

        public void Raise(T value)
        {
            _unityEvent?.Invoke(value);
        }
        
    }
    
    public class EventListener : EventListener<Empty> {}
}