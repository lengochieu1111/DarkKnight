using System;
using HIEU_NL.Platformer.Script.Effect;
using HIEU_NL.Platformer.Script.Game;
using NaughtyAttributes;
using UnityEngine;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Viking
{
    public class Ice_Viking : AttackEffect_Platformer
    {
        public Transform PlayerTransform => GameMode_Platformer.Instance.Player.MyTransform;
        
        [SerializeField, BoxGroup("Settings"), ReadOnly] private bool _isDead;
        [SerializeField, BoxGroup("Settings")] private float _speed = 5f;
        [SerializeField, BoxGroup("Settings")] private float _lifeTimeMax = 5f;
        private float _lifeTimer;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            //##
            if (PlayerTransform)
            {
                transform.right = (PlayerTransform.position - transform.position).normalized;
            }
            
            OnInteracted += Ice_OnInteracted;
        }

        protected override void Update()
        {
            base.Update();
            
            //##

            if (!_isDead)
            {
                if (_lifeTimer < _lifeTimeMax)
                {
                    _lifeTimer += Time.deltaTime;
                    Moving();
                }
                else
                {
                    _isDead = true;
                    Deactivate();
                }
            }

        }
        

        protected override void OnDisable()
        {
            base.OnDisable();
            
            OnInteracted -= Ice_OnInteracted;
        }

        protected override void ResetValues()
        {
            base.ResetValues();
            
            //##
            _isTracing = true;
            _isDead = false;
            _lifeTimer = 0f;
            _hitedList.Clear();
        }
        
        private void Ice_OnInteracted(object sender, EventArgs e)
        {
            if (_isDead || !_isTracing) return;
            
            _isDead = true;
            _isTracing = false;

            _animator.Play(ANIM_HASH_Dead);
            Deactivate_UniTask();
        }

        private void Moving()
        {
            transform.position += transform.right * (Time.deltaTime * _speed);
        }
        
    }
}