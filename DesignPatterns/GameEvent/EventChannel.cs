using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.DesignPatterns.GameEvent
{
    public abstract class EventChannel<T> : ScriptableObject
    {
        private readonly HashSet<EventListener<T>> _observers = new();
        
        public void Invoke(T value)
        {
            foreach (var observer in _observers)
            {
                observer.Raise(value);
            }
        }
        
        public void Register(EventListener<T> observer) => _observers.Add(observer);
        public void Deregister(EventListener<T> observer) => _observers.Remove(observer);
        
    }

    [CreateAssetMenu(fileName = "Event Channel", menuName = "Scriptable Object / Game Event / Event Channel")]
    public class EventChannel : EventChannel<Empty>
    {
        
    }
    
}