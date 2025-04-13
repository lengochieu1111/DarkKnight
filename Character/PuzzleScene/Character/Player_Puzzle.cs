using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Manager;
using HIEU_NL.ObjectPool.Audio;
using HIEU_NL.Puzzle.Script.Entity.Character;
using HIEU_NL.Puzzle.Script.Game;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Player
{
    public class Player_Puzzle : DynamicEntity_Puzzle
    {
        public static event EventHandler<int> OnPlayerActed;
        public static event EventHandler OnPlayerPause;
        public static event EventHandler OnPlayerWin;

        private PuzzlePLayerInputActions _inputActions;

        [Header("Animator")] 
        [SerializeField] private Animator _animator;
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

        protected override void ActionFinish()
        {
            base.ActionFinish();

            bool isWin = false;
            // check interact with space portal
            if (TryActionPointCompletedInteractWithStaticEntities(out List<StaticEntity_Puzzle> staticEntitis)
                && staticEntitis.Exists(x => x is SpacePortal_Puzzle))
            {
                isWin = true;
                _inputActions.Player.Disable();
                OnPlayerWin?.Invoke(this, EventArgs.Empty);
            }

            if (!isWin)
            {
                OnPlayerActed?.Invoke(this, actionUsageCounter);
            }

        }

        #endregion

        #region Move

        protected override void RequestAction(Vector2 moveDirection)
        {
            if (GameMode_Puzzle.Instance.GetGameActionCounter() > 0)
            {
                if (CanAction())
                {
                    Flip(moveDirection);
                }

                base.RequestAction(moveDirection);
            }
            else
            {
                HandleDead();
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

        public override void ReceiverInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
        {
            base.ReceiverInteract(senderEntity, receverDirection);

            if (senderEntity is Key_Puzzle)
            {
                _isHavingKey = true;
            }
            else if (senderEntity is Lock_Puzzle)
            {
                // _isHavingKey = false;

                HandleMove();
            }
        }
        
        protected override void HandleInteractWithStaticEntity(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        {
            base.HandleInteractWithStaticEntity(receverEntity, senderDirection);
            
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
            
            if (receverEntity is Lock_Puzzle && !_isHavingKey)
            {
                actionUsageCounter++;
                _animator.CrossFadeInFixedTime(ANIM_HASH_Attack, 0f);
            }

        }

        protected override void HandleInteractWithDynamicEntity(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        {
            base.HandleInteractWithDynamicEntity(receverEntity, senderDirection);
            
            _animator.CrossFadeInFixedTime(ANIM_HASH_Attack, 0f);
        }

        #endregion

        protected override void HandlePain()
        {
            base.HandlePain();

            PlayBloodEffect();
            PlayPainSound();
        }

        protected override void HandleDead()
        {
            base.HandleDead();
            
            _inputActions.Player.Disable();
        }

        #region Effect
        
        private void PlayBloodEffect()
        {
            Prefab_Puzzle impactPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.EFFECT_Blood, position:transform.position, rotation: Quaternion.identity);
            impactPrefab.Activate();
        }
        
        private void PlayPainSound()
        {
            SoundType soundType = UnityEngine.Random.Range(0, 2) == 0 ? SoundType.Spike_Damage_1 : SoundType.Spike_Damage_1;
            ((SoundManager)SoundManager.Instance).PlaySound(soundType);
        }
        
        #endregion
        
        
        /*
         * 
         */
        
        private void RequestPauseGame()
        {
            if (GameMode_Puzzle.Instance.IsGamePlaying())
            {
                OnPlayerPause?.Invoke(this, EventArgs.Empty);
            }
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
