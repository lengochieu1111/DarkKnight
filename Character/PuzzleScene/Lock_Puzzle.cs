using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Entity;
using HIEU_NL.Puzzle.Script.Entity.Player;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Character
{
    public class Lock_Puzzle : StaticEntity
    {
        protected override void ResetValues()
        {
            base.ResetValues();

            this.canMoveInto = false;
        }

        #region Interaction

        public override void SendInteract(BaseEntity receverEntity, Vector2 senderDirection)
        {
            base.SendInteract(receverEntity, senderDirection);

            receverEntity.ReceiveInteract(this, senderDirection);

            this.DestroySelf();

        }

        public override void ReceiveInteract(BaseEntity senderEntity, Vector2 receverDirection)
        {
            base.ReceiveInteract(senderEntity, receverDirection);

            if (senderEntity.TryGetComponent(out Player_Puzzle player))
            {
                if (player.IsHavingKey())
                {
                    this.canMoveInto = true;

                    this.SendInteract(senderEntity, Vector2.zero);
                }
            }
        }

        #endregion

    }

}
