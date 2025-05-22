using HIEU_NL.Platformer.Script.Effect;
using HIEU_NL.Platformer.Script.Entity.Enemy.Boss;
using HIEU_NL.Platformer.Script.Game;
using HIEU_NL.Platformer.Script.Interface;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Bringer
{
    using DesignPatterns.StateMachine;
    public class Bringer : BaseBoss
    {
        //# STATE MACHINE
        private Bringer_State.IdleState _idleState; //## Default State
        
        [SerializeField, Foldout("Attack"), MinMaxSlider(5, 20)] private Vector2 _spellDamage  = new Vector2 (10, 10);
        
        [SerializeField, BoxGroup("Shale Camera")] private CinemachineImpulseSource _cinemachineImpulseSource;
        [SerializeField, BoxGroup("Shale Camera")] private Vector3 _footstepForce;
        [SerializeField, BoxGroup("Shale Camera")] private Vector3 _attack_1_Force;
        [SerializeField, BoxGroup("Shale Camera")] private Vector3 _attack_2_Force;

        #region UNITY CORE

        protected override void Awake()
        {
            base.Awake();

            //##
            _idleState = new Bringer_State.IdleState(this, animator);
            Bringer_State.PatrolState patrolState = new Bringer_State.PatrolState(this, animator);
            Bringer_State.ChaseState chaseState = new Bringer_State.ChaseState(this, animator);
            Bringer_State.AttackState attackState = new Bringer_State.AttackState(this, animator);
            Bringer_State.PainState painState = new Bringer_State.PainState(this, animator);
            Bringer_State.DeadState deadState = new Bringer_State.DeadState(this, animator);

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
        

        #endregion
        
        protected override bool CanAnyToAttack()
        {
            if (!IsActivate) return false;
            
            if (attackIndex == 1)
            {
                return PlayerInChaseRange();
            }

            return base.CanAnyToAttack();
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
        
        private void AE_Attack_2_Effect()
        {
            ShakeCamera(_attack_2_Force);
            SpawnSpell();

            void SpawnSpell()
            {
                Vector3 spawnPosition = new Vector3(targetTransform.position.x, MyTransform.position.y, MyTransform.position.z);
                Prefab_Platformer spellPrefab = ObjectPool_Platformer.Instance.GetPoolObject(
                    PrefabType_Platformer.Spell_Bringer, spawnPosition);

                if (spellPrefab is AttackEffect_Platformer attackEffect)
                {
                    HitData hitData = new HitData(damage: (int)Random.Range(_spellDamage.x, _spellDamage.y));
                    attackEffect.Setup(hitData);
                }
                
                spellPrefab.Activate();
            }
        }

        #endregion


        public override bool PlayerInChaseRange()
        {
            if (GameMode_Platformer.Instance.Player.IsDead) return false;

            return targetTransform.position.x > PatrolPositionLeft.x - 3f
                   && targetTransform.position.x < PatrolPositionRight.x + 3f
                   && targetTransform.position.y > ChasePositionBelow.y
                   && targetTransform.position.y < ChasePositionAbove.y;
        }

    }
}

