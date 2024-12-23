using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HIEU_NL.Puzzle.Script.Entity
{
    public class DynamicEntity : BaseEntity
    {
        [Header("Action")] [SerializeField] protected Transform actionPointTransform;
        [SerializeField] protected Vector2 actionDirection;
        [SerializeField] protected float actionTime = 0.2f;

        [Header("Move")] [SerializeField] protected bool isMoving;
        private Coroutine _movementCoroutine;

        [Header("Interact")] [SerializeField] protected bool isInteracting;
        private Coroutine _interactCoroutine;

        [Header("Dead")] [SerializeField] protected bool isDead;

        #region Setup

        protected override void Start()
        {
            this.actionPointTransform.parent = null;
        }

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (this.actionPointTransform == null)
            {
                this.actionPointTransform = this.transform.Find("ActionPosition");
            }
        }

        protected override void ResetValues()
        {
            base.ResetValues();

            this.canMoveInto = false;
        }

        #endregion

        #region Action

        protected virtual void RequestAction(Vector2 moveDirection)
        {
            if (this.CanAction())
            {
                this.actionDirection = moveDirection;

                BaseEntity baseEntity;

                if (this.TryAction(out baseEntity))
                {
                    this.HandleMove();
                }
                else
                {
                    this.HandleCannotMove();
                }

                if (baseEntity != null && !this.isDead)
                {
                    this.SendInteract(baseEntity, this.actionDirection);
                }
            }
        }

        protected virtual bool TryAction(out BaseEntity baseEntity)
        {
            Vector2 pointToCheck = (Vector2)this.actionPointTransform.position + this.actionDirection;
            Collider2D[] colliders = Physics2D.OverlapPointAll(pointToCheck);

            if (colliders.Length > 0)
            {
                DynamicEntity dynamicEntityInteract = null;
                StaticEntity staticEntityInteract = null;

                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent(out DynamicEntity dynamicEntity))
                    {
                        dynamicEntityInteract = dynamicEntity;
                    }

                    if (collider.TryGetComponent(out StaticEntity staticEntity))
                    {
                        staticEntityInteract = staticEntity;
                    }
                }

                if (dynamicEntityInteract != null)
                {
                    baseEntity = dynamicEntityInteract;
                    return dynamicEntityInteract.CanMoveInto();
                }
                else
                {
                    if (staticEntityInteract != null)
                    {
                        baseEntity = staticEntityInteract;
                        return staticEntityInteract.CanMoveInto();
                    }
                }

                baseEntity = null;

                return false;
            }

            baseEntity = null;

            return true;
        }

        protected bool CanAction()
        {
            return !this.isMoving && !this.isInteracting;
        }

        #endregion

        #region Movement

        protected virtual void HandleMove()
        {
            this.actionPointTransform.position += (Vector3)this.actionDirection;
            this._movementCoroutine = StartCoroutine(this.Movement());
        }

        private IEnumerator Movement()
        {
            this.MoveStarted();

            this.isMoving = true;

            Vector3 startPos = this.transform.position;
            Vector3 endPos = this.actionPointTransform.position;

            float elapsedTime = 0f;

            while (elapsedTime < this.actionTime)
            {
                float t = elapsedTime / this.actionTime;
                this.transform.position = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));

                elapsedTime += Time.deltaTime;

                yield return null;
            }

            this.transform.position = endPos;

            this.isMoving = false;

            this.MoveCompleted();
        }


        protected virtual void MoveStarted()
        {
        }

        protected virtual void MoveCompleted()
        {
        }

        protected virtual void HandleCannotMove()
        {
        }

        #endregion

        #region Interaction

        public override void SendInteract(BaseEntity receverEntity, Vector2 senderDirection)
        {
            this._interactCoroutine = StartCoroutine(this.Interact(receverEntity, senderDirection));
        }

        public override void ReceiveInteract(BaseEntity senderEntity, Vector2 receverDirection)
        {

        }

        private IEnumerator Interact(BaseEntity receverEntity, Vector2 senderDirection)
        {
            this.isInteracting = true;

            receverEntity.ReceiveInteract(this, senderDirection);

            yield return new WaitForSeconds(this.actionTime);

            this.isInteracting = false;
        }

        #endregion

        protected bool TryInteractWithStaticEntities(out List<StaticEntity> staticEntitis)
        {
            bool isInteractive = false;

            Vector2 pointToCheck = this.transform.position;
            Collider2D[] colliders = Physics2D.OverlapPointAll(pointToCheck);

            staticEntitis = new List<StaticEntity>();
            staticEntitis.Clear();

            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent(out StaticEntity staticEntity))
                    {
                        isInteractive = true;
                        staticEntitis.Add(staticEntity);
                    }
                }
            }

            return isInteractive;
        }

    }

}
