using System;
using HIEU_NL.Platformer.Script.Interface;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Boss
{
    public abstract class BaseBoss : BaseEnemy
    {
        public static event EventHandler<float> OnHealthChange; // float : health percent
        public static event EventHandler<float> OnEnergyChange; // float : energy percent
        
        //# ENERGY
        [SerializeField, BoxGroup("ENERGY")] protected int energy = 100;
        [SerializeField, BoxGroup("ENERGY")] protected int maxEnergy = 100;
        public int Energy => energy;
        public int MaxEnergy => maxEnergy;
        
        
        /*#region CALL EVENT ACTION
        
        protected virtual void CallEvent_OnHealthChange()
        {
            OnHealthChange?.Invoke(this, GetHealthPercentage());
        }
        
        protected virtual void CallEvent_OnEnergyChange()
        {
            OnHealthChange?.Invoke(this, GetEnergyPercentage());
        }
        
        #endregion*/
        
        public float GetEnergyPercentage() { return energy * 1f / maxEnergy; }
        
        #region INTERFACE : DAMAGEABLE
        
        public override bool ITakeDamage(HitData hitData)
        {
            bool result = base.ITakeDamage(hitData);

            if (!result) return false;

            //## Health Change Event
            OnHealthChange?.Invoke(this, GetHealthPercentage());
            
            return true;
            
        }
        
        #endregion

        
    }
}

