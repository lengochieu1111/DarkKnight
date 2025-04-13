using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HIEU_NL.Manager;
using HIEU_NL.ObjectPool.Audio;
using HIEU_NL.Puzzle.Script.Entity.Character;
using HIEU_NL.Puzzle.Script.Entity.Player;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Enemy
{
    public class Enemy_Puzzle : DynamicEntity_Puzzle
    {

        private int AnimTrigger_Pain = Animator.StringToHash("Pain");
        private int AnimTrigger_Dead = Animator.StringToHash("Dead");
        
        private int AnimStateHash_Dead = Animator.StringToHash("Dead");
        
        [SerializeField, BoxGroup("ANIMATOR")] private Animator _animator;
        
        [SerializeField, BoxGroup("COLLIDER")] private Collider2D _collider;

        protected override void HandleMove()
        {
            base.HandleMove();
            
            PlayBigImpactEffect();
        }

        protected override void HandleCannotMove()
        {
            base.HandleCannotMove();

            HandleDead();
        }

        public override void ReceiverInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
        {
            base.ReceiverInteract(senderEntity, receverDirection);

            if (senderEntity is Player_Puzzle)
            {
                RequestAction(receverDirection);
                PlayEnemyKickSound();
                _animator.SetTrigger(AnimTrigger_Pain);
            }
        }

        protected override void HandlePain()
        {
            base.HandlePain();
            
            HandleDead();
        }
        
        protected override void HandleDead()
        {
            base.HandleDead();
            
            PlayBigImpactEffect();
            PlayEnemyDeadSound();
            
            _collider.enabled = false;
            _animator.SetTrigger(AnimTrigger_Dead);
            UT_Dead().Forget();
        }

        private async UniTask UT_Dead()
        {
            await UniTask.WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(AnimStateHash_Dead));
            float deadAnimTime = _animator.GetCurrentAnimatorStateInfo(0).length;
            await UniTask.WaitForSeconds(deadAnimTime);
            DestroySelf();
        }
        
        private void PlayEnemyKickSound()
        {
            SoundType soundType = SoundType.Enemy_Kick_1;
            int indexRandom = UnityEngine.Random.Range(0, 3);
            switch (indexRandom)
            {
                case 0:
                    soundType = SoundType.Enemy_Kick_1;
                    break;
                case 1:
                    soundType = SoundType.Enemy_Kick_2;
                    break;
                case 2:
                    soundType = SoundType.Enemy_Kick_3;
                    break;
            }
            ((SoundManager)SoundManager.Instance).PlaySound(soundType);
        }
        
        private void PlayEnemyDeadSound()
        {
            SoundType soundType = SoundType.Enemy_Dead_1;
            int indexRandom = UnityEngine.Random.Range(0, 3);
            switch (indexRandom)
            {
                case 0:
                    soundType = SoundType.Enemy_Dead_1;
                    break;
                case 1:
                    soundType = SoundType.Enemy_Dead_2;
                    break;
                case 2:
                    soundType = SoundType.Enemy_Dead_3;
                    break;
            }
            ((SoundManager)SoundManager.Instance).PlaySound(soundType);
        }

    }

}
