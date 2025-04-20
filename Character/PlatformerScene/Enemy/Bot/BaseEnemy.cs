using System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HIEU_NL.Platformer.Script.Game;
using HIEU_NL.Platformer.Script.Interface;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using HIEU_NL.Platformer.SerializableClass;
using HIEU_NL.Platformer.SO.Entity.Enemy;
using HIEU_NL.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace HIEU_NL.Platformer.Script.Entity.Enemy
{
    using DesignPatterns.StateMachine;
    using static HIEU_NL.Utilities.ParameterExtensions.Animation;
    
    [RequireComponent(typeof(CapsuleCollider2D), typeof(BoxCollider2D))]
    public abstract class BaseEnemy : BaseEntity, IAttackable
    {
        public static event EventHandler OnAnyDeadEnemy;

        //# STATS
        public EnemyStats Stats => stats ? stats as EnemyStats : null;
        
        //# ANIMATOR
        [SerializeField, BoxGroup("ANIMATOR"), Required] protected Animator animator;

        //# STATS
        [SerializeField, BoxGroup("STATS"), Required] protected EnemyStats enemyStats;
        
        //# HEALTH
        [SerializeField, BoxGroup("HEALTH")] protected Bar_PlatformerUI healthBar;

        //# STATE MACHINE
        protected StateMachine stateMachine;
        [SerializeField, BoxGroup("STATE")] protected BaseEnemyState.State currentState;
        [SerializeField, BoxGroup("STATE")] protected BaseEnemyState.State previousState;
        public BaseEnemyState.State CurrentState => currentState;
        public BaseEnemyState.State PreviousState => previousState;
        
        //## IDLE
        [SerializeField, Foldout("Idle")] protected bool isIdling;

        //## PATROL
        [SerializeField, Foldout("Patrol")] protected bool isPatroling;
        public Vector3 PatrolPositionLeft { get; protected set; }
        public Vector3 PatrolPositionRight { get; protected set; }
        
        //## CHASE
        [SerializeField, Foldout("Chase")] protected bool isChasing;
        [SerializeField, Foldout("Chase")] protected Transform targetTransform;
        public Transform TargetTransform => targetTransform;
        public Vector3 ChasePositionBelow { get; protected set; }
        public Vector3 ChasePositionAbove { get; protected set; }

        //## ATTACK
        [SerializeField, Foldout("Attack")] protected bool isAttacking;
        [SerializeField, Foldout("Attack")] protected bool isTracing;
        [SerializeField, Foldout("Attack")] protected int attackIndex;
        public int AttackIndex => attackIndex;
        private List<HittableObject> _listHasBeenHit = new();

        //## PAIN
        [SerializeField, Foldout("Pain")] protected bool canPain;
        [SerializeField, Foldout("Pain")] protected bool isPaining;
        [SerializeField, Foldout("Pain")] protected bool isRequestingPain;
        [SerializeField, Foldout("Pain")] protected float painRecoveryTime = 5f;
        
        
        //## SPAWM ITEM
        [SerializeField, BoxGroup("SPAWN ITEM")] protected SpawnItemData spawnItemData;

        #region UNITY CORE

            protected override void Awake()
            {
                base.Awake();

                //##
                stateMachine = new StateMachine();

                /*
                idleState = new BaseEnemyState.IdleState(this, animator);
                BaseEnemyState.IdleState golem1PatrolState = new BaseEnemyState.IdleState(this, animator);
                BaseEnemyState.ChaseState chaseState = new BaseEnemyState.ChaseState(this, animator);
                BaseEnemyState.AttackState attackState = new BaseEnemyState.AttackState(this, animator);
                BaseEnemyState.PainState painState = new BaseEnemyState.PainState(this, animator);
                BaseEnemyState.DeadState deadState = new BaseEnemyState.DeadState(this, animator);

                SetupTransitionStates();

                //## LOCAL FUNCTION
                void SetupTransitionStates()
                {
                    AddTransition(_idleState, golem1PatrolState, new FuncPredicate(CanIdleToPatrol));
                    AddTransition(golem1PatrolState, idleState, new FuncPredicate(CanPatrolToIdle));
                    AddTransition(chaseState, golem1PatrolState, new FuncPredicate(CanChaseToPatrol));
                    
                    AddTransition(attackState, idleState, new FuncPredicate(CanAttackToIdle));
                    AddTransition(attackState, golem1PatrolState, new FuncPredicate(CanAttackToPatrol));
                    
                    AddTransition(painState, golem1PatrolState, new FuncPredicate(CanPainToPatrol));

                    AddAnyTransition(chaseState, new FuncPredicate(CanAnyToChase));
                    AddAnyTransition(attackState, new FuncPredicate(CanAnyToAttack));
                    AddAnyTransition(painState, new FuncPredicate(CanAnyToPain));
                    AddAnyTransition(deadState, new FuncPredicate(CanAnyToDead));
                }*/
                
            }

            protected override void OnEnable()
            {
                base.OnEnable();

                stateMachine.OnChangeState += StateMachine_OnChangeState;
                // OnTakeDamage += Self_OnTakeDamage;
            }

            protected virtual void Update()
            {
                stateMachine.Update();

                if (isTracing)
                {
                    ITracing();
                }

            }

            protected override void FixedUpdate()
            {
                base.FixedUpdate();
                
                //##
                stateMachine.FixedUpdate();
                
            }

            protected override void OnDisable()
            {
                base.OnDisable();
                
                // OnTakeDamage -= Self_OnTakeDamage;
                stateMachine.OnChangeState -= StateMachine_OnChangeState;
            }

            #endregion

        #region SETUP/RESET
        
        protected override void ResetValues()
        {
            base.ResetValues();
            
            targetTransform = GameMode_Platformer.Instance.Player.MyTransform;
            
            //##
            if (mapPlacementPointPlatformer.IsValid() && mapPlacementPointPlatformer.DestinationPointArray.IsValid()
                && mapPlacementPointPlatformer.DestinationPointArray[0].IsValid() && mapPlacementPointPlatformer.DestinationPointArray[1].IsValid()
                && mapPlacementPointPlatformer.DestinationPointArray[2].IsValid() && mapPlacementPointPlatformer.DestinationPointArray[3].IsValid())
            {
                PatrolPositionLeft = mapPlacementPointPlatformer.DestinationPointArray[0].transform.position;
                PatrolPositionRight = mapPlacementPointPlatformer.DestinationPointArray[1].transform.position;
                ChasePositionAbove = mapPlacementPointPlatformer.DestinationPointArray[2].transform.position;
                ChasePositionBelow = mapPlacementPointPlatformer.DestinationPointArray[3].transform.position;
            }
            else
            {
                PatrolPositionLeft = transform.position.Add(x: -4);
                PatrolPositionRight = transform.position.Add(x: 4);
                ChasePositionBelow = transform.position.Add(y: -4);
                ChasePositionAbove = transform.position.Add(y: 4);
            }

            /*
            Debug.DrawLine((Vector3)LeftPatrolPosition, (Vector3)LeftPatrolPosition + Vector3.up * 5, Color.green, 100f);
            Debug.DrawLine((Vector3)RightPatrolPosition, (Vector3)RightPatrolPosition + Vector3.up * 5, Color.green, 100f);
            Debug.DrawLine((Vector3)LeftChasePosition, (Vector3)LeftChasePosition + Vector3.up * 5, Color.red, 100f);
            Debug.DrawLine((Vector3)RightChaselPosition, (Vector3)RightChaselPosition + Vector3.up * 5, Color.red, 100f);
            */

            //##
            isIdling = false;

            //##
            isPatroling = false;

            //##
            isChasing = false;

            //##
            isAttacking = false;
            isTracing = false;
            attackIndex = 0;
            
            //##
            canPain = true;
            isPaining = false;
            isRequestingPain = false;

        }

        #endregion

        #region EVENT ACTION
        
        private void Self_OnTakeDamage(object sender, HitData e)
        {
            if (!isDead)
            {
                isRequestingPain = true;
            }
        }

        protected virtual void StateMachine_OnChangeState()
        {
            previousState = currentState;
        }

        #endregion

        #region STATE MACHINE

        protected virtual void AddTransition(IState from, IState to, IPredicate condition)
        {
            stateMachine.AddTransition(from, to, condition);
        }

        protected virtual void AddAnyTransition(IState to, IPredicate condition)
        {
            stateMachine.AddAnyTransition(to, condition);
        }

        //# TRANSITION STATE CONDITION FUNCTION
        protected virtual bool CanIdleToPatrol()
        {
            return !isDead && !isIdling && !isPatroling;
        }
        
        protected virtual bool CanPatrolToIdle()
        {
            return !isDead && !isPatroling && !isIdling;
        }
        
        protected virtual bool CanChaseToPatrol()
        {
            return !isDead && !isChasing && !isPatroling && !PlayerInChaseRange();
        }
        
        protected virtual bool CanAttackToIdle()
        {
            return !isDead && !isAttacking && !isIdling && PlayerInAttackRange();
        }
        
        protected virtual bool CanAttackToPatrol()
        {
            return !isDead && !isAttacking && !isIdling && !PlayerInChaseRange();
        }
        
        protected virtual bool CanPainToPatrol()
        {
            return !isDead && !isPaining && !isPatroling && !PlayerInChaseRange();
        }
        
        //## ANY TRANSITION
        protected virtual bool CanAnyToChase()
        {
            return !isDead && !isPaining && !isAttacking && !isChasing && PlayerInChaseRange() && !PlayerInAttackRange();
        }
        
        protected virtual bool CanAnyToAttack()
        {
            return !isDead && !isPaining && !isAttacking && PlayerInAttackRange() && currentState is not BaseEnemyState.State.Attack;
        }
        
        protected virtual bool CanAnyToPain()
        {
            return !isDead && canPain && isRequestingPain && !isPaining && currentState is not BaseEnemyState.State.Pain;
        }
        
        protected virtual bool CanAnyToDead()
        {
            return isDead && currentState is not BaseEnemyState.State.Dead;
        }

        //# IDLE
        public virtual void Begin_IdleState()
        {
            currentState = BaseEnemyState.State.Idle;
            isIdling = true;
        }

        public virtual void Finish_IdleState()
        {
            isIdling = false;
        } 
        
        //# PATROL
        public virtual void Begin_PatrolState()
        {
            currentState = BaseEnemyState.State.Patrol;
            isPatroling = true;
        }

        public virtual void Finish_PatrolState()
        {
            isPatroling = false;
        }

        //# CHASE
        public virtual void Begin_ChaseState()
        {
            currentState = BaseEnemyState.State.Chase;
            isChasing = true;
        }

        public virtual void Finish_ChaseState()
        {
            isChasing = false;
        }

        //# ATTACK
        public virtual void Begin_AttackState()
        {
            currentState = BaseEnemyState.State.Attack;
            isAttacking = true;
        }

        public virtual void Finish_AttackState()
        {
            isAttacking = false;
        }
        
        //# PAIN
        public virtual void Begin_PainState()
        {
            currentState = BaseEnemyState.State.Pain;
            isRequestingPain = false;
            isPaining = true;
            canPain = false;
            
            UT_PainRecovery().Forget();
        }

        public virtual void Finish_PainState()
        {
            isRequestingPain = false;
            isPaining = false;
        }
        
        //# DEAD
        public virtual void Begin_DeadState()
        {
            currentState = BaseEnemyState.State.Dead;
            // isDead = true;
        }

        public virtual void Finish_DeadState()
        {
            // isDead = false;
        }

        #endregion

        #region MAIN

        public virtual bool PlayerInChaseRange()
        {
            return targetTransform.position.x > PatrolPositionLeft.x
                && targetTransform.position.x < PatrolPositionRight.x
                && targetTransform.position.y > ChasePositionBelow.y
                && targetTransform.position.y < ChasePositionAbove.y;
        }
            
        public virtual bool PlayerInAttackRange()
        {
            int coefficient = isFlippingLeft ? -1 : 1;
            coefficient = isBeginFlipLeft ? coefficient * -1 : coefficient;

            Vector3 boxCenter =
                new Vector2(MyTransform.position.x + Stats.AttackDataArray[attackIndex].AttackOffsetWidth * coefficient,
                    MyTransform.position.y + Stats.AttackDataArray[attackIndex].AttackOffsetHeight);
            Vector3 boxSize = new Vector2(Stats.AttackDataArray[attackIndex].AttackRadiusWidth,
                Stats.AttackDataArray[attackIndex].AttackRangeHeight);
            RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.right, 0f, Stats.AttackLayer);
            return hit.collider != null;
        }

        protected virtual async UniTask UT_PainRecovery()
        {
            await UniTask.WaitForSeconds(painRecoveryTime);
            canPain = true;
        }
        
        #endregion

        #region SETTER/GETTER

        public virtual void SetIsFlippingLeft(bool isFlippingLeft)
        {
            this.isFlippingLeft = isFlippingLeft;

            HandleFlip();

            //##

            void HandleFlip()
            {
                int yAxisRotateLeft = isBeginFlipLeft ? Y_AXIS_0 : Y_AXIS_180;
                int yAxisRotateRight = isBeginFlipLeft ? Y_AXIS_180 : Y_AXIS_0;
                    
                if (this.isFlippingLeft)
                {
                    MyTransform.rotation = Quaternion.Euler(0, yAxisRotateLeft, 0);
                }
                else
                {
                    MyTransform.rotation = Quaternion.Euler(0, yAxisRotateRight, 0);
                }
            }

        }

        #endregion

        #region IATTACKABLE

        public virtual void IBeginAttack()
        {
            
        }

        public virtual void IAttacking()
        {
            
        }

        public virtual void IFinishAttack()
        {
            attackIndex = (attackIndex + 1) % Stats.AttackDataArray.Length;
        }

        public virtual void IBeginTrace()
        {
            isTracing = true;
            _listHasBeenHit.Clear();
        }

        public virtual void ITracing()
        {
            int coefficient = isFlippingLeft ? -1 : 1;
            coefficient = isBeginFlipLeft ? coefficient * -1 : coefficient;
            Vector3 boxCenter = new Vector2(MyTransform.position.x + Stats.AttackDataArray[attackIndex].AttackOffsetWidth * coefficient, MyTransform.position.y + Stats.AttackDataArray[attackIndex].AttackOffsetHeight);
            Vector3 boxSize = new Vector2(Stats.AttackDataArray[attackIndex].AttackRadiusWidth, Stats.AttackDataArray[attackIndex].AttackRangeHeight);
            RaycastHit2D[] hitArray = Physics2D.BoxCastAll(boxCenter, boxSize, 0f, Vector2.right, 0f, Stats.AttackLayer);

            if (hitArray.Length > 0)
            {
                foreach (RaycastHit2D hit in hitArray)
                {
                    if (hit.transform.TryGetComponent(out HittableObject hittableObject)
                        && hittableObject.CanHit()
                        && !_listHasBeenHit.Contains(hittableObject))
                    {
                        HitData hitData = new HitData(
                            damageCauser: this,
                            isCausedByPlayer: false
                        );
                        
                        hittableObject.IHit(hitData);
                        
                        _listHasBeenHit.Add(hittableObject);
                    }
                }
            }
        }

        public virtual void IFinishTrace()
        {
            isTracing = false;
        }

        #endregion

        #region ANIM EVENT

        protected virtual void AE_BeginTrace()
        {
            IBeginTrace();
        }
    
        protected virtual void AE_FinishTrace()
        {
            IFinishTrace();
        }
        
        protected virtual void AE_FinishAttack()
        {
            IFinishAttack();
        }

        #endregion
        
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            int coefficient = isFlippingLeft ? -1 : 1;
            coefficient = isBeginFlipLeft ? coefficient * -1 : coefficient;
            Vector3 boxCenter = new Vector2(transform.position.x + Stats.AttackDataArray[attackIndex].AttackOffsetWidth * coefficient, transform.position.y + Stats.AttackDataArray[attackIndex].AttackOffsetHeight);
            Vector3 boxSize = new Vector2(Stats.AttackDataArray[attackIndex].AttackRadiusWidth, Stats.AttackDataArray[attackIndex].AttackRangeHeight);
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
        
        #region IDAMAGEABLE

        public override bool ITakeDamage(HitData hitData)
        {
            bool result = base.ITakeDamage(hitData);

            if (healthBar != null)
            {
                healthBar?.Update_Bar(GetHealthPercentage());
            }

            if (!result) return false;

            //## Dead Event
            if (isDead)
            {
                OnAnyDeadEnemy?.Invoke(this, EventArgs.Empty);
            }

            return true;
        }

        #endregion
        
        protected override void HandlePain()
        {
            base.HandlePain();
            
            //##
            if (canPain)
            {
                isRequestingPain = true;
            }
        }
        
        protected override void HandleDead()
        {
            base.HandleDead();

            SpawnRandomItem();
        }
        

        private void SpawnRandomItem()
        {
            SpawnRandomMedicine();
        }
        
        private void SpawnRandomMedicine()
        {
            if (spawnItemData.HasMedicineHealth)
            {
                if (spawnItemData.MedicineHealth_Thirty_QuantityRange != Vector2.zero && Random.value > spawnItemData.MedicineHealth_Thirty_QuantityRange.y)
                {
                    Prefab_Platformer medicineHealth_Thirty =
                        ObjectPool_Platformer.Instance.GetPoolObject(PrefabType_Platformer.ITEM_Medicien_Health_30Per,
                            centerOfBodyTransform.position, Quaternion.identity);
                    medicineHealth_Thirty.Activate();
                }
                
                if (spawnItemData.MedicineHealth_Seventy_QuantityRange != Vector2.zero && Random.value > spawnItemData.MedicineHealth_Seventy_QuantityRange.y)
                {
                    Prefab_Platformer medicineHealth_Seventy =
                        ObjectPool_Platformer.Instance.GetPoolObject(PrefabType_Platformer.ITEM_Medicien_Health_70Per,
                            centerOfBodyTransform.position, Quaternion.identity);
                    medicineHealth_Seventy.Activate();
                }
                
                if (spawnItemData.MedicineHealth_Hundred_QuantityRange != Vector2.zero && Random.value > spawnItemData.MedicineHealth_Hundred_QuantityRange.y)
                {
                    Prefab_Platformer medicineHealth_Hundred =
                        ObjectPool_Platformer.Instance.GetPoolObject(PrefabType_Platformer.ITEM_Medicien_Health_100Per,
                            centerOfBodyTransform.position, Quaternion.identity);
                    medicineHealth_Hundred.Activate();
                }

            }
            
            if (spawnItemData.HasMedicineEnergy)
            {
                if (spawnItemData.MedicineEnergy_Thirty_QuantityRange != Vector2.zero && Random.value > spawnItemData.MedicineEnergy_Thirty_QuantityRange.y)
                {
                    Prefab_Platformer medicineEnergy_Thirty =
                        ObjectPool_Platformer.Instance.GetPoolObject(PrefabType_Platformer.ITEM_Medicien_Energy_30Per,
                            centerOfBodyTransform.position, Quaternion.identity);
                    medicineEnergy_Thirty.Activate();
                }
                
                if (spawnItemData.MedicineEnergy_Seventy_QuantityRange != Vector2.zero && Random.value > spawnItemData.MedicineEnergy_Seventy_QuantityRange.y)
                {
                    Prefab_Platformer medicineEnergy_Seventy =
                        ObjectPool_Platformer.Instance.GetPoolObject(PrefabType_Platformer.ITEM_Medicien_Energy_70Per,
                            centerOfBodyTransform.position, Quaternion.identity);
                    medicineEnergy_Seventy.Activate();
                }
                
                if (spawnItemData.MedicineEnergy_Hundred_QuantityRange != Vector2.zero && Random.value > spawnItemData.MedicineEnergy_Hundred_QuantityRange.y)
                {
                    Prefab_Platformer medicineEnergy_Hundred =
                        ObjectPool_Platformer.Instance.GetPoolObject(PrefabType_Platformer.ITEM_Medicien_Energy_100Per,
                            centerOfBodyTransform.position, Quaternion.identity);
                    medicineEnergy_Hundred.Activate();
                }

            }
        }

        
    }
}

