using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Skeleton_Sword
{
    using DesignPatterns.StateMachine;
    public class Skeleton_Sword : BaseEnemy
    {
        //# STATE MACHINE
        private Skeleton_Sword_State.IdleState _idleState; //## Default State
        
        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                _idleState = new Skeleton_Sword_State.IdleState(this, animator);
                Skeleton_Sword_State.PatrolState patrolState = new Skeleton_Sword_State.PatrolState(this, animator);
                Skeleton_Sword_State.ChaseState chaseState = new Skeleton_Sword_State.ChaseState(this, animator);
                Skeleton_Sword_State.AttackState attackState = new Skeleton_Sword_State.AttackState(this, animator);
                Skeleton_Sword_State.PainState painState = new Skeleton_Sword_State.PainState(this, animator);
                Skeleton_Sword_State.DeadState deadState = new Skeleton_Sword_State.DeadState(this, animator);

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

