using System;
using NaughtyAttributes;
using System.Collections.Generic;
using HIEU_NL.Manager;
using HIEU_NL.ObjectPool.Audio;
using HIEU_NL.Platformer.Script.Entity;
using HIEU_NL.Platformer.Script.Interface;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HIEU_NL.Platformer.Script.Effect
{
    public class AttackEffect_Platformer : BaseEffect_Platformer, IAttackable
    {
        public event EventHandler OnInteracted;
        
        [SerializeField, BoxGroup("HIT")] private PrefabType_Platformer _hitEffectType;
        [SerializeField, BoxGroup("HIT")] private SoundType _attackSoundType;
        [SerializeField, BoxGroup("HIT")] private SoundType _hitSoundType;
        [SerializeField, BoxGroup("HIT")] private bool _isRandomRotation;
        [SerializeField, BoxGroup("HIT"), MinMaxSlider(-25f, 25f)] private Vector2 _randomRotationRange;

        
        [SerializeField, Required] private BoxCollider2D _hitCollider;
        [SerializeField] private LayerMask _hitLayer;
        protected bool _isTracing;
        protected List<HittableObject> _hitedList = new List<HittableObject>();
        private HitData _hitData;

        protected override void ResetValues()
        {
            base.ResetValues();

            _isTracing = false;

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireCube(_hitCollider.bounds.center, _hitCollider.bounds.size);

        }

        protected virtual void Update()
        {
            if (_isTracing)
            {
                ITracing();
            }

        }

        #region IATTACKABLE

        public void IBeginAttack()
        {
        }

        public void IAttacking()
        {
        }

        public void IFinishAttack()
        {
        }

        public void IBeginTrace()
        {
            _isTracing = true;
            _hitedList.Clear();
        }

        public void ITracing()
        {
            RaycastHit2D[] hitArray = Physics2D.BoxCastAll(_hitCollider.bounds.center, _hitCollider.bounds.size, 0f,
                Vector2.right, 0f, _hitLayer);

            foreach (RaycastHit2D hit in hitArray)
            {
                if (hit.transform.TryGetComponent(out HittableObject hittableObject) &&
                    !_hitedList.Contains(hittableObject))
                {
                    bool hitSuccess = hittableObject.IHit(_hitData);
                    if (hitSuccess)
                    {
                        _hitedList.Add(hittableObject);

                        //## Play audio
                        if (_hitSoundType is not SoundType.NONE)
                        {
                            ((SoundManager)SoundManager.Instance).PlaySound(_hitSoundType);
                        }

                        //## Play Effect
                        if (_hitEffectType is not PrefabType_Platformer.NONE)
                        {
                            Transform hitTransform = hittableObject.transform;
                            if (hit.transform.TryGetComponent(out BaseEntity entity))
                            {
                                hitTransform = entity.CenterOfBodyTransform;
                            }

                            Quaternion rotation = _isRandomRotation
                                ? Quaternion.Euler(0f, 0f, Random.Range(_randomRotationRange.x, _randomRotationRange.y))
                                : Quaternion.identity;
                            Prefab_Platformer hitPrefab = ObjectPool_Platformer.Instance.GetPoolObject(_hitEffectType,
                                rotation: rotation, parent: hitTransform);
                            hitPrefab?.Activate();
                        }
                    }

                    HandleInteracted(hit.transform);
                    
                    OnInteracted?.Invoke(this, EventArgs.Empty);

                }
            }
        }

        public void IFinishTrace()
        {
            _isTracing = false;
        }

        #endregion

        //# ANIMATION EVENT FUNCTION

        private void AE_BeginTrace()
        {
            IBeginTrace();
            
            //## Play audio
            if (_attackSoundType is not SoundType.NONE)
            {
                ((SoundManager)SoundManager.Instance).PlaySound(_attackSoundType);
            }
            
        }

        private void AE_FinishTrace()
        {
            IFinishTrace();
        }
        
        //#

        public void Setup(HitData hitData)
        {
            _hitData = hitData;
        }

        protected virtual void HandleInteracted(Transform hitTransform)
        {
            
        }


    }

}

