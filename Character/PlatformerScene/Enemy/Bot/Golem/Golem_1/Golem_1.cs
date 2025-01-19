using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Golem_1
{
    using DesignPatterns.StateMachine;
    public class Golem_1 : BaseEnemy
    {
        //# STATE MACHINE
        private Golem_1_State.IdleState _idleState; //## Default State
        
        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                _idleState = new Golem_1_State.IdleState(this, animator);
                Golem_1_State.PatrolState patrolState = new Golem_1_State.PatrolState(this, animator);
                Golem_1_State.ChaseState chaseState = new Golem_1_State.ChaseState(this, animator);
                Golem_1_State.AttackState attackState = new Golem_1_State.AttackState(this, animator);
                Golem_1_State.PainState painState = new Golem_1_State.PainState(this, animator);
                Golem_1_State.DeadState deadState = new Golem_1_State.DeadState(this, animator);

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

