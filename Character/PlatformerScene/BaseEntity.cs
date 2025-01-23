using System;
using HIEU_NL.DesignPatterns.ObjectPool.Multiple;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Interface;
using HIEU_NL.Platformer.Script.Map;
using HIEU_NL.Platformer.SO.Entity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace HIEU_NL.Platformer.Script.Entity
{
    public abstract class BaseEntity : PoolPrefab, IDamageable
    {
        public event EventHandler<HitData> OnTakeDamage;
        public event EventHandler OnDead;
        
        [field: SerializeField, BoxGroup("MAP HOUSE")] public EMapHouseType MapHouseType { get; private set; }
        [field: SerializeField, BoxGroup("MAP HOUSE")] public EBotPlacementType BotPlacementType { get; private set; }
        [field: SerializeField, BoxGroup("MAP HOUSE"), ReadOnly] public MapPlacementPoint MapPlacementPoint;

        //# SELF
        public Transform MyTransform => transform;
        [SerializeField, BoxGroup("SELF")] protected bool isBeginFlipLeft;
        [SerializeField, BoxGroup("SELF")] protected bool isFlippingLeft;
        public bool IsFlippingLeft => isFlippingLeft;

        //# BODY
        [SerializeField, BoxGroup("BODY"), Required] protected Transform centerOfBodyTransform;
        
        //# STATS
        [SerializeField, BoxGroup("STATS")] protected EntityStats stats;
        
        //# HEALTH
        [SerializeField, BoxGroup("HEALTH")] protected bool isDead;
        [SerializeField, BoxGroup("HEALTH")] protected int health = 100;
        [SerializeField, BoxGroup("HEALTH")] protected int maxHealth = 100;
        public int Health => health;
        public int MaxHealth => maxHealth;
        public bool IsDead => isDead;

        //# COLLISION CHECK
        [SerializeField, BoxGroup("COLLISION"), Required] protected Rigidbody2D rigidbody_;
        [SerializeField, BoxGroup("COLLISION"), Required] protected CapsuleCollider2D capsuleCollider;
        [SerializeField, BoxGroup("COLLISION"), Required] protected BoxCollider2D boxCollider;
        [SerializeField, BoxGroup("COLLISION")] protected bool noUseGravity;
        [SerializeField, BoxGroup("COLLISION")] protected bool isGrounded;
        [SerializeField, BoxGroup("COLLISION")] protected bool bumpedHead;
        [SerializeField, BoxGroup("COLLISION")] protected bool isTouchingWall;
        protected RaycastHit2D groundHit;
        protected RaycastHit2D headHit;
        protected RaycastHit2D wallHit;
        protected RaycastHit2D lastWallHit;
        public bool IsTouchingWall => isTouchingWall;
            
        #region UNITY CORE
        
        protected virtual void FixedUpdate()
        {
            //## Collision Chesks
            CollisionChesks();
                
            //## Use gravity
            if (!noUseGravity)
            {
                ApplyGravity();
            }

        }
        
        #endregion
        
        protected override void ResetValues()
        {
            base.ResetValues();
            
            //##
            isFlippingLeft = isBeginFlipLeft;

            //## Health
            isDead = false;
            health = stats.MaxHealth;
            maxHealth = stats.MaxHealth;
        }
        
        #region COLLISION CHECK

        protected virtual void CollisionChesks()
        {
            Check_IsGrounded();
            Check_BumpedHead();
            Check_IsTouchingWall();
        }
        
        protected virtual void Check_IsGrounded()
        {
            Vector3 boxCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
            Vector3 boxSize = new Vector2(boxCollider.bounds.size.x, stats.GroundDetectionRayLength);

            groundHit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, stats.GroundDetectionRayLength, stats.TerrainLayer);
            isGrounded = groundHit.collider != null;

            if (stats.DebugShowIsGroundedBox)
            {
                Color rayColor = Color.red;
                if (isGrounded)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2), boxCenter.y), Vector2.down * stats.GroundDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x + boxSize.x / 2), boxCenter.y), Vector2.down * stats.GroundDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2), boxCenter.y - stats.GroundDetectionRayLength), Vector2.right * boxSize.x, rayColor);
            }
            
        }
        
        protected virtual void Check_BumpedHead()
        {
            Vector3 boxCenter = new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y);
            Vector3 boxSize = new Vector2(capsuleCollider.bounds.size.x * stats.HeadWidth, stats.HeadDetectionRayLength);

            headHit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.up, stats.HeadDetectionRayLength, stats.TerrainLayer);
            bumpedHead = headHit.collider != null;

            if (stats.DebugShowHeadBumpBox)
            {
                Color rayColor = Color.red;
                if (bumpedHead)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2 * stats.HeadWidth), boxCenter.y), Vector2.up * stats.HeadDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x + (boxSize.x / 2) * stats.HeadWidth), boxCenter.y), Vector2.up * stats.HeadDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2 * stats.HeadWidth), boxCenter.y + stats.HeadDetectionRayLength), Vector2.right * boxSize.x * stats.HeadWidth, rayColor);

            }
            
        }

        protected virtual void Check_IsTouchingWall()
        {
            float originEndPoint = 0f;
            if (isFlippingLeft)
            {
                originEndPoint = capsuleCollider.bounds.min.x;
            }
            else
            {
                originEndPoint = capsuleCollider.bounds.max.x;
            }

            float adjustedHeight = capsuleCollider.size.y * stats.WallDetectionRayHeightMultiplier;

            Vector2 boxCastOrigin = new Vector2(originEndPoint, capsuleCollider.bounds.center.y);
            Vector2 boxCastSize = new Vector2(stats.WallDetectionRayLength, adjustedHeight);

            wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, MyTransform.right, stats.WallDetectionRayLength, stats.TerrainLayer);

            if (wallHit.collider != null)
            {
                lastWallHit = wallHit;
                isTouchingWall = true;
            }
            else
            {
                isTouchingWall = false;
            }

            if (stats.DebugShowWallHitBox)
            {
                Color rayColor = Color.red;
                if (isTouchingWall)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
                Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
                Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
                Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

                Debug.DrawRay(boxBottomLeft, boxBottomRight, rayColor);
                Debug.DrawRay(boxBottomRight, boxTopRight, rayColor);
                Debug.DrawRay(boxTopRight, boxTopLeft, rayColor);
                Debug.DrawRay(boxTopLeft, boxBottomLeft, rayColor);
            }

        }         

        #endregion

        #region MAIN

        protected virtual void ApplyGravity()
        {
            float verticalVelocity = 0f;
            float horizontalVelocity = 0f;
            if (isGrounded)
            {
                verticalVelocity = -0.01f;
            }
            else
            {
                verticalVelocity = Physics2D.gravity.y * stats.GravityCoefficient;
            }

            rigidbody_.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }
        
        #endregion

        #region IDAMAGEABLE

        public virtual bool ITakeDamage(HitData hitData)
        {
            if (isDead) return false;
            
            health = Mathf.Max(0, health - hitData.Damage);
            isDead = health <= 0;

            PoolPrefab bloodPool = MultipleObjectPool.Instance.GetPoolObject(PoolPrefabType.EFFECT_Blood_1_PLATFORMER, parent:centerOfBodyTransform);
            bloodPool.Activate();
            
            //## Hit Event
            OnTakeDamage?.Invoke(this, hitData);

            //## Dead Event
            if (isDead)
            {
                OnDead?.Invoke(this, EventArgs.Empty);
            }

            return true;
        }

        #endregion

        #region SETTER/GETTER

        public float GetHealthPercentage() { return health * 1f / maxHealth; }

        public void SetMapPlacementPoint(MapPlacementPoint mapPlacementPoint)
        {
            MapPlacementPoint = mapPlacementPoint;
        }

        #endregion



    }
}

