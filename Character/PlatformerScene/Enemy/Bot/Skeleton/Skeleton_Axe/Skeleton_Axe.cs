using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Skeleton_Axe
{
    using DesignPatterns.StateMachine;
    public class Skeleton_Axe : BaseEnemy
    {
        //# STATE MACHINE
        private Skeleton_Axe_State.IdleState _idleState; //## Default State
        
        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                _idleState = new Skeleton_Axe_State.IdleState(this, animator);
                Skeleton_Axe_State.PatrolState patrolState = new Skeleton_Axe_State.PatrolState(this, animator);
                Skeleton_Axe_State.ChaseState chaseState = new Skeleton_Axe_State.ChaseState(this, animator);
                Skeleton_Axe_State.AttackState attackState = new Skeleton_Axe_State.AttackState(this, animator);
                Skeleton_Axe_State.PainState painState = new Skeleton_Axe_State.PainState(this, animator);
                Skeleton_Axe_State.DeadState deadState = new Skeleton_Axe_State.DeadState(this, animator);

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

