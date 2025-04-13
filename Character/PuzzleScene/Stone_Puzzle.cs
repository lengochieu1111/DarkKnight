using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Manager;
using HIEU_NL.ObjectPool.Audio;
using HIEU_NL.Puzzle.Script.Entity.Player;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Character
{
    public class Stone_Puzzle : DynamicEntity_Puzzle
    {

        public override void ReceiverInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
        {
            base.ReceiverInteract(senderEntity, receverDirection);

            if (senderEntity is Player_Puzzle)
            {
                RequestAction(receverDirection);
                PlayStoneKickSound();
            }
        }

        protected override void HandleMove()
        {
            base.HandleMove();
            
            PlayBigImpactEffect();
        }

        protected override void HandleCannotMove()
        {
            base.HandleCannotMove();
            
            PlaySmallImpactEffect();
        }
        
        private void PlayStoneKickSound()
        {
            SoundType soundType = SoundType.Stone_Kick_1;
            int indexRandom = UnityEngine.Random.Range(0, 3);
            switch (indexRandom)
            {
                case 0:
                    soundType = SoundType.Stone_Kick_1;
                    break;
                case 1:
                    soundType = SoundType.Stone_Kick_2;
                    break;
                case 2:
                    soundType = SoundType.Stone_Kick_3;
                    break;
            }
            ((SoundManager)SoundManager.Instance).PlaySound(soundType);
        }
    }

}
