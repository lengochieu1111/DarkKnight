using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Entity;
using HIEU_NL.Puzzle.Script.Entity.Player;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Character
{
    public class Key_Puzzle : StaticEntity
    {

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

            if (senderEntity is Player_Puzzle)
            {
                this.SendInteract(senderEntity, Vector2.zero);
            }
        }

        #endregion

    }

}
