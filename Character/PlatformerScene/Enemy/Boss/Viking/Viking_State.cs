using System.Collections;
using System.Threading;
using HIEU_NL.DesignPatterns.StateMachine;
using HIEU_NL.Utilities;
using HIEU_NL.Utilities.Move;
using UnityEngine;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Viking
{
    public class Viking_State
    {
        /// <summary>
        /// IDLE STATE
        /// </summary>
        public class IdleState : BaseEnemyState.IdleState
        {
            private Viking _viking;
            private int _animHash;
            private Coroutine _idleCoroutine;
    
            public IdleState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
    
                _viking = owner as Viking;
                
                _animHash = _viking.HasEnhanced ? ANIM_HASH_Idle_Enhanced : ANIM_HASH_Idle;
                animator.CrossFadeInFixedTime(_animHash, transitionAnimDuration);
    
                _idleCoroutine = owner.StartCoroutine(Idle_Coroutine());
    
            }
    
            public override void OnExit()
            {
                if (_idleCoroutine != null)
                {
                    owner.StopCoroutine(_idleCoroutine);
                    _idleCoroutine = null;
                }
                
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
            private Viking _viking;
            private int _animHash;
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
    
                _viking = owner as Viking;
                _animHash = _viking.HasEnhanced ? ANIM_HASH_Run_Enhanced : ANIM_HASH_Run;
                animator.CrossFadeInFixedTime(_animHash, transitionAnimDuration);
    
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
                    || !owner.IsFlippingLeft && owner.MyTransform.position.x >= owner.PatrolPositionRight.x
                    || owner.IsTouchingWall;
            }
    
        }
        
        /// <summary>
        /// CHASE STATE
        /// </summary>
        public class ChaseState : BaseEnemyState.ChaseState
        {
            private Viking _viking;
            private int _animHash;
            private Vector2 _chaseDirection;
            private float _chaseSpeed;
            
            private CancellationTokenSource _cancellationTokenSource;
    
            public ChaseState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
                
                _chaseSpeed = 0f;
                
                _viking = owner as Viking;
                _animHash = _viking.HasEnhanced ? ANIM_HASH_Run_Enhanced : ANIM_HASH_Run;
                animator.CrossFadeInFixedTime(_animHash, transitionAnimDuration);
            }
    
            public override void Update()
            {
                if (owner.PlayerInChaseRange())
                {
                    if (LookingTowardThePlayer())
                    {
                        owner.SetIsFlippingLeft(!owner.IsFlippingLeft);
                    }

                    _chaseSpeed = Mathf.Lerp(_chaseSpeed, owner.Stats.ChaseSpeed, owner.Stats.ChaseSpeed * Time.deltaTime);
                    _chaseDirection = owner.TargetTransform.position.x < owner.MyTransform.position.x ? Vector2.left : Vector2.right;

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
                _cancellationTokenSource?.Dispose();
                
                base.OnExit();
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
            private Viking _viking;
            private CurveMove _curveMove;
            private int _attackAnimHash;
            private int _actualAttackIndex;
            private Coroutine _attackCoroutine;
            private bool _isPlayedJumpEndAnimation;
            private bool _isPlayedAttackAnimation;
            public int ActualAttackIndex => _actualAttackIndex;
    
            public AttackState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
                
                _viking = owner as Viking;
                _isPlayedAttackAnimation = false;
                _isPlayedJumpEndAnimation = false;
                
                if (owner.AttackIndex == 3 || owner.AttackIndex == 4)
                {
                    _actualAttackIndex = Random.Range(3, 5);

                    /*if (_actualAttackIndex == 4 &&
                        Vector2.Distance(owner.MyTransform.position, owner.TargetTransform.position) <
                        _viking.SpecialAttack_2_MinAttackDistance)
                    {
                        _actualAttackIndex = 3;
                        
                        if (_actualAttackIndex == 3 && !GameMode_Platformer.Instance.Player.IsGrounded)
                        {
                            _actualAttackIndex = Random.Range(0, 3);
                        }
                    }
                    else if (_actualAttackIndex == 3 && !GameMode_Platformer.Instance.Player.IsGrounded)
                    {
                        _actualAttackIndex = 4;
                        
                        if (_actualAttackIndex == 4 &&
                            Vector2.Distance(owner.MyTransform.position, owner.TargetTransform.position) <
                            _viking.SpecialAttack_2_MinAttackDistance)
                        {
                            _actualAttackIndex = Random.Range(0, 3);
                        }
                    }
                    else
                    {
                        _actualAttackIndex = Random.Range(0, 4);
                    }*/
                    
                }
                else
                {
                    _actualAttackIndex = owner.AttackIndex;
                }

    
                if (_actualAttackIndex == 3)
                {
                    int jumpStartAnimHash = _viking.HasEnhanced ? ANIM_HASH_JumpStart_Enhanced : ANIM_HASH_JumpStart;
                    animator.CrossFadeInFixedTime(jumpStartAnimHash, transitionAnimDuration);

                    Vector2 startPosition = owner.MyTransform.position;
                    float widthOffset = owner.IsFlippingLeft
                        ? Random.Range(_viking.WidthTargetOffset.x, _viking.WidthTargetOffset.y)
                        : Random.Range(_viking.WidthTargetOffset.x, _viking.WidthTargetOffset.y) * -1;
                    Vector2 endPosition = owner.TargetTransform.position
                        .With(y: owner.MyTransform.position.y)
                        .Add(x: widthOffset);
                    
                    _curveMove = new CurveMove(owner.MyTransform, startPosition,
                        endPosition, _viking.JumpHeight, _viking.JumpSpeed);
                    
                    _curveMove.OnFinished += CurveMove_OnFinished;
                }
                else
                {
                    PlayAttackAnimation();
                }
    
            }

            private void CurveMove_OnFinished()
            {
                _curveMove.OnFinished -= CurveMove_OnFinished; 
                _curveMove = null;
            }

            public override void Update()
            {
                base.Update();

                if (_curveMove != null && _actualAttackIndex == 3)
                {
                    _curveMove?.Moving();

                    if (!_isPlayedJumpEndAnimation && _curveMove.GetProgressPercentage() > 0.5f)
                    {
                        _isPlayedJumpEndAnimation = true;
                        int jumpEmdAnimHash = _viking.HasEnhanced ? ANIM_HASH_JumpEnd_Enhanced : ANIM_HASH_JumpEnd;
                        animator.CrossFadeInFixedTime(jumpEmdAnimHash, transitionAnimDuration);
                    }
                    else if (!_isPlayedAttackAnimation && _curveMove.GetProgressPercentage() > 0.8f)
                    {
                        _isPlayedAttackAnimation = true;
                        PlayAttackAnimation();
                    }
                    
                }
                
            }

            public override void OnExit()
            {
                if (_attackCoroutine != null)
                {
                    owner.StopCoroutine(_attackCoroutine);
                    _attackCoroutine = null;
                }

                base.OnExit();
            }
            
            //#

            private void PlayAttackAnimation()
            {
                AnimationType animationType = AnimationType.Attack_1;
                switch (_actualAttackIndex)
                {
                    case 0:
                        animationType = _viking.HasEnhanced ? AnimationType.Attack_1_Enhanced : AnimationType.Attack_1;
                        break;
                    case 1:
                        animationType = _viking.HasEnhanced ? AnimationType.Attack_2_Enhanced : AnimationType.Attack_2;
                        break;
                    case 2:
                        animationType = _viking.HasEnhanced ? AnimationType.Attack_3_Enhanced : AnimationType.Attack_3;
                        break;
                    case 3:
                        animationType = _viking.HasEnhanced ? AnimationType.Attack_Special_1_Enhanced : AnimationType.Attack_Special_1;
                        break;
                    case 4:
                        animationType = _viking.HasEnhanced ? AnimationType.Attack_Special_2_Enhanced : AnimationType.Attack_Special_2;
                        break;
                }
                
                _attackAnimHash = ALL_ANIM_HASH[animationType];
                animator.CrossFadeInFixedTime(_attackAnimHash, transitionAnimDuration);
                    
                _attackCoroutine = owner.StartCoroutine(Attack_Coroutine());
            }
            
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
            private Viking _viking;
            private Coroutine _painCoroutine;
            private int _animHash;

            public PainState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                base.OnEnter();
                
                blendAnimCoefficient = 0.8f;

                _viking = owner as Viking;
                _animHash = _viking.HasEnhanced ? ANIM_HASH_Pain_Enhanced : ANIM_HASH_Pain;
                animator.CrossFadeInFixedTime(_animHash, transitionAnimDuration);
    
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
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(_animHash));
                float painTime = animator.GetCurrentAnimatorStateInfo(0).length * blendAnimCoefficient;
                yield return new WaitForSeconds(painTime);
                owner.Finish_PainState();
            }
            
        }
        
        /// <summary>
        /// ENHANCED STATE
        /// </summary>
        public class EnhancedState : BaseState<Viking>
        {
            protected float blendAnimCoefficient = 0.8f;
            protected float transitionAnimDuration = 0.2f;
            private Coroutine _enhancedCoroutine;
            private int _animHash;
    
            public EnhancedState(Viking owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_EnhancedState();
                
                blendAnimCoefficient = 0.8f;

                _animHash = ANIM_HASH_Enhanced;
                animator.CrossFadeInFixedTime(_animHash, transitionAnimDuration);
    
                _enhancedCoroutine = owner.StartCoroutine(Enhanced_Coroutine());
            }
            
    
            public override void OnExit()
            {
                owner.StopCoroutine(_enhancedCoroutine);
                _enhancedCoroutine = null;
                
                owner.Finish_EnhancedState();
            }
            
            //#
            
            private IEnumerator Enhanced_Coroutine()
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(_animHash));
                float painTime = animator.GetCurrentAnimatorStateInfo(0).length * blendAnimCoefficient;
                yield return new WaitForSeconds(painTime);
                owner.Finish_EnhancedState();
            }
            
        }
        
        /// <summary>
        /// DEFENSE STATE
        /// </summary>
        /*public class DefenseState : BaseState<Viking>
        {
            protected float blendAnimCoefficient = 0.8f;
            protected float transitionAnimDuration = 0.2f;
            private Coroutine _defenseCoroutine;
            private int _animHash;
    
            public DefenseState(Viking owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_DefenseState();
                
                blendAnimCoefficient = 0.8f;

                _animHash = ANIM_HASH_Defense;
                animator.CrossFadeInFixedTime(_animHash, 0);
    
                _defenseCoroutine = owner.StartCoroutine(Defense_Coroutine());
            }
            
    
            public override void OnExit()
            {
                owner.StopCoroutine(_defenseCoroutine);
                _defenseCoroutine = null;
                
                owner.Finish_DefenseState();
            }
            
            //#
            
            private IEnumerator Defense_Coroutine()
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(_animHash));
                float painTime = animator.GetCurrentAnimatorStateInfo(0).length * blendAnimCoefficient;
                yield return new WaitForSeconds(painTime);
                owner.Finish_DefenseState();
            }
            
        }*/
        
        /// <summary>
        /// DIE STATE
        /// </summary>
        public class DeadState : BaseEnemyState.DeadState 
        {
            private Viking _viking;
            private int _animHash;
            private Coroutine _deadCoroutine;
            
            public DeadState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_DeadState();

                _viking = owner as Viking;
                _animHash = _viking.HasEnhanced ? ANIM_HASH_Dead_Enhanced : ANIM_HASH_Dead;
                animator.CrossFadeInFixedTime(_animHash, transitionAnimDuration);

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


