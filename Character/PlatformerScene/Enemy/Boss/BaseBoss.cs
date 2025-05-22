using System;
using HIEU_NL.Platformer.Script.Game;
using HIEU_NL.Platformer.Script.Interface;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Boss
{
    public abstract class BaseBoss : BaseEnemy
    {
        public static event EventHandler<float> OnHealthChange; // float : health percent
        public bool IsActivate = false;

        protected override void ResetValues()
        {
            base.ResetValues();
            
            IsActivate = false;
        }

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
        
        
        protected override void HandleTakeDamage(HitData hitData)
        {
            base.HandleTakeDamage(hitData);
            
            //## Health Change Event
            OnHealthChange?.Invoke(this, GetHealthPercentage());
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            //##
            PlatformerCanvas.Instance.OnBossComing += PlatformerCanvas_OnBossComing;
        }

        private void PlatformerCanvas_OnBossComing(object sender, EventArgs e)
        {
            IsActivate = true;
        }



        
    }
}

