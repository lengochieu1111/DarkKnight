using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Mosquito
{
    using DesignPatterns.StateMachine;
    public class Mosquito : BaseEnemy
    {
        //# STATE MACHINE
        private Mosquito_State.IdleState _idleState; //## Default State
        
        //## PAIN
        [SerializeField, Foldout("Pain")] private float _stunHealthPercentage = 0.3f;
        public float StunHealthPercentage => _stunHealthPercentage;

        
        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                _idleState = new Mosquito_State.IdleState(this, animator);
                Mosquito_State.PatrolState patrolState = new Mosquito_State.PatrolState(this, animator);
                Mosquito_State.ChaseState chaseState = new Mosquito_State.ChaseState(this, animator);
                Mosquito_State.AttackState attackState = new Mosquito_State.AttackState(this, animator);
                Mosquito_State.PainState painState = new Mosquito_State.PainState(this, animator);
                Mosquito_State.DeadState deadState = new Mosquito_State.DeadState(this, animator);

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

    }
}

