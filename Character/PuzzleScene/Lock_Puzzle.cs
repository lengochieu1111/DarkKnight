using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Manager;
using HIEU_NL.ObjectPool.Audio;
using HIEU_NL.Puzzle.Script.Entity.Player;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Character
{
    public class Lock_Puzzle : StaticEntity_Puzzle
    {
        protected override void ResetValues()
        {
            base.ResetValues();

            canMoveInto = false;
        }

        #region Interaction

        public override void SendInteract(BaseEntity_Puzzle receverEntity, Vector2 senderDirection)
        {
            base.SendInteract(receverEntity, senderDirection);

            receverEntity.ReceiverInteract(this, senderDirection);

            DestroySelf();

        }

        public override void ReceiverInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
        {
            base.ReceiverInteract(senderEntity, receverDirection);

            if (senderEntity.TryGetComponent(out Player_Puzzle player))
            {
                if (player.IsHavingKey())
                {
                    canMoveInto = true;

                    PlayDoorOpenSound();

                    SendInteract(senderEntity, Vector2.zero);
                }
                else
                {
                    PlaySmallImpactEffect();
                    PlayDoorKickSound();
                }
            }
        }

        #endregion
        
        private void PlayDoorKickSound()
        {
            SoundType soundType;
            switch (Random.Range(0, 3))
            {
                case 0:
                    soundType = SoundType.Door_Kick_1;
                    break;
                case 1:
                    soundType = SoundType.Door_Kick_2;
                    break;
                default:
                    soundType = SoundType.Door_Kick_3;
                    break;
            }
            ((SoundManager)SoundManager.Instance).PlaySound(soundType);
        }
        
        private void PlayDoorOpenSound()
        {
            SoundType soundType = SoundType.Door_Open;
            ((SoundManager)SoundManager.Instance).PlaySound(soundType);
        }

    }

}
