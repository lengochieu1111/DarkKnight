using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Manager;
using HIEU_NL.ObjectPool.Audio;
using HIEU_NL.Puzzle.Script.Entity.Character;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using HIEU_NL.Utilities;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity
{
    public class DynamicEntity_Puzzle : BaseEntity_Puzzle
    {
        [Header("Action")] [SerializeField] protected Transform actionPointTransform;
        [SerializeField] protected Vector2 actionDirection;
        [SerializeField] protected float actionTime = 0.2f;
        protected int actionUsageCounter;

        [Header("Move")] [SerializeField] protected bool isMoving;
        private Coroutine _movementCoroutine;

        [Header("Interact")] [SerializeField] protected bool isInteracting;
        private Coroutine _interactCoroutine;

        [Header("Pain")] [SerializeField] protected bool isPain;
            
        [Header("Dead")] [SerializeField] protected bool isDead;

        #region Setup

        protected override void Start()
        {
            actionPointTransform.parent = null;
        }

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (actionPointTransform == null)
            {
                actionPointTransform = transform.Find("ActionPosition");
            }
        }

        protected override void ResetValues()
        {
            base.ResetValues();

            canMoveInto = false;
            isInteracting = false;
            isMoving = false;
            isDead = false;
            actionUsageCounter = 0;
        }

        #endregion

        #region Action

        protected virtual void RequestAction(Vector2 moveDirection)
        {
            if (CanAction())
            {
                ActionBegin();
                
                actionDirection = moveDirection;

                BaseEntity_Puzzle baseEntity;

                if (TryAction(out baseEntity))
                {
                    HandleMove();
                }
                else
                {
                    HandleCannotMove();
                }
                
                if (baseEntity != null && !isDead)
                {
                    SendInteract(baseEntity, actionDirection);
                }

                ActionFinish();

            }
        }

        protected virtual bool TryAction(out BaseEntity_Puzzle baseEntity)
        {
            Vector2 pointToCheck = (Vector2)actionPointTransform.position + actionDirection;
            Collider2D[] colliders = Physics2D.OverlapPointAll(pointToCheck);

            if (colliders.Length > 0)
            {
                DynamicEntity_Puzzle dynamicEntityInteract = null;
                StaticEntity_Puzzle staticEntityInteract = null;

                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent(out DynamicEntity_Puzzle dynamicEntity))
                    {
                        dynamicEntityInteract = dynamicEntity;
                    }

                    if (collider.TryGetComponent(out StaticEntity_Puzzle staticEntity))
                    {
                        staticEntityInteract = staticEntity;
                    }
                }

                if (dynamicEntityInteract != null)
                {
                    baseEntity = dynamicEntityInteract;
                    return dynamicEntityInteract.CanMoveInto();
                }
                else if (staticEntityInteract != null)
                {
                    baseEntity = staticEntityInteract;
                    return staticEntityInteract.CanMoveInto();
                }
                
                baseEntity = null;

                return false;
            }
            
            baseEntity = null;

            return true;
        }

        protected bool CanAction()
        {
            return !isDead && !isMoving && !isInteracting && !isPain;
        }
        
        protected virtual void ActionBegin()
        {
            actionUsageCounter = 0;
        }
        
        protected virtual void ActionFinish()
        {
            StartCoroutine(IE_ActionFinish());
        }
        
        private IEnumerator IE_ActionFinish()
        {
            yield return new WaitForSeconds(actionTime);

            isMoving = false;
            isPain = false;
            isInteracting = false;
        }

        #endregion

        #region Movement

        protected virtual void HandleMove()
        {
            actionUsageCounter++;
            actionPointTransform.position += (Vector3)actionDirection;
            
            // check interact with trap
            if (TryActionPointCompletedInteractWithStaticEntities(out List<StaticEntity_Puzzle> staticEntitis)
                && staticEntitis.Exists(x => x is Trap_Puzzle))
            {
                isPain = true;
                actionUsageCounter++;
            }
            
            _movementCoroutine = StartCoroutine(IE_Movement());
        }

        private IEnumerator IE_Movement()
        {
            isMoving = true;
            
            MoveStarted();

            Vector3 startPos = transform.position;
            Vector3 endPos = actionPointTransform.position;

            float elapsedTime = 0f;

            while (elapsedTime < actionTime)
            {
                float t = elapsedTime / actionTime;
                transform.position = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            transform.position = endPos;

            MoveCompleted();
            
            isMoving = false;
        }

        protected virtual void MoveStarted()
        {
            PlayDustTrailEffect();
            PlayDashSound();
        }

        protected virtual void MoveCompleted()
        {
            if (isPain)
            {
                HandlePain();
            }
        }

        protected virtual void HandleCannotMove()
        {
            // check interact with trap
            if (TryActionPointCompletedInteractWithStaticEntities(out List<StaticEntity_Puzzle> staticEntitis)
                && staticEntitis.Exists(x => x is Trap_Puzzle))
            {
                isPain = true;
                actionUsageCounter++;
            }
            
            if (isPain)
            {
                HandlePain();
            }
        }

        #endregion

        #region Interaction

        public override void SendInteract(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        {
            if (receverEntity is DynamicEntity_Puzzle)
            {
                actionUsageCounter++;
                HandleInteractWithDynamicEntity(receverEntity, senderDirection);
            }
            else if (receverEntity is StaticEntity_Puzzle)
            {
                HandleInteractWithStaticEntity(receverEntity, senderDirection);
            }
            
            _interactCoroutine = StartCoroutine(IE_Interact(receverEntity, senderDirection));
        }

        public override void ReceiverInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
        {

        }

        private IEnumerator IE_Interact(BaseEntity_Puzzle receiverEntity, Vector2 senderDirection)
        {
            isInteracting = true;

            receiverEntity.ReceiverInteract(this, senderDirection);

            yield return new WaitForSeconds(actionTime);

            isInteracting = false;
        }
        
        protected virtual void HandleInteractWithStaticEntity(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        { }

        protected virtual void HandleInteractWithDynamicEntity(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        { }

        #endregion

        protected bool TryActionPointCompletedInteractWithStaticEntities(out List<StaticEntity_Puzzle> staticEntities)
        {
            staticEntities = new List<StaticEntity_Puzzle>();
            
            Vector2 pointToCheck = actionPointTransform.position;
            Collider2D[] colliders = Physics2D.OverlapPointAll(pointToCheck);
            
            if (colliders.IsNullOrEmpty()) return false;

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out StaticEntity_Puzzle staticEntity))
                {
                    staticEntities.Add(staticEntity);
                }
            }

            return staticEntities.IsValid();
        }


        protected virtual void HandlePain() { }
        
        protected virtual void HandleDead()
        {
            if (isDead) return;
            isDead = true;
        }

        #region Effect

        private void PlayDustTrailEffect()
        {
            Prefab_Puzzle impactPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.EFFECT_DustTrail, position:transform.position, rotation: Quaternion.identity);
            impactPrefab.Activate();
        }
        
        private void PlayDashSound()
        {
            SoundType soundType = UnityEngine.Random.Range(0, 2) == 0 ? SoundType.Dash_1 : SoundType.Dash_2;
            ((SoundManager)SoundManager.Instance).PlaySound(soundType);
        }
        
        #endregion
        
        public bool IsPain()
        {
            return isPain;
        }
        
    }
}

