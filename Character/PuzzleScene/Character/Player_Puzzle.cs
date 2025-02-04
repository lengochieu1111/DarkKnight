using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Entity.Character;
using HIEU_NL.Puzzle.Script.Entity.Enemy;
using HIEU_NL.Puzzle.Script.Game;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Player
{
    public class Player_Puzzle : DynamicEntity_Puzzle
    {
        public static event EventHandler OnPlayerActed;
        public static event EventHandler OnPlayerPause;
        public static event EventHandler OnPlayerLoses;

        private PuzzlePLayerInputActions _inputActions;

        [Header("Animator")] [SerializeField] private Animator _animator;
        // [ShowNonSerializedField] private bool _isFlippingLeft;
        [ShowNonSerializedField] private bool _isHavingKey;

        protected override void Awake()
        {
            base.Awake();

            _inputActions = new PuzzlePLayerInputActions();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _inputActions.Player.Enable();

            _inputActions.Player.Action.started += Player_Action_started;
            _inputActions.Player.Pause.started += Player_Pause_started;
        }

        protected override void OnDisable()
        {
            base.OnEnable();

            _inputActions.Player.Disable();
        }

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
        }

        #region Input Action

        private void Player_Action_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Vector2 inputDirection = _inputActions.Player.Action.ReadValue<Vector2>();
            RequestAction(inputDirection);
        }

        private void Player_Pause_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            RequestPauseGame();
        }

        #endregion

        /*
         *
         */

        #region Move

        protected override void RequestAction(Vector2 moveDirection)
        {
            if (GameMode_Puzzle.Instance.GetGameActionCounter() > 0)
            {
                if (CanAction())
                {
                    Flip(moveDirection);
                }

                ActionEffect();

                base.RequestAction(moveDirection);
            }
            else if (GameMode_Puzzle.Instance.GetGameActionCounter() == 0)
            {
                OnPlayerLoses?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void MoveStarted()
        {
            base.MoveStarted();

            OnPlayerActed?.Invoke(this, EventArgs.Empty);
        }

        protected override void MoveCompleted()
        {
            base.MoveCompleted();

            // check interact with trap, space portal
            if (TryInteractWithStaticEntities(out List<StaticEntity_Puzzle> staticEntitis))
            {
                foreach (StaticEntity_Puzzle entity in staticEntitis)
                {
                    if (entity is Trap_Puzzle)
                    {
                        HandleHit();

                        return;
                    }
                    else if (entity is SpacePortal_Puzzle)
                    {
                        Debug.Log("Interact Space Portal");

                        return;
                    }
                }
            }

        }

        protected override void HandleCannotMove()
        {
            base.HandleCannotMove();

            // check interact with trap
            if (TryInteractWithStaticEntities(out List<StaticEntity_Puzzle> staticEntitis))
            {
                foreach (StaticEntity_Puzzle entity in staticEntitis)
                {
                    if (entity is Trap_Puzzle)
                    {
                        HandleHit();

                        return;
                    }
                }
            }

        }

        private void Flip(Vector2 moveDirection)
        {
            if (moveDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            else if (moveDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        #endregion

        #region Interact

        public override void SendInteract(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        {
            base.SendInteract(receverEntity, senderDirection);

            if (receverEntity is Stone_Puzzle)
            {
                OnPlayerActed?.Invoke(this, EventArgs.Empty);
            }
            else if (receverEntity is Enemy_Puzzle)
            {
                OnPlayerActed?.Invoke(this, EventArgs.Empty);
            }

        }

        public override void ReceiveInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
        {
            base.ReceiveInteract(senderEntity, receverDirection);

            if (senderEntity is Key_Puzzle)
            {
                _isHavingKey = true;
            }
            else if (senderEntity is Lock_Puzzle)
            {
                _isHavingKey = false;

                HandleMove();
            }
        }

        #endregion

        private void HandleHit()
        {
            PlayAnimState_Hit();

            OnPlayerActed?.Invoke(this, EventArgs.Empty);
        }

        private void PlayAnimState_Hit()
        {
            // _animator.SetTrigger(ParameterExtensions.Animation.ANIM_PARAMETER_HIT);
        }

        private void RequestPauseGame()
        {
            if (GameMode_Puzzle.Instance.IsGamePlaying())
            {
                OnPlayerPause?.Invoke(this, EventArgs.Empty);
            }
        }
        
        private void ActionEffect()
        {
            /*Vector2 actionPosition = (Vector2)actionPointTransform.position + actionDirection;
            Vector2 attackDirection = actionPosition - (Vector2)transform.position;

            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            Quaternion actionRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (_isFlippingLeft)
            {
                actionRotation = Quaternion.Euler(-180, actionRotation.eulerAngles.y, actionRotation.eulerAngles.z);
            }
            
            Prefab_Puzzle playerPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.EFFECT_Slash_1_Normal, position:actionPosition, rotation: actionRotation, parent: actionPointTransform);
            playerPrefab.Activate();*/

            _animator.CrossFadeInFixedTime(ANIM_HASH_Attack, 0f);
        }

        /*
         *
         */

        public bool IsHavingKey()
        {
            return _isHavingKey;
        }

    }

}
