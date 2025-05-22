using System;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Effect
{
    public class BaseEffect_Platformer : Prefab_Platformer
    {
        [SerializeField, BoxGroup("ACTIVATE")]
        protected bool _autoDeactivate = true;
        
        [SerializeField, BoxGroup("ANIMATION")]
        protected bool _deactiveAfterAnimationEnd;

        [SerializeField, BoxGroup("ANIMATION"), Required("ANIMATION"), ShowIf("_deactiveAfterAnimationEnd")]
        protected Animator _animator;

        [SerializeField, BoxGroup("PARTICLE SYSTEM")]
        protected bool _deactiveAfterParticleSystemEnd;

        [SerializeField, BoxGroup("PARTICLE SYSTEM"), Required("ANIMATION"), ShowIf("_deactiveAfterParticleSystemEnd")]
        protected ParticleSystem _particleSystem;

        protected Coroutine _deactivateCoroutine;

        protected override void OnEnable()
        {
            base.OnEnable();

            //## Deactive Affter Effect End

            if (_autoDeactivate)
            {
                Deactivate_UniTask().Forget();
            }

        }

        protected async UniTask Deactivate_UniTask()
        {
            if (_deactiveAfterAnimationEnd && _animator != null)
            {
                float deactivateTime = _animator.GetCurrentAnimatorStateInfo(0).length;
                await UniTask.WaitForSeconds(deactivateTime);

                Deactivate();

            }
            else if (_deactiveAfterParticleSystemEnd && _particleSystem != null)
            {
                await UniTask.WaitUntil(() => ParticleSystemIsActive());

                await UniTask.WaitUntil(() => !ParticleSystemIsActive());

                Deactivate();

                //##
                bool ParticleSystemIsActive()
                {
                    try
                    {
                        return _particleSystem.isPlaying && _particleSystem.particleCount > 0
                                                         && !_particleSystem.isStopped && _particleSystem.IsAlive();
                    }
                    catch (Exception e)
                    {
                    }

                    return false;

                }

            }
        }

    }

}
