using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Beetle
{
    using DesignPatterns.StateMachine;
    public class Beetle : BaseEnemy
    {
        //# STATE MACHINE
        private Beetle_State.IdleState _idleState; //## Default State
        
        //## PAIN
        [SerializeField, Foldout("Pain")] private float _stunHealthPercentage = 0.3f;
        public float StunHealthPercentage => _stunHealthPercentage;
        
        [SerializeField, Foldout("Attack")] private Transform[] _projectileTransform;


        #region UNITY CORE

        protected override void Awake()
        {
            base.Awake();

            //##
            _idleState = new Beetle_State.IdleState(this, animator);
            Beetle_State.PatrolState patrolState = new Beetle_State.PatrolState(this, animator);
            Beetle_State.ChaseState chaseState = new Beetle_State.ChaseState(this, animator);
            Beetle_State.AttackState attackState = new Beetle_State.AttackState(this, animator);
            Beetle_State.PainState painState = new Beetle_State.PainState(this, animator);
            Beetle_State.DeadState deadState = new Beetle_State.DeadState(this, animator);

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
            return !isDead && !isPaining && !isAttacking && PlayerInChaseRange() && PlayerInAttackRange() && currentState is not BaseEnemyState.State.Attack;
        }

        #region MAIN

        public override bool PlayerInAttackRange()
        {
            int coefficient = isFlippingLeft ? -1 : 1;
            // coefficient = isBeginFlipLeft ? coefficient * -1 : coefficient;
            Vector2 endAttackPoint = new Vector2(
                MyTransform.position.x + Stats.AttackDataArray[attackIndex].AttackRadiusWidth * coefficient,
                MyTransform.position.y);
                
            Debug.DrawRay(endAttackPoint, Vector2.up, Color.red);
            Debug.DrawLine(MyTransform.position, endAttackPoint, Color.red);

            return IsPointBetweenTwoPointsOnXAxis(endAttackPoint, MyTransform.position, TargetTransform.position);
        }

        private bool IsPointBetweenTwoPointsOnXAxis(Vector2 pointA, Vector2 pointB, Vector2 testPoint)
        {
            float minX = Mathf.Min(pointA.x, pointB.x);
            float maxX = Mathf.Max(pointA.x, pointB.x);
            return testPoint.x >= minX && testPoint.x < maxX;
        }

        #endregion

        #region ANIMATION EVENT

        private void AE_AttackProjectileLeft()
        {
            SpawnProjectile(_projectileTransform[0]);
        }
        
        private void AE_AttackProjectileRight()
        {
            SpawnProjectile(_projectileTransform[1]);
        }
        
        private void SpawnProjectile(Transform spawnPoint)
        {
            Prefab_Platformer projectile = ObjectPool_Platformer.Instance.GetPoolObject(PrefabType_Platformer.Projectile_Beetle, spawnPoint.position, Quaternion.identity);
            Projectile_Beetle projectile_Beetle = projectile as Projectile_Beetle;
            projectile_Beetle.Activate(TargetTransform.position);
        }

        #endregion

    }
}

