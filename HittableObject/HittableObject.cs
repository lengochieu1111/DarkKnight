using System.Collections;
using System.Collections.Generic;
using HIEU_NL.DesignPatterns.InterfaceAttribute;
using HIEU_NL.Platformer.Script.Entity;
using HIEU_NL.Platformer.Script.Interface;
using NaughtyAttributes;
using UnityEngine;

public class HittableObject : RyoMonoBehaviour, IHittable
{
    public InterfaceReference<IDamageable> _damageable;

    public void IHit(HitData hitData)
    {
        _damageable?.Value.ITakeDamage(hitData);
    }
    
}
