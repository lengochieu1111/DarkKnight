using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HIEU_NL.Platformer.Script.GameItem
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class BaseGameItem_Platformer : Prefab_Platformer
    {
        public enum EItemType
        {
            Medicine,
            Weapon,
        }
        
        
        //# COLLISION CHECK
        [SerializeField, BoxGroup] private Rigidbody2D rigidbody_;
        [SerializeField, BoxGroup("COLLISION"), Required] protected BoxCollider2D boxCollider;
        [SerializeField, BoxGroup("COLLISION")] protected bool noUseGravity;
        [SerializeField, BoxGroup("COLLISION")] protected bool isGrounded;
        [SerializeField, BoxGroup("COLLISION")] protected LayerMask terrainLayer;
        [SerializeField, BoxGroup("COLLISION")] protected float groundDetectionRayLength = 0.02f;
        [SerializeField, BoxGroup("COLLISION")] protected float gravityCoefficient = 0.5f;
        protected RaycastHit2D groundHit;
        
        
        //# SHOW
        [SerializeField, BoxGroup("Show Effect")] protected float bounceHeight = 1f;
        [SerializeField, BoxGroup("Show Effect")] protected float bounceDuration = 0.4f;
        [SerializeField, BoxGroup("Show Effect")] protected float horizontalOffset = 0.5f;

        #region UNITY CORE

        protected override void OnEnable()
        {
            base.OnEnable();
            
            AppearEffect();

        }

        protected virtual void FixedUpdate()
        {
            //## Collision Chesks
            CollisionChecks();
                
            //## Use gravity
            if (!noUseGravity)
            {
                ApplyGravity();
            }

        }
        
        #endregion

        #region COLLISION CHECK

        protected virtual void CollisionChecks()
        {
            Check_IsGrounded();
        }
        
        protected virtual void Check_IsGrounded()
        {
            Vector3 boxCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
            Vector3 boxSize = new Vector2(boxCollider.bounds.size.x, groundDetectionRayLength);

            groundHit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, groundDetectionRayLength, terrainLayer);
            isGrounded = groundHit.collider != null;

            if (true)
            {
                Color rayColor = Color.red;
                if (isGrounded)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2), boxCenter.y), Vector2.down * groundDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x + boxSize.x / 2), boxCenter.y), Vector2.down * groundDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCenter.x - boxSize.x / 2), boxCenter.y - groundDetectionRayLength), Vector2.right * boxSize.x, rayColor);
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
                verticalVelocity = Physics2D.gravity.y * gravityCoefficient;
            }

            rigidbody_.velocity = new Vector2(horizontalVelocity, verticalVelocity);
        }

        private void AppearEffect()
        {
            Vector3 startPos = transform.position;

            // Random hướng sang trái/phải
            float randomX = Random.Range(-horizontalOffset, horizontalOffset);
            Vector3 targetPos = new Vector3(startPos.x + randomX, startPos.y, startPos.z);

            // Bật lên theo Y rồi hạ xuống
            transform.DOMoveY(startPos.y + bounceHeight, bounceDuration / 2)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    transform.DOMoveY(startPos.y, bounceDuration / 2)
                        .SetEase(Ease.InQuad);
                });

            // Di chuyển ngang
            transform.DOMoveX(targetPos.x, bounceDuration)
                .SetEase(Ease.InOutSine);

            // Optional: scale pop đẹp
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            
        }
        
        public void PickUpSelf()
        {
            Deactivate();
        }
        
        #endregion
        
        
        
    }
}

