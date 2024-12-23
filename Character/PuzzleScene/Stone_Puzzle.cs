using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Entity.Player;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Character
{
    public class Stone_Puzzle : DynamicEntity
    {

        public override void ReceiveInteract(BaseEntity senderEntity, Vector2 receverDirection)
        {
            base.ReceiveInteract(senderEntity, receverDirection);

            if (senderEntity is Player_Puzzle)
            {
                this.RequestAction(receverDirection);
            }
        }

    }

}
