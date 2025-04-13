using System;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Beetle
{
    public class Projectile_Beetle : Prefab_Platformer
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _lifeTime = 5f;
        private float _lifeTimer;

        protected override void ResetValues()
        {
            base.ResetValues();

            _lifeTimer = 0f;
        }

        private void Update()
        {
            if (_lifeTimer < _lifeTime)
            {
                _lifeTimer += Time.deltaTime;
                
                Moving();
            }
            else
            {
                Deactivate();
            }
        }

        private void Moving()
        {
            transform.position += transform.right * (_moveSpeed * Time.deltaTime);
        }

        public void Activate(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.right = direction;
            
            //##
            Activate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Deactivate();
        }
        
    }
}