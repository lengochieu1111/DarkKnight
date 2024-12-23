using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Interface;
using UnityEngine;
using UnityEngine.Serialization;


namespace HIEU_NL.Platformer.Script.Entity.Enemy
{
    using DesignPatterns.StateMachine;
    using static Golem_1_State;
    using Player;
    using static HIEU_NL.Utilities.ParameterExtensions.Animation;
    
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D), typeof(BoxCollider2D))]
    public class BaseEnemy : BaseEntity
    {
        //# SELF
        public Transform MyTransform { get; private set; }
        [field: SerializeField, BoxGroup("PLAYER"), Required] public Player Player;
        
        //# ANIMATOR
        [SerializeField, BoxGroup("ANIMATOR"), Required] private Animator _animator;
        [field: SerializeField, BoxGroup("ANIMATOR")] public bool IsFlippingLeft { get; private set; }

        //# COLLISION CHECK
        [SerializeField, BoxGroup("COLLISION"), Required] private Rigidbody2D _rigidbody;
        [SerializeField, BoxGroup("COLLISION"), Required] private CapsuleCollider2D _capsuleCollider;
        [SerializeField, BoxGroup("COLLISION"), Required] private BoxCollider2D _boxCollider;

        [SerializeField, BoxGroup("COLLISION")] private bool _isGrounded;
        [SerializeField, BoxGroup("COLLISION")] private bool _bumpedHead;
        [field: SerializeField, BoxGroup("COLLISION")] public bool IsTouchingWall { get; private set; }
        [SerializeField, BoxGroup("COLLISION")] private LayerMask _groundLayer;
        [SerializeField, BoxGroup("COLLISION")] private float _groundDetectionRayLength = 0.02f;
        [SerializeField, BoxGroup("COLLISION")] private float _headWidth = 0.75f;
        [SerializeField, BoxGroup("COLLISION")] private float _headDetectionRayLength = 0.02f;
        [SerializeField, BoxGroup("COLLISION")] private float _wallDetectionRayLength = 0.125f;
        [SerializeField, BoxGroup("COLLISION")] private float _wallDetectionRayHeightMultiplier = 0.9f;
        private RaycastHit2D _groundHit;
        private RaycastHit2D _headHit;
        private RaycastHit2D _wallHit;
        private RaycastHit2D _lastWallHit;

        //# STATE MACHINE
        private StateMachine _stateMachine;
        private IdleState _idleState; //## Default State
        [field: SerializeField, BoxGroup("STATE")] public State CurrentState;
        [field: SerializeField, BoxGroup("STATE")] public State PreviousState;
        
        //## IDLE
        [SerializeField, Foldout("Idle")] private bool _isIdling;
        [field: SerializeField, Foldout("Idle")] public float IdleTimeMax { get; private set; } = 2f;

        //## PATROL
        [SerializeField, Foldout("Patrol")] private bool _isPatroling;
        [field: SerializeField, Foldout("Patrol")] public float PatrolSpeed { get; private set; } = 1f;
        [SerializeField, Foldout("Patrol")] private float _patrolRadius = 5f;
        public Vector3 LeftPatrolPosition { get; private set; }
        public Vector3 RightPatrolPosition { get; private set; }
        
        //## CHASE
        [SerializeField, Foldout("Chase")] private bool _isChasing;
        [field: SerializeField, Foldout("Chase")] public float ChaseSpeed { get; private set; } = 2f;
        [SerializeField, Foldout("Chase")] private float _chaseRadius = 5f;
        public Vector3 LeftChasePosition { get; private set; }
        public Vector3 RightChaselPosition { get; private set; }

        //## ATTACK
        [SerializeField, Foldout("Attack")] private bool _isAttacking;
        [SerializeField, Foldout("Attack")] private LayerMask _attackLayer;
        [SerializeField, Foldout("Attack")] private float _attackRadiusWidth = 5f;
        [SerializeField, Foldout("Attack")] private float _attackRangeHeight = 5f;
        [SerializeField, Foldout("Attack")] private float _attackOffsetWidth = 5f;
        [SerializeField, Foldout("Attack")] private float _attackOffsetHeight = 5f;
        
        //## PAIN
        [SerializeField, Foldout("Pain")] private bool _isPaining;
        [SerializeField, Foldout("Pain")] private bool _isRequestingPain;
        
        //## DEAD
        [SerializeField, Foldout("Dead")] private bool _isDead;
        [SerializeField, Foldout("Dead")] private bool _isRequestingDead;

        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                _stateMachine = new StateMachine();

                _idleState = new IdleState(this, _animator);
                PatrolState patrolState = new PatrolState(this, _animator);
                ChaseState chaseState = new ChaseState(this, _animator);
                AttackState attackState = new AttackState(this, _animator);
                PainState painState = new PainState(this, _animator);
                DeadState deadState = new DeadState(this, _animator);

                SetupTransitionStates();

                //## LOCAL FUNCTION
                void SetupTransitionStates()
                {
                    AddTransition(_idleState, patrolState, new FuncPredicate(CanIdleToPatrol));
                    AddTransition(patrolState, _idleState, new FuncPredicate(CanPatrolToIdle));
                    AddTransition(chaseState, patrolState, new FuncPredicate(CanChaseToPatrol));
                    
                    AddTransition(attackState, _idleState, new FuncPredicate(CanAttackToIdle));
                    AddTransition(attackState, patrolState, new FuncPredicate(CanAttackToPatrol));
                    
                    AddTransition(painState, patrolState, new FuncPredicate(CanPainToPatrol));

                    AddAnyTransition(chaseState, new FuncPredicate(CanAnyToChase));
                    AddAnyTransition(attackState, new FuncPredicate(CanAnyToAttack));
                    AddAnyTransition(painState, new FuncPredicate(CanAnyToPain));
                    AddAnyTransition(deadState, new FuncPredicate(CanAnyToDead));
                }
                
            }

            protected override void OnEnable()
            {
                base.OnEnable();

                _stateMachine.SetState(_idleState);
                
                _stateMachine.OnChangeState += StateMachine_OnOnChangeState;
                _stateMachine.OnChangeState += StateMachine_OnOnChangeState;

            }

            private void Update()
            {
                _stateMachine.Update();
            }

            private void FixedUpdate()
            {
                //##
                _stateMachine.FixedUpdate();
                
                //##
                CollisionChesks();
                
                //##
                ApplyGravity();

            }

            protected override void OnDisable()
            {
                base.OnDisable();
                
                _stateMachine.OnChangeState -= StateMachine_OnOnChangeState;
            }

            #endregion

        #region SETUP/RESET
        
            protected override void SetupValues()
            {
                base.SetupValues();

                MyTransform = transform;
            }

            protected override void ResetValues()
            {
                base.ResetValues();
                
                
                //##
                IsFlippingLeft = false;
                LeftPatrolPosition = new Vector3 (MyTransform.position.x - _patrolRadius, MyTransform.position.y);
                RightPatrolPosition = new Vector3 (MyTransform.position.x + _patrolRadius, MyTransform.position.y);
                LeftChasePosition = new Vector3 (MyTransform.position.x - _chaseRadius, MyTransform.position.y);
                RightChaselPosition = new Vector3 (MyTransform.position.x + _chaseRadius, MyTransform.position.y);

                /*
                Debug.DrawLine((Vector3)LeftPatrolPosition, (Vector3)LeftPatrolPosition + Vector3.up * 5, Color.green, 100f);
                Debug.DrawLine((Vector3)RightPatrolPosition, (Vector3)RightPatrolPosition + Vector3.up * 5, Color.green, 100f);
                Debug.DrawLine((Vector3)LeftChasePosition, (Vector3)LeftChasePosition + Vector3.up * 5, Color.red, 100f);
                Debug.DrawLine((Vector3)RightChaselPosition, (Vector3)RightChaselPosition + Vector3.up * 5, Color.red, 100f);
                */

                //##
                _isIdling = false;

                //##
                _isPatroling = false;

                //##
                _isChasing = false;

                //##
                _isAttacking = false;
                
                //##
                _isPaining = false;
                _isRequestingPain = false;
                
                //##
                _isDead = false;
                _isRequestingDead = false;

            }

        #endregion

        #region EVENT ACTION

            private void StateMachine_OnOnChangeState()
            {
                PreviousState = CurrentState;
            }

        #endregion

        #region STATE MACHINE

            private void AddTransition(IState from, IState to, IPredicate condition)
            {
                _stateMachine.AddTransition(from, to, condition);
            }

            private void AddAnyTransition(IState to, IPredicate condition)
            {
                _stateMachine.AddAnyTransition(to, condition);
            }

            //# TRANSTION STATE CONDITION FUNCTION
            private bool CanIdleToPatrol()
            {
                return !_isDead && !_isIdling && !_isPatroling;
            }
            
            private bool CanPatrolToIdle()
            {
                return !_isDead && !_isPatroling && !_isIdling;
            }
            
            private bool CanChaseToPatrol()
            {
                return !_isDead && !_isChasing && !_isPatroling;
            }
            
            private bool CanAttackToIdle()
            {
                return !_isDead && !_isAttacking && !_isIdling && PlayerInAttackRange();
            }
            
            private bool CanAttackToPatrol()
            {
                return !_isDead && !_isAttacking && !_isIdling && !PlayerInChaseRange();
            }
            
            private bool CanPainToPatrol()
            {
                return !_isDead && !_isPaining && !_isPatroling && !PlayerInChaseRange();
            }
            
            //## ANY TRASITION
            private bool CanAnyToChase()
            {
                return !_isDead && !_isPaining && !_isAttacking && !_isChasing && PlayerInChaseRange() && !PlayerInAttackRange();
            }
            
            private bool CanAnyToAttack()
            {
                return !_isDead && !_isPaining && !_isAttacking && PlayerInAttackRange() && CurrentState is not State.Attack;
            }
            
            private bool CanAnyToPain()
            {
                return !_isDead && _isRequestingPain && !_isPaining && CurrentState is not State.Pain;
            }
            
            private bool CanAnyToDead()
            {
                return _isRequestingDead && !_isDead && CurrentState is not State.Dead;
            }

            //# IDLE
            public void Begin_IdleState()
            {
                CurrentState = State.Idle;
                _isIdling = true;
            }

            public void Finish_IdleState()
            {
                _isIdling = false;
            } 
            
            //# PATROL
            public void Begin_PatrolState()
            {
                CurrentState = State.Patrol;
                _isPatroling = true;
            }

            public void Finish_PatrolState()
            {
                _isPatroling = false;
            }

            //# CHASE
            public void Begin_ChaseState()
            {
                CurrentState = State.Chase;
                _isChasing = true;
            }

            public void Finish_ChaseState()
            {
                _isChasing = false;
            }

            //# ATTACK
            public void Begin_AttackState()
            {
                CurrentState = State.Attack;
                _isAttacking = true;
            }

            public void Finish_AttackState()
            {
                _isAttacking = false;
            }
            
            //# PAIN
            public void Begin_PainState()
            {
                CurrentState = State.Pain;
                _isRequestingPain = false;
                _isPaining = true;
            }

            public void Finish_PainState()
            {
                _isRequestingPain = false;
                _isPaining = false;
            }
            
            //# DEAD
            public void Begin_DeadState()
            {
                CurrentState = State.Dead;
                _isDead = true;
            }

            public void Finish_DeadState()
            {
                _isDead = false;
            }

        #endregion

        #region COLLISION CHECK

            private void CollisionChesks()
            {
                Check_IsGrounded();
                Check_BumpedHead();
                Check_IsTouchingWall();
            }
            
            private void Check_IsGrounded()
            {
                Vector3 boxCenter = new Vector2(_boxCollider.bounds.center.x, _boxCollider.bounds.min.y);
                Vector3 boxSize = new Vector2(_boxCollider.bounds.size.x, _groundDetectionRayLength);

                _groundHit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, _groundDetectionRayLength, _groundLayer);
                _isGrounded = _groundHit.collider != null;

    /*            Color rayColor = Color.red;
                if (_isGrounded)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2), boxCenter.y), Vector2.down * _groundDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x + boxSize.x / 2), boxCenter.y), Vector2.down * _groundDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2), boxCenter.y - _groundDetectionRayLength), Vector2.right * boxSize.x, rayColor);*/

            }
            
            private void Check_BumpedHead()
            {
                Vector3 boxCenter = new Vector2(_capsuleCollider.bounds.center.x, _capsuleCollider.bounds.max.y);
                Vector3 boxSize = new Vector2(_capsuleCollider.bounds.size.x * _headWidth, _headDetectionRayLength);

                _headHit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.up, _headDetectionRayLength, _groundLayer);
                _bumpedHead = _headHit.collider != null;

    /*            Color rayColor = Color.red;
                if (_isGrounded)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2 * _headWidth), boxCenter.y), Vector2.up * _headDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x + (boxSize.x / 2) * _headWidth), boxCenter.y), Vector2.up * _headDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2 * _headWidth), boxCenter.y + _headDetectionRayLength), Vector2.right * boxSize.x * _headWidth, rayColor);*/

            }

            private void Check_IsTouchingWall()
            {
                float originEndPoint = 0f;
                if (IsFlippingLeft)
                {
                    originEndPoint = _capsuleCollider.bounds.min.x;
                }
                else
                {
                    originEndPoint = _capsuleCollider.bounds.max.x;
                }

                float adjustedHeight = _capsuleCollider.size.y * _wallDetectionRayHeightMultiplier;

                Vector2 boxCastOrigin = new Vector2(originEndPoint, _capsuleCollider.bounds.center.y);
                Vector2 boxCastSize = new Vector2(_wallDetectionRayLength, adjustedHeight);

                _wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, _wallDetectionRayLength, _groundLayer);

                if (_wallHit.collider != null)
                {
                    _lastWallHit = _wallHit;
                    IsTouchingWall = true;
                }
                else
                {
                    IsTouchingWall = false;
                }

                //##

                /*Color rayColor = Color.red;
                if (_isGrounded)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
                Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
                Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
                Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

                Debug.DrawRay(boxBottomLeft, boxBottomRight, rayColor);
                Debug.DrawRay(boxBottomRight, boxTopRight, rayColor);
                Debug.DrawRay(boxTopRight, boxTopLeft, rayColor);
                Debug.DrawRay(boxTopLeft, boxBottomLeft, rayColor);*/

            }

        #endregion

        #region MAIN

            private void ApplyGravity()
            {
                float _verticalVelocity = 0f;
                if (_isGrounded)
                {
                    _verticalVelocity = -0.01f;
                }
                else
                {
                    _verticalVelocity = Physics2D.gravity.y;
                }

                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _verticalVelocity);
            }

            //

            public bool PlayerInChaseRange()
            {
                return Player.transform.position.x > LeftChasePosition.x
                    && Player.transform.position.x < RightChaselPosition.x;
            }
            
            public bool PlayerInAttackRange()
            {
                int coefficient = IsFlippingLeft ? -1 : 1;
                Vector3 boxCenter = new Vector2(transform.position.x + _attackOffsetWidth * coefficient, transform.position.y + _attackOffsetHeight);
                Vector3 boxSize = new Vector2(_attackRadiusWidth, _attackRangeHeight);
                RaycastHit2D[] hitArray = Physics2D.BoxCastAll(boxCenter, boxSize, 0f, Vector2.right, 0f, _attackLayer);
                return hitArray.Length > 0;
            }
            
            /*public bool PlayerInAttackRange()
            {
                Vector2 boxCenter = new Vector2(transform.position.x + _attackOffsetWidth, transform.position.y + _attackOffsetHeight);
                Vector2 boxSize = new Vector2(_attackRadiusWidth, _attackRangeHeight);

                RaycastHit2D[] hitArray = Physics2D.BoxCastAll(boxCenter, boxSize, 0f, Vector2.right, 0f, _attackLayer);

                if (hitArray.Length > 0)
                {
                    foreach (RaycastHit hit in hitArray)
                    {
                        if (hit.transform.TryGetComponent(out HittableObject hittableObject))
                        {
                            HitData hitData = new HitData(
                                damageCauser: this,
                                isCausedByPlayer: false
                                );
                            hittableObject.OnHit(hitData);
                        }
                    }
                }

                return Player.transform.position.x > LeftChasePosition.x
                    && Player.transform.position.x < RightChaselPosition.x;
            }*/

        #endregion

        #region SETTER/GETTER

            public void SetIsFlippingLeft(bool isFlippingLeft)
            {
                IsFlippingLeft = isFlippingLeft;

                HandleFlip();

                //##

                void HandleFlip()
                {
                    if (IsFlippingLeft)
                    {
                        transform.Rotate(0, Y_AXIS_ROTATE_LEFT, 0);
                    }
                    else
                    {
                        transform.Rotate(0, Y_AXIS_ROTATE_RIGHT, 0);
                    }
                }

            }

        #endregion


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            int coefficient = IsFlippingLeft ? -1 : 1;
            Vector3 boxCenter = new Vector2(transform.position.x + _attackOffsetWidth * coefficient, transform.position.y + _attackOffsetHeight);
            Vector3 boxSize = new Vector2(_attackRadiusWidth, _attackRangeHeight);
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
        
        #region DAMAGE

        public override bool ITakeDamage(HitData hitData)
        {
            if (base.ITakeDamage(hitData))
            {
                if (health > 0)
                {
                    _isRequestingPain = true;
                }
                else
                {
                    _isRequestingDead = true;
                }

                return true;
            }
            
            return false;
        }

        #endregion

    }
}

