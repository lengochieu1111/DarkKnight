using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity
{
    public class StaticEntity_Puzzle : BaseEntity_Puzzle
    {

        protected override void ResetValues()
        {
            base.ResetValues();

            this.canMoveInto = true;
        }

        #region Interaction

        public override void ReceiverInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection) { }

        public override void SendInteract(BaseEntity_Puzzle receverEntity, Vector2 senderDirection) { }

        #endregion

    }

}
