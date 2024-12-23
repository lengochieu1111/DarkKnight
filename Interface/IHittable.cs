using HIEU_NL.Platformer.Script.Entity;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Interface
{
    public interface IHittable
    {
        public abstract void IHit(HitData hitData);
    }
    
    public class HitData
    {
        public BaseEntity DamageCauser { get; private set; }
        public Vector3 AttackDirection { get; private set; }
        public float Damage { get; private set; }
        public bool IsCausedByPlayer { get; private set; }

        public HitData(Entity.BaseEntity damageCauser = default, Vector3 attackDirection = default, float damage = 10f,
            bool isCausedByPlayer = true)
        {
            DamageCauser = damageCauser;
            AttackDirection = attackDirection;
            Damage = damage;
            IsCausedByPlayer = isCausedByPlayer;
        }

    }
    
}

