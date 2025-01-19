using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Mushroom
{
    using DesignPatterns.StateMachine;
    public class Mushroom : BaseEnemy
    {
        //# STATE MACHINE
        private Mushroom_State.IdleState _idleState; //## Default State
        
        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                _idleState = new Mushroom_State.IdleState(this, animator);
                Mushroom_State.PatrolState patrolState = new Mushroom_State.PatrolState(this, animator);
                Mushroom_State.ChaseState chaseState = new Mushroom_State.ChaseState(this, animator);
                Mushroom_State.AttackState attackState = new Mushroom_State.AttackState(this, animator);
                Mushroom_State.PainState painState = new Mushroom_State.PainState(this, animator);
                Mushroom_State.DeadState deadState = new Mushroom_State.DeadState(this, animator);

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

        #region SETUP/RESET

        protected override void ResetValues()
        {
            base.ResetValues();

            //##
            isFlippingLeft = true;
        }
        
        #endregion

    }
}

