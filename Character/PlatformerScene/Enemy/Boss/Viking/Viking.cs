using HIEU_NL.Platformer.Script.Entity.Enemy.Boss;
using HIEU_NL.Platformer.Script.Interface;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Viking
{
    using DesignPatterns.StateMachine;
    public class Viking : BaseBoss
    {
        //# STATE MACHINE
        private Viking_State.IdleState _idleState; //## Default State
        
        [SerializeField, BoxGroup("Shake Camera")] private CinemachineImpulseSource _cinemachineImpulseSource;
        [SerializeField, BoxGroup("Shake Camera")] private Vector3 _footstepForce;
        [SerializeField, BoxGroup("Shake Camera")] private Vector3 _attack_1_Force;
        [SerializeField, BoxGroup("Shake Camera")] private Vector3 _attack_2_Force;
        
        [field: SerializeField, Foldout("Attack"), MinMaxSlider(1, 3)] public Vector2 Attack_1_CountRange { get; private set; } = new Vector2(1, 3);
        [field: SerializeField, Foldout("Attack"), MinMaxSlider(1, 3)] public Vector2 SpecialAttack_2_CountRange { get; private set; } = new Vector2(1, 3);
        [field: SerializeField, Foldout("Attack")] public float JumpHeight { get; private set; } = 5f;
        [field: SerializeField, Foldout("Attack")] public float JumpSpeed { get; private set; } = 1f;
        [field: SerializeField, Foldout("Attack"), MinMaxSlider(0.5f, 2f)] public Vector2 WidthTargetOffset { get; private set; } = new Vector2(0.5f, 1f);

        #region UNITY CORE

        protected override void Awake()
        {
            base.Awake();

            //##
            _idleState = new Viking_State.IdleState(this, animator);
            Viking_State.PatrolState patrolState = new Viking_State.PatrolState(this, animator);
            Viking_State.ChaseState chaseState = new Viking_State.ChaseState(this, animator);
            Viking_State.AttackState attackState = new Viking_State.AttackState(this, animator);
            Viking_State.PainState painState = new Viking_State.PainState(this, animator);
            Viking_State.DeadState deadState = new Viking_State.DeadState(this, animator);

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

            stateMachine.SetState(_idleState);

        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            //##
            Attack_1_CountRange = new Vector2(Mathf.RoundToInt(Attack_1_CountRange.x), Mathf.RoundToInt(Attack_1_CountRange.y));
            SpecialAttack_2_CountRange = new Vector2(Mathf.RoundToInt(SpecialAttack_2_CountRange.x), Mathf.RoundToInt(SpecialAttack_2_CountRange.y));
        }

        #endregion

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
        
        private void AE_Attack_2_Effect()
        {
            ShakeCamera(_attack_2_Force);
            SpawnSpell();

            void SpawnSpell()
            {
                Vector3 spawnPosition = new Vector3(targetTransform.position.x, MyTransform.position.y, MyTransform.position.z);
                Prefab_Platformer spellPrefab = ObjectPool_Platformer.Instance.GetPoolObject(
                    PrefabType_Platformer.Spell_Bringer, spawnPosition);
                spellPrefab.Activate();
            }
        }

        #endregion

        
        protected override bool CanAnyToAttack()
        {
            if (attackIndex == 3 || attackIndex == 4)
            {
                return !isDead && !isPaining && !isAttacking && currentState is not BaseEnemyState.State.Attack;
            }

            return base.CanAnyToAttack();
        }

        public override bool PlayerInChaseRange()
        {
            return targetTransform.position.x > PatrolPositionLeft.x - 3f
                   && targetTransform.position.x < PatrolPositionRight.x + 3f
                   && targetTransform.position.y > ChasePositionBelow.y
                   && targetTransform.position.y < ChasePositionAbove.y;
        }

    }
}

