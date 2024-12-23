using HIEU_NL.DesignPatterns.StateMachine;
using HIEU_NL.Platformer.Script;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;

namespace HIEU_NL.Platformer.Script.Entity.Enemy
{
    public class Golem_1_State
    {
        public enum State
        {
            Idle,
            Patrol,
            Chase,
            Attack,
            Pain,
            Dead
        }
        
        /// <summary>
        /// IDLE STATE
        /// </summary>
        public class IdleState : BaseState<BaseEnemy>
        {
            private float _transitionAnimDuration = 0.2f;
            private Coroutine _idleCoroutine;
    
            public IdleState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                owner.Begin_IdleState();
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Idle, _transitionAnimDuration);
    
                _idleCoroutine = owner.StartCoroutine(Idle_Coroutine());
    
            }
    
            public override void OnExit()
            {
                owner.StopCoroutine(_idleCoroutine);
                _idleCoroutine = null;
                owner.Finish_IdleState();
            }
            
            //#
            
            IEnumerator Idle_Coroutine()
            {
                yield return new WaitForSeconds(owner.IdleTimeMax);
                owner.Finish_IdleState();
            }
    
        }
        
        /// <summary>
        /// PATROL STATE
        /// </summary>
        public class PatrolState : BaseState<BaseEnemy>
        {
            private Vector2 _patrolDirection;
            private float _patrolSpeed;
            private float _transitionAnimDuration = 0.2f;
    
            public PatrolState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                owner.Begin_PatrolState();
    
                if (HasReachToDestination())
                {
                    owner.SetIsFlippingLeft(!owner.IsFlippingLeft);
                }
    
                _patrolSpeed = 0f;
                _patrolDirection = owner.IsFlippingLeft ? Vector2.left : Vector2.right;
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Walk, _transitionAnimDuration);
    
            }
    
            public override void Update()
            {
                if (HasReachToDestination())
                {
                    HandleReachToDestionation();
                }
                else
                {
                    _patrolSpeed = Mathf.Lerp(_patrolSpeed, owner.PatrolSpeed, owner.PatrolSpeed * Time.deltaTime);
                    owner.MyTransform.position += (Vector3)_patrolDirection * _patrolSpeed * Time.deltaTime;
                }
    
            }
    
            public override void OnExit()
            {
                owner.Finish_PatrolState();
            }
    
            //# 
    
            void HandleReachToDestionation()
            {
                owner.Finish_PatrolState();
            }
    
            bool HasReachToDestination()
            {
                return owner.IsFlippingLeft && owner.MyTransform.position.x <= owner.LeftPatrolPosition.x
                    || !owner.IsFlippingLeft && owner.MyTransform.position.x >= owner.RightPatrolPosition.x
                    || owner.IsTouchingWall;
            }
    
        }
        
        /// <summary>
        /// CHASE STATE
        /// </summary>
        public class ChaseState : BaseState<BaseEnemy>
        {
            private Vector2 _chaseDirection;
            private float _chaseSpeed;
            private float _transitionAnimDuration = 0.2f;
    
            public ChaseState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                owner.Begin_ChaseState();
    
                _chaseSpeed = 0f;
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Walk, _transitionAnimDuration);
    
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
                    _chaseSpeed = Mathf.Lerp(_chaseSpeed, owner.ChaseSpeed, owner.ChaseSpeed * Time.deltaTime);
    
                    owner.MyTransform.position += (Vector3)_chaseDirection * _chaseSpeed * Time.deltaTime;
                }
                else
                {
                    owner.Finish_ChaseState();
                }
    
            }
    
            public override void OnExit()
            {
                owner.Finish_ChaseState();
            }
            
            //#
    
            bool LookingTowardThePlayer()
            {
                return owner.IsFlippingLeft && owner.MyTransform.position.x <= owner.TargetTransform.position.x
                    || !owner.IsFlippingLeft && owner.MyTransform.position.x >= owner.TargetTransform.position.x;
            }
    
    
        }
        
        /// <summary>
        /// ATTACK STATE
        /// </summary>
        public class AttackState : BaseState<BaseEnemy>
        {
            private float _transitionAnimDuration = 0.2f;
            private Coroutine _attackCoroutine;
    
            public AttackState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                owner.Begin_AttackState();
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Attack, _transitionAnimDuration);
    
                _attackCoroutine = owner.StartCoroutine(Attack_Coroutine());
    
            }
    
            public override void OnExit()
            {
                owner.StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
                owner.Finish_AttackState();
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
        public class PainState : BaseState<BaseEnemy>
        {
            private float _blendAnimCoefficient = 0.8f;
            private float _transitionAnimDuration = 0.2f;
            private Coroutine _painCoroutine;
    
            public PainState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_PainState();
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Pain, _transitionAnimDuration);
    
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
                 float painTime = animator.GetCurrentAnimatorStateInfo(0).length * _blendAnimCoefficient;
                 yield return new WaitForSeconds(painTime);
                 owner.Finish_PainState();
             }
            
        }
        
        /// <summary>
        /// DIE STATE
        /// </summary>
        public class DeadState : BaseState<BaseEnemy>
        {
            private float _blendAnimCoefficient = 1f;
            private float _transitionAnimDuration = 0.2f;
            private Coroutine _deadCoroutine;
            
            public DeadState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_DeadState();
                
                animator.CrossFadeInFixedTime(ANIM_HASH_Dead, _transitionAnimDuration);

                _deadCoroutine = owner.StartCoroutine(Dead_Coroutine());
                
            }
        
            public override void OnExit()
            {
                owner.StopCoroutine(_deadCoroutine);
                _deadCoroutine = null;
                
                owner.Finish_DeadState();
            }
            
            //#
            private IEnumerator Dead_Coroutine()
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(ANIM_HASH_Dead));
                float painTime = animator.GetCurrentAnimatorStateInfo(0).length * _blendAnimCoefficient;
                yield return new WaitForSeconds(painTime);
                
                owner.Deactivate();
            }

        }
        
    }
}


