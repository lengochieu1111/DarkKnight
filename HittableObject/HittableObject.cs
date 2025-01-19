using System.Collections;
using System.Collections.Generic;
using HIEU_NL.DesignPatterns.InterfaceAttribute;
using HIEU_NL.Platformer.Script.Interface;
using UnityEngine;

public class HittableObject : RyoMonoBehaviour, IHittable
{
    [SerializeField] private InterfaceReference<IDamageable> _damageable;

    public bool IHit(HitData hitData)
    {
        if (_damageable == null || _damageable.Value == null) return false;
        return _damageable.Value.ITakeDamage(hitData);
    }
    
}
