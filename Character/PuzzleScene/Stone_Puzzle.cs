using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Entity.Player;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity.Character
{
    public class Stone_Puzzle : DynamicEntity_Puzzle
    {

        public override void ReceiveInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection)
        {
            base.ReceiveInteract(senderEntity, receverDirection);

            if (senderEntity is Player_Puzzle)
            {
                this.RequestAction(receverDirection);
            }
        }

    }

}
