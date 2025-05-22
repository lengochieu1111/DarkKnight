using HIEU_NL.Platformer.Script.Entity;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Interface
{
    public interface IHittable
    {
        public abstract bool IHit(HitData hitData);
    }
    
    public class HitData
    {
        public Prefab_Platformer DamageCauser { get; private set; }
        public Vector3 AttackDirection { get; private set; }
        public int Damage { get; private set; }
        public bool IsCausedByPlayer { get; private set; }

        public HitData(Prefab_Platformer damageCauser = default, Vector3 attackDirection = default, int damage = 10,
            bool isCausedByPlayer = true)
        {
            DamageCauser = damageCauser;
            AttackDirection = attackDirection;
            Damage = damage;
            IsCausedByPlayer = isCausedByPlayer;
        }

    }
    
}

