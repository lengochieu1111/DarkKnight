using System.Collections;
using HIEU_NL.Platformer.Script.Effect;
using HIEU_NL.Platformer.Script.Entity.Enemy.Boss;
using HIEU_NL.Platformer.Script.Game;
using HIEU_NL.Platformer.Script.Interface;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Viking
{
    using DesignPatterns.StateMachine;
    public class Viking : BaseBoss
    {
        //# STATE MACHINE
        private Viking_State.IdleState _idleState; //## Default State
        private Viking_State.AttackState _attackState;
        
        [SerializeField, BoxGroup("Ice"), Required] protected Transform _iceTransform;
        
        [SerializeField, BoxGroup("Shake Camera")] private CinemachineImpulseSource _cinemachineImpulseSource;
        [SerializeField, BoxGroup("Shake Camera")] private Vector3 _footstepForce;
        [SerializeField, BoxGroup("Shake Camera")] private Vector3 _attack_1_Force;
        [SerializeField, BoxGroup("Shake Camera")] private Vector3 _attack_2_Force;

        [field: SerializeField, Foldout("Attack"), ReadOnly] public bool HasRequestRandomSpecialAttack;
        [field: SerializeField, Foldout("Attack"), MinMaxSlider(5f, 15f)] public Vector2 RandomTimeSpecialAttack { get; private set; } = new Vector2(5f, 10f);
        [field: SerializeField, Foldout("Attack"), MinMaxSlider(2, 6)] public Vector2 SpecialAttack_2_CountRange { get; private set; } = new Vector2(1, 3);
        [field: SerializeField, Foldout("Attack")] public float SpecialAttack_2_MinAttackDistance { get; private set; } = 3f;
        [field: SerializeField, Foldout("Attack")] public float JumpHeight { get; private set; } = 8f;
        [field: SerializeField, Foldout("Attack")] public float JumpSpeed { get; private set; } = 16f;
        [field: SerializeField, Foldout("Attack"), MinMaxSlider(0.5f, 2f)] public Vector2 WidthTargetOffset { get; private set; } = new Vector2(0.5f, 1f);

        [SerializeField, Foldout("Attack"), MinMaxSlider(5, 20)] private Vector2 _iceDamage = new Vector2 (10, 10);

        private float _randomTimeSpecialAttackCounter;
        
        [SerializeField, Foldout("Enhanced")] private bool _hasEnhanced;
        [SerializeField, Foldout("Enhanced")] private bool _isEnhancing;
        [SerializeField, Foldout("Enhanced")] private bool _isRequestingEnhanced;
        [field: SerializeField, Foldout("Enhanced"), Range(0.1f, 0.9f)] public float HealthPercentageToEnhanced { get; private set; } = 0.4f;
        public bool HasEnhanced => _hasEnhanced;
        
        #region UNITY CORE

        protected override void Awake()
        {
            base.Awake();

            //##
            _idleState = new Viking_State.IdleState(this, animator);
            Viking_State.PatrolState patrolState = new Viking_State.PatrolState(this, animator);
            Viking_State.ChaseState chaseState = new Viking_State.ChaseState(this, animator);
            _attackState = new Viking_State.AttackState(this, animator);
            Viking_State.PainState painState = new Viking_State.PainState(this, animator);
            Viking_State.DeadState deadState = new Viking_State.DeadState(this, animator);
            Viking_State.EnhancedState enhancedState = new Viking_State.EnhancedState(this, animator);

            SetupTransitionStates();

            //## LOCAL FUNCTION
            void SetupTransitionStates()
            {
                AddTransition(_idleState, patrolState, new FuncPredicate(CanIdleToPatrol));
                AddTransition(patrolState, _idleState, new FuncPredicate(CanPatrolToIdle));
                AddTransition(chaseState, patrolState, new FuncPredicate(CanChaseToPatrol));
                
                AddTransition(_attackState, _idleState, new FuncPredicate(CanAttackToIdle));
                AddTransition(_attackState, patrolState, new FuncPredicate(CanAttackToPatrol));
                
                AddTransition(painState, patrolState, new FuncPredicate(CanPainToPatrol));
                
                AddTransition(enhancedState, chaseState, new FuncPredicate(CanEnhancedToChase));

                AddAnyTransition(chaseState, new FuncPredicate(CanAnyToChase));
                AddAnyTransition(_attackState, new FuncPredicate(CanAnyToAttack));
                AddAnyTransition(enhancedState, new FuncPredicate(CanAnyToEnhanced));
                AddAnyTransition(painState, new FuncPredicate(CanAnyToPain));
                AddAnyTransition(deadState, new FuncPredicate(CanAnyToDead));
            }
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            stateMachine.SetState(_idleState);

        }

        protected override void Update()
        {
            base.Update();
            
            //##
            if (IsActivate && !HasRequestRandomSpecialAttack)
            {
                _randomTimeSpecialAttackCounter += Time.deltaTime;

                float randomTimeSpecialAttack = _hasEnhanced ? RandomTimeSpecialAttack.x : RandomTimeSpecialAttack.y;
                if (_randomTimeSpecialAttackCounter >= randomTimeSpecialAttack)
                {
                    HasRequestRandomSpecialAttack = true;
                }
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            //##
            SpecialAttack_2_CountRange = new Vector2(Mathf.RoundToInt(SpecialAttack_2_CountRange.x), Mathf.RoundToInt(SpecialAttack_2_CountRange.y));

            HealthPercentageToEnhanced = Mathf.Floor(HealthPercentageToEnhanced * 10f) / 10f;

        }

        #endregion

        protected override void ResetValues()
        {
            base.ResetValues();
            
            //##
            HasRequestRandomSpecialAttack = false;
            
            //##
            _hasEnhanced = false;
            _isEnhancing = false;
            _isRequestingEnhanced = false;
            
        }

        #region MAIN

        private void ShakeCamera(Vector3 force)
        {
            _cinemachineImpulseSource.GenerateImpulseAt(transform.position, force);
        }

        #endregion

        #region ANIMATION EVENT

        private void AE_FootstepEffect()
        {
            ShakeCamera(_footstepForce);
        }
        
        private void AE_Attack_1_Effect()
        {
            ShakeCamera(_attack_1_Force);
        }
        
        private void AE_Attack_Special_2_Effect()
        {
            SpawnIce();

            void SpawnIce()
            {
                Prefab_Platformer icePrefab = ObjectPool_Platformer.Instance.GetPoolObject(
                    PrefabType_Platformer.Ice_Viking, _iceTransform.position);

                if (icePrefab is AttackEffect_Platformer attackEffect)
                {
                    attackEffect.Setup(new HitData(damage: (int)Random.Range(_iceDamage.x, _iceDamage.y)));
                }
                icePrefab.Activate();
            }
        }

        #endregion
        
        //##
        
        protected virtual bool CanEnhancedToChase()
        {
            return !isDead && !_isEnhancing;
        }
        
        public override void Finish_AttackState()
        {
            base.Finish_AttackState();
            
            if (attackIndex == 3 || attackIndex == 4)
            {
                HasRequestRandomSpecialAttack = false;
                _randomTimeSpecialAttackCounter = 0;
            }
        }

        //## ANY TRANSITION

        
        protected override bool CanAnyToAttack()
        {
            if (!IsActivate) return false;

            if (attackIndex == 3 || attackIndex == 4)
            {
                return !isDead && !isPaining && !_isEnhancing && !isAttacking && currentState is not BaseEnemyState.State.Attack;
            }
            
            if (HasRequestRandomSpecialAttack)
            {
                if (!isDead && !isPaining && !_isEnhancing && !isAttacking &&
                    currentState is not BaseEnemyState.State.Attack)
                {
                    attackIndex = Random.Range(3, 5);
                    return true;
                }
            }

            return !isDead && !isPaining && !_isEnhancing && !isAttacking && PlayerInAttackRange() && currentState is not BaseEnemyState.State.Attack;

        }
        
        protected virtual bool CanAnyToEnhanced()
        {
            return !isDead && !(isAttacking && _attackState.ActualAttackIndex == 3) && _isRequestingEnhanced && !_isEnhancing && currentState is not BaseEnemyState.State.Enhanced;
        }
        
        protected override bool CanAnyToChase()
        {
            return !isDead && !_isEnhancing && !isPaining && !isAttacking && !isChasing && PlayerInChaseRange() && !PlayerInAttackRange();
        }
        
        protected override bool CanAnyToPain()
        {
            return !isDead && !_isEnhancing && !(isAttacking && _attackState.ActualAttackIndex == 3) && canPain && isRequestingPain && !isPaining && currentState is not BaseEnemyState.State.Pain;
        }
        

        //#
        
        public virtual void Begin_EnhancedState()
        {
            currentState = BaseEnemyState.State.Enhanced;
            _isEnhancing = true;
            _hasEnhanced = true;
            _isRequestingEnhanced = false;
        }

        public virtual void Finish_EnhancedState()
        {
            _isEnhancing = false;
        } 
        
        //#
        
        public override bool PlayerInChaseRange()
        {
            if (GameMode_Platformer.Instance.Player.IsDead) return false;

            return targetTransform.position.x > PatrolPositionLeft.x - 3f
                   && targetTransform.position.x < PatrolPositionRight.x + 3f
                   && targetTransform.position.y > ChasePositionBelow.y
                   && targetTransform.position.y < ChasePositionAbove.y;
        }
        
        
        protected override void HandlePain()
        {
            if (!_hasEnhanced && GetHealthPercentage() < HealthPercentageToEnhanced)
            {
                _isRequestingEnhanced = true;
            }
            else if (canPain)
            {
                isRequestingPain = true;
            }

        }
        
        
    }
}

