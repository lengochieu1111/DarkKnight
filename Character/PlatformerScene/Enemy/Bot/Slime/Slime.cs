using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Slime
{
    using DesignPatterns.StateMachine;
    public class Slime : BaseEnemy
    {
        //# STATE MACHINE
        private Slime_State.IdleState _idleState; //## Default State
        
        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                _idleState = new Slime_State.IdleState(this, animator);
                Slime_State.PatrolState patrolState = new Slime_State.PatrolState(this, animator);
                Slime_State.ChaseState chaseState = new Slime_State.ChaseState(this, animator);
                Slime_State.AttackState attackState = new Slime_State.AttackState(this, animator);
                Slime_State.PainState painState = new Slime_State.PainState(this, animator);
                Slime_State.DeadState deadState = new Slime_State.DeadState(this, animator);

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

