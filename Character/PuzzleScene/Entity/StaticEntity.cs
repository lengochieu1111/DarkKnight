using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity
{
    public class StaticEntity : BaseEntity
    {

        protected override void ResetValues()
        {
            base.ResetValues();

            this.canMoveInto = true;
        }

        #region Interaction

        public override void ReceiveInteract(BaseEntity senderEntity, Vector2 receverDirection)
        {

        }

        public override void SendInteract(BaseEntity receverEntity, Vector2 senderDirection)
        {

        }

        #endregion

    }

}
