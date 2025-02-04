using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HIEU_NL.Puzzle.Script.Entity
{
    public class DynamicEntity_Puzzle : BaseEntity_Puzzle
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
        }

        #endregion

        #region Action

        protected virtual void RequestAction(Vector2 moveDirection)
        {
            if (CanAction())
            {
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
            return !isMoving && !isInteracting;
        }

        #endregion

        #region Movement

        protected virtual void HandleMove()
        {
            actionPointTransform.position += (Vector3)actionDirection;
            _movementCoroutine = StartCoroutine(Movement());
        }

        private IEnumerator Movement()
        {
            MoveStarted();

            isMoving = true;

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

            isMoving = false;

            MoveCompleted();
        }

        private void SlashEffect()
        {
            
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

        public override void SendInteract(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        {
            _interactCoroutine = StartCoroutine(Interact(receverEntity, senderDirection));
        }

        public override void ReceiveInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
        {

        }

        private IEnumerator Interact(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        {
            isInteracting = true;

            receverEntity.ReceiveInteract(this, senderDirection);

            yield return new WaitForSeconds(actionTime);

            isInteracting = false;
        }

        #endregion

        protected bool TryInteractWithStaticEntities(out List<StaticEntity_Puzzle> staticEntitis)
        {
            bool isInteractive = false;

            Vector2 pointToCheck = transform.position;
            Collider2D[] colliders = Physics2D.OverlapPointAll(pointToCheck);

            staticEntitis = new List<StaticEntity_Puzzle>();
            staticEntitis.Clear();

            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent(out StaticEntity_Puzzle staticEntity))
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
