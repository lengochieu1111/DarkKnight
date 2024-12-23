using UnityEngine;
using System;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace HIEU_NL.DesignPatterns.InterfaceAttribute
{
    [Serializable]
    public class InterfaceReference<TInterface, TObject>where TObject : Object where TInterface : class
    {
        [SerializeField] private TObject _underlyingValue;

        public TInterface Value
        {
            get => _underlyingValue switch
            {
                null => null,
                TInterface @interface => @interface,
                _ => throw new InvalidOperationException($"{_underlyingValue} needs to implement interface {nameof(TInterface)}.")
            };
            
            set => _underlyingValue = value switch
            {
                null => null,
                TObject newValue => newValue,
                _ => throw new ArgumentException($"{value} needs to be of type {typeof(TObject).Name}.", string.Empty)
            };
        }

        public TObject UnderlyingValue
        {
            get => _underlyingValue;
            set => _underlyingValue = value;
        }
        
        public InterfaceReference() { }
        
        public InterfaceReference(TObject target) => _underlyingValue = target;
        
        public InterfaceReference(TInterface @interface) => _underlyingValue = @interface as TObject;
        
    }
    
    /// <summary>
    /// 
    /// </summary>

    [Serializable]
    public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class { }
    
}