using System;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Effect
{
    public class BaseEffect_Puzzle : Prefab_Puzzle
    {
        public event EventHandler OnDeactive;
        
        [SerializeField, BoxGroup("ANIMATION")]
        private bool _deactiveAfterAnimationEnd;

        [SerializeField, BoxGroup("ANIMATION"), Required("ANIMATION"), ShowIf("_deactiveAfterAnimationEnd")]
        private Animator _animator;

        [SerializeField, BoxGroup("PARTICLE SYSTEM")]
        private bool _deactiveAfterParticleSystemEnd;

        [SerializeField, BoxGroup("PARTICLE SYSTEM"), Required("ANIMATION"), ShowIf("_deactiveAfterParticleSystemEnd")]
        private ParticleSystem _particleSystem;

        private Coroutine _deactivateCoroutine;

        protected override void OnEnable()
        {
            base.OnEnable();

            //## Deactive Affter Effect End
            Deactivate_UniTask().Forget();

        }

        private async UniTask Deactivate_UniTask()
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
                    return _particleSystem.isPlaying && _particleSystem.particleCount > 0
                                                     && !_particleSystem.isStopped && _particleSystem.IsAlive();
                }

            }
        }

        public override void Deactivate()
        {
            OnDeactive?.Invoke(this, EventArgs.Empty);
            
            base.Deactivate();
        }
    }

}
