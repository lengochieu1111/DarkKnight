using System.Collections;
using UnityEngine;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Skeleton_Sword
{
    public class Skeleton_Sword_State
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
                return owner.IsFlippingLeft && owner.MyTransform.position.x <= owner.PatrolPositionLeft.x
                       || !owner.IsFlippingLeft && owner.MyTransform.position.x >= owner.PatrolPositionRight.x;
                // || owner.IsTouchingWall;
            }
    
        }
        
        /// <summary>
        /// CHASE STATE
        /// </summary>
        public class ChaseState : BaseEnemyState.ChaseState
        {
            private Vector2 _chaseDirection;
            private float _chaseSpeed;
    
            public ChaseState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
    
                _chaseSpeed = 0f;
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Walk, transitionAnimDuration);
    
            }
    
            public override void Update()
            {
                if (owner.PlayerInChaseRange())
                {
                    if (LookingTowardThePlayer())
                    {
                        owner.SetIsFlippingLeft(!owner.IsFlippingLeft);
                    }
    
                    _chaseDirection = owner.MyTransform.position.x > owner.TargetTransform.position.x ? Vector2.left : Vector2.right;
                    _chaseSpeed = Mathf.Lerp(_chaseSpeed, owner.Stats.ChaseSpeed, owner.Stats.ChaseSpeed * Time.deltaTime);
    
                    owner.MyTransform.position += (Vector3)_chaseDirection * (_chaseSpeed * Time.deltaTime);
                }
                else
                {
                    owner.Finish_ChaseState();
                }
    
            }
    
            //#
    
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
            private int _attackAnimHash;
    
            public AttackState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();

                _attackAnimHash = ALL_ANIM_HASH[owner.Stats.AttackDataArray[owner.AttackIndex].AttackAnimType];
                animator.CrossFadeInFixedTime(_attackAnimHash, transitionAnimDuration);
    
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
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(_attackAnimHash));
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


