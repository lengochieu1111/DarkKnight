using System;
using UnityEngine;

namespace HIEU_NL.DesignPatterns.InterfaceAttribute
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public readonly Type InterfaceType;

        public RequireInterfaceAttribute(Type interfaceType)
        {
            Debug.Assert(interfaceType.IsInterface, $"{nameof(interfaceType)} needs to be an interfaceType.");
            this.InterfaceType = interfaceType;
        }
        
    }
}