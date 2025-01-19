using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Golem_2
{
    public class Golem_2_State
    {
        /// <summary>
        /// IDLE STATE
        /// </summary>
        public class IdleState : BaseEnemyState.IdleState
        {
            private Coroutine _idleCoroutine;
    
            public IdleState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Idle, transitionAnimDuration);
    
                _idleCoroutine = owner.StartCoroutine(Idle_Coroutine());
    
            }
    
            public override void OnExit()
            {
                owner.StopCoroutine(_idleCoroutine);
                _idleCoroutine = null;
                
                base.OnExit();
            }
            
            //#
            
            private IEnumerator Idle_Coroutine()
            {
                yield return new WaitForSeconds(owner.Stats.IdleTimeMax);
                owner.Finish_IdleState();
            }
    
        }
        
        /// <summary>
        /// PATROL STATE
        /// </summary>
        public class PatrolState : BaseEnemyState.PatrolState
        {
            private Vector2 _patrolDirection;
            private float _patrolSpeed;
    
            public PatrolState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
    
                if (HasReachToDestination())
                {
                    owner.SetIsFlippingLeft(!owner.IsFlippingLeft);
                }
    
                _patrolSpeed = 0f;
                _patrolDirection = owner.IsFlippingLeft ? Vector2.left : Vector2.right;
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Walk, transitionAnimDuration);
    
            }
    
            public override void Update()
            {
                if (HasReachToDestination())
                {
                    HandleReachToDestionation();
                }
                else
                {
                    _patrolSpeed = Mathf.Lerp(_patrolSpeed, owner.Stats.PatrolSpeed, owner.Stats.PatrolSpeed * Time.deltaTime);
                    owner.MyTransform.position += (Vector3)_patrolDirection * (_patrolSpeed * Time.deltaTime);
                }
    
            }
    
            //# 
    
            private void HandleReachToDestionation()
            {
                owner.Finish_PatrolState();
            }
    
            private bool HasReachToDestination()
            {
                return owner.IsFlippingLeft && owner.MyTransform.position.x <= owner.LeftPatrolPosition.x
                    || !owner.IsFlippingLeft && owner.MyTransform.position.x >= owner.RightPatrolPosition.x
                    || owner.IsTouchingWall;
            }
    
        }
        
        /// <summary>
        /// CHASE STATE
        /// </summary>
        public class ChaseState : BaseEnemyState.ChaseState
        {
            private Golem_2 _owner_golem_2;
            private Vector2 _chaseDirection;
            private float _chaseSpeed;
            
            private float _appearDistance = 1f;
            private bool _isTeleporting;
            private CancellationTokenSource _cancellationTokenSource;
    
            public ChaseState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
                
                _chaseSpeed = 0f;
                _isTeleporting = false;
                _owner_golem_2 = owner as Golem_2;
                
                if (_owner_golem_2 != null && _owner_golem_2.GetHealthPercentage() < _owner_golem_2.TeleportHealthPercentage)
                {
                    _isTeleporting = true;
                }
                
                if (_isTeleporting)
                {
                    if (LookingTowardThePlayer())
                    {
                        owner.SetIsFlippingLeft(!owner.IsFlippingLeft);
                    }
                    
                    _cancellationTokenSource = new CancellationTokenSource();
                    Teleport_UniTask().SuppressCancellationThrow().Forget();
                    
                }
                else
                {
                    animator.CrossFadeInFixedTime(ANIM_HASH_Walk, transitionAnimDuration);
                }
    
            }
    
            public override void Update()
            {
                if (_isTeleporting) return;
                
                if (owner.PlayerInChaseRange())
                {
                    if (LookingTowardThePlayer())
                    {
                        owner.SetIsFlippingLeft(!owner.IsFlippingLeft);
                    }

                    _chaseSpeed = Mathf.Lerp(_chaseSpeed, owner.Stats.ChaseSpeed, owner.Stats.ChaseSpeed * Time.deltaTime);
                    _chaseDirection = (owner.TargetTransform.position - owner.MyTransform.position).normalized;

                    owner.MyTransform.position += (Vector3)_chaseDirection * (_chaseSpeed * Time.deltaTime);
                }
                else
                {
                    owner.Finish_ChaseState();
                }
            }

            public override void OnExit()
            {
                _cancellationTokenSource?.Cancel();
                // _cancellationTokenSource?.Dispose();
                
                base.OnExit();
            }

            //#
            
            private async UniTask Teleport_UniTask()
            {
                animator.CrossFadeInFixedTime(ANIM_HASH_TeleportStart, transitionAnimDuration);

                await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(ANIM_HASH_TeleportStart), 
                    cancellationToken:_cancellationTokenSource.Token);
                float teleportStartTime = animator.GetCurrentAnimatorStateInfo(0).length;
                await UniTask.WaitForSeconds(teleportStartTime, cancellationToken:_cancellationTokenSource.Token);
                
                _chaseDirection = (owner.MyTransform.position - owner.TargetTransform.position).normalized;
                Vector2 appearPosition = (Vector2)owner.TargetTransform.position + _chaseDirection * _appearDistance;
                owner.MyTransform.position = appearPosition;
                
                animator.CrossFadeInFixedTime(ANIM_HASH_TeleportEnd, transitionAnimDuration);

                await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(ANIM_HASH_TeleportEnd)
                    , cancellationToken:_cancellationTokenSource.Token);
                float teleportEndTime = animator.GetCurrentAnimatorStateInfo(0).length;
                await UniTask.WaitForSeconds(teleportEndTime
                    , cancellationToken:_cancellationTokenSource.Token);
                
                owner.Finish_ChaseState();
                
            }

            private bool LookingTowardThePlayer()
            {
                return owner.IsFlippingLeft && owner.MyTransform.position.x <= owner.TargetTransform.position.x
                    || !owner.IsFlippingLeft && owner.MyTransform.position.x >= owner.TargetTransform.position.x;
            }
    
        }
        
        /// <summary>
        /// ATTACK STATE
        /// </summary>
        public class AttackState : BaseEnemyState.AttackState
        {
            private Coroutine _attackCoroutine;
    
            public AttackState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Attack, transitionAnimDuration);
    
                _attackCoroutine = owner.StartCoroutine(Attack_Coroutine());
    
            }
    
            public override void OnExit()
            {
                owner.StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
                
                base.OnExit();
            }
            
            //#
            
            private IEnumerator Attack_Coroutine()
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(ANIM_HASH_Attack));
                float attackTime = animator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(attackTime);
                owner.Finish_AttackState();
            }
    
        }
        
        /// <summary>
        /// PAIN STATE
        /// </summary>
        public class PainState : BaseEnemyState.PainState
        {
            private Coroutine _painCoroutine;
    
            public PainState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                base.OnEnter();
                
                blendAnimCoefficient = 0.8f;
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Pain, transitionAnimDuration);
    
                _painCoroutine = owner.StartCoroutine(Pain_Coroutine());
    
            }
    
            public override void OnExit()
            {
                owner.StopCoroutine(_painCoroutine);
                _painCoroutine = null;
                owner.Finish_PainState();
            }
            
            //#
            
            private IEnumerator Pain_Coroutine()
            {
                 yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(ANIM_HASH_Pain));
                 float painTime = animator.GetCurrentAnimatorStateInfo(0).length * blendAnimCoefficient;
                 yield return new WaitForSeconds(painTime);
                 owner.Finish_PainState();
            }
            
        }
        
        /// <summary>
        /// DIE STATE
        /// </summary>
        public class DeadState : BaseEnemyState.DeadState 
        {
            private Coroutine _deadCoroutine;
            
            public DeadState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_DeadState();
                
                animator.CrossFadeInFixedTime(ANIM_HASH_Dead, transitionAnimDuration);

                _deadCoroutine = owner.StartCoroutine(Dead_Coroutine());
                
            }
        
            public override void OnExit()
            {
                owner.StopCoroutine(_deadCoroutine);
                _deadCoroutine = null;
                
                base.OnExit();
            }
            
            //#
            private IEnumerator Dead_Coroutine()
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(ANIM_HASH_Dead));
                float painTime = animator.GetCurrentAnimatorStateInfo(0).length * blendAnimCoefficient;
                yield return new WaitForSeconds(painTime);
                
                owner.Deactivate();
            }

        }
     
    }
    
}


