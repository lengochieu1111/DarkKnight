using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Manager;
using HIEU_NL.ObjectPool.Audio;
using HIEU_NL.Puzzle.Script.Entity;
using HIEU_NL.Puzzle.Script.Entity.Player;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Character
{
    public class Key_Puzzle : StaticEntity_Puzzle
    {

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

            if (senderEntity is Player_Puzzle)
            {
                PlayKeyPickUpSound();
                PlayLockPickUpEffect();
                
                SendInteract(senderEntity, Vector2.zero);
            }
        }

        #endregion
        
        private void PlayKeyPickUpSound()
        {
            SoundType soundType = SoundType.Key_PickUp;
            ((SoundManager)SoundManager.Instance).PlaySound(soundType);
        }
        
        private void PlayLockPickUpEffect()
        {
            Prefab_Puzzle impactPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.EFFECT_Lock_PickUp, position:transform.position, rotation: Quaternion.identity);
            impactPrefab.Activate();
        }

    }

}
