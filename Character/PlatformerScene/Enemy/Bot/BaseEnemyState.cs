using UnityEngine;
using System.Collections;
using HIEU_NL.DesignPatterns.StateMachine;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;

namespace HIEU_NL.Platformer.Script.Entity.Enemy
{
    public class BaseEnemyState
    {
        public enum State
        {
            Idle,
            Patrol,
            Chase,
            Attack,
            Pain,
            Dead,
            
            Enhanced,
            Defense
        }
        
        /// <summary>
        /// IDLE STATE
        /// </summary>
        public class IdleState : BaseState<BaseEnemy>
        {
            protected float blendAnimCoefficient = 1f;
            protected float transitionAnimDuration = 0.2f;
            
            public IdleState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                owner.Begin_IdleState();
            }
    
            public override void OnExit()
            {
                owner.Finish_IdleState();
            }
            
        }
        
        /// <summary>
        /// PATROL STATE
        /// </summary>
        public class PatrolState : BaseState<BaseEnemy>
        {
            protected float blendAnimCoefficient = 1f;
            protected float transitionAnimDuration = 0.2f;
    
            public PatrolState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                owner.Begin_PatrolState();
    
            }
    
            public override void OnExit()
            {
                owner.Finish_PatrolState();
            }
    
        }
        
        /// <summary>
        /// CHASE STATE
        /// </summary>
        public class ChaseState : BaseState<BaseEnemy>
        {
            protected float blendAnimCoefficient = 1f;
            protected float transitionAnimDuration = 0.2f;
    
            public ChaseState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                owner.Begin_ChaseState();
            }
    
            public override void OnExit()
            {
                owner.Finish_ChaseState();
            }
            
        }
        
        /// <summary>
        /// ATTACK STATE
        /// </summary>
        public class AttackState : BaseState<BaseEnemy>
        {
            protected float blendAnimCoefficient = 1f;
            protected float transitionAnimDuration = 0.2f;
    
            public AttackState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                owner.Begin_AttackState();
            }
    
            public override void OnExit()
            {
                owner.Finish_AttackState();
            }
            
        }
        
        /// <summary>
        /// PAIN STATE
        /// </summary>
        public class PainState : BaseState<BaseEnemy>
        {
            protected float blendAnimCoefficient = 0.8f;
            protected float transitionAnimDuration = 0.2f;
    
            public PainState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_PainState();
            }
    
            public override void OnExit()
            {
                owner.Finish_PainState();
            }
            
        }
        
        /// <summary>
        /// DIE STATE
        /// </summary>
        public class DeadState : BaseState<BaseEnemy>
        {
            protected float blendAnimCoefficient = 1f;
            protected float transitionAnimDuration = 0.2f;
            
            public DeadState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_DeadState();
            }
        
            public override void OnExit()
            {
                owner.Finish_DeadState();
            }
            
        }
    }
}