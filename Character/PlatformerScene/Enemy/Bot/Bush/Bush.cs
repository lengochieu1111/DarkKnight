using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Bush
{
    using DesignPatterns.StateMachine;
    public class Bush : BaseEnemy
    {
        //# STATE MACHINE
        private Bush_State.IdleState _idleState; //## Default State
        
        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                _idleState = new Bush_State.IdleState(this, animator);
                Bush_State.PatrolState patrolState = new Bush_State.PatrolState(this, animator);
                Bush_State.ChaseState chaseState = new Bush_State.ChaseState(this, animator);
                Bush_State.AttackState attackState = new Bush_State.AttackState(this, animator);
                Bush_State.PainState painState = new Bush_State.PainState(this, animator);
                Bush_State.DeadState deadState = new Bush_State.DeadState(this, animator);

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

