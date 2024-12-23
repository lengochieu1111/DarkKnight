using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity
{
    public abstract class BaseEntity : RyoMonoBehaviour
    {
        [SerializeField] protected bool canMoveInto;

        #region Interaction

        public abstract void SendInteract(BaseEntity receverEntity, Vector2 senderDirection);
        public abstract void ReceiveInteract(BaseEntity senderEntity, Vector2 receverDirection);

        #endregion

        /*
         *
         */

        protected void DestroySelf()
        {
            this.gameObject.SetActive(false);
        }

        /*
         *
         */

        public virtual bool CanMoveInto()
        {
            return canMoveInto;
        }

    }

}
