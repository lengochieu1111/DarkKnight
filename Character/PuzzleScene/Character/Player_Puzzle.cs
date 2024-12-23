using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Entity.Character;
using HIEU_NL.Puzzle.Script.Entity.Enemy;
using HIEU_NL.Utilities;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Player
{
    public class Player_Puzzle : DynamicEntity
    {
        public static event EventHandler OnPlayerActed;
        public static event EventHandler OnPlayerPause;
        public static event EventHandler OnPlayerLoses;

        [Header("Input")] private PuzzlePLayerInputActions _inputActions;

        [Header("Animator")] [SerializeField] private Animator _animator;
        [SerializeField] private bool _isHavingKey;

        protected override void Awake()
        {
            base.Awake();

            this._inputActions = new PuzzlePLayerInputActions();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            this._inputActions.Player.Enable();

            this._inputActions.Player.Action.started += Player_Action_started;
            this._inputActions.Player.Pause.started += Player_Pause_started;
        }

        protected override void OnDisable()
        {
            base.OnEnable();

            this._inputActions.Player.Disable();
        }

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (this._animator == null)
            {
                this._animator = this.GetComponentInChildren<Animator>();
            }
        }

        #region Input Action

        private void Player_Action_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            Vector2 inputDirection = this._inputActions.Player.Action.ReadValue<Vector2>();
            this.RequestAction(inputDirection);
        }

        private void Player_Pause_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            this.RequestPauseGame();
        }

        #endregion

        /*
         *
         */

        #region Move

        protected override void RequestAction(Vector2 moveDirection)
        {
            if (GameManager_Puzzle.Instance.GetGameActionCounter() > 0)
            {
                if (this.CanAction())
                {
                    this.Flip(moveDirection);
                }

                base.RequestAction(moveDirection);
            }
            else if (GameManager_Puzzle.Instance.GetGameActionCounter() == 0)
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
            if (this.TryInteractWithStaticEntities(out List<StaticEntity> staticEntitis))
            {
                foreach (StaticEntity entity in staticEntitis)
                {
                    if (entity is Trap_Puzzle)
                    {
                        this.HandleHit();

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
            if (this.TryInteractWithStaticEntities(out List<StaticEntity> staticEntitis))
            {
                foreach (StaticEntity entity in staticEntitis)
                {
                    if (entity is Trap_Puzzle)
                    {
                        this.HandleHit();

                        return;
                    }
                }
            }

        }

        private void Flip(Vector2 moveDirection)
        {
            if (moveDirection.x < 0)
            {
                this.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            else if (moveDirection.x > 0)
            {
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        #endregion

        #region Interact

        public override void SendInteract(BaseEntity receverEntity, Vector2 senderDirection)
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

        public override void ReceiveInteract(BaseEntity senderEntity, Vector2 receverDirection)
        {
            base.ReceiveInteract(senderEntity, receverDirection);

            if (senderEntity is Key_Puzzle)
            {
                this._isHavingKey = true;
            }
            else if (senderEntity is Lock_Puzzle)
            {
                this._isHavingKey = false;

                this.HandleMove();
            }
        }

        #endregion

        private void HandleHit()
        {
            this.PlayAnimState_Hit();

            OnPlayerActed?.Invoke(this, EventArgs.Empty);
        }

        private void PlayAnimState_Hit()
        {
            // this._animator.SetTrigger(ParameterExtensions.Animation.ANIM_PARAMETER_HIT);
        }

        private void RequestPauseGame()
        {
            if (GameManager_Puzzle.Instance.IsGamePlaying())
            {
                OnPlayerPause?.Invoke(this, EventArgs.Empty);
            }
        }

        /*
         *
         */

        public bool IsHavingKey()
        {
            return this._isHavingKey;
        }

    }

}
