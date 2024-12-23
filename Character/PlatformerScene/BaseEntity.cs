using System;
using HIEU_NL.DesignPatterns.ObjectPool.Multiple;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Interface;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace HIEU_NL.Platformer.Script.Entity
{
    public abstract class BaseEntity : PoolPrefab, IDamageable
    {
        public event EventHandler<HitData> OnTakeDamage;
        public event EventHandler OnDead;

        [SerializeField, BoxGroup("BODY"), Required] protected Transform centerOfBodyTransform;
        
        [SerializeField, BoxGroup("HEALTH")] protected float health = 100f;
        [SerializeField, BoxGroup("HEALTH")] protected float maxHealth = 100f;

        protected override void ResetValues()
        {
            base.ResetValues();

            health = maxHealth;
        }

        #region IDAMAGEABLE

        public virtual bool ICanTakeDamage()
        {
            return health > 0f;
        }

        public virtual bool ITakeDamage(HitData hitData)
        {
            if (!ICanTakeDamage()) return false;
            
            health = Mathf.Max(0, health - hitData.Damage);

            PoolPrefab bloodPool = MultipleObjectPool.Instance.GetPoolObject(PoolPrefabType.Blood_1_PLATFORMER, parent:centerOfBodyTransform);
            bloodPool.Activate();
            
            //##
            OnTakeDamage?.Invoke(this, hitData);

            if (health <= 0)
            {
                OnDead?.Invoke(this, EventArgs.Empty);
            }

            return true;
        }

        #endregion




    }
}

