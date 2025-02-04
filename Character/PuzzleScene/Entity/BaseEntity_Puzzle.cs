using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Map;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity
{
    public abstract class BaseEntity_Puzzle : Prefab_Puzzle
    {
        [field: SerializeField] public EMapHouseType_Puzzle MapHouseType { get; private set; }
        [SerializeField] protected bool canMoveInto;

        #region Interaction

        public abstract void SendInteract(BaseEntity_Puzzle receverEntity, Vector2 senderDirection);
        public abstract void ReceiveInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection);

        #endregion

        protected virtual void DestroySelf()
        {
            gameObject.SetActive(false);
        }

        public virtual bool CanMoveInto()
        {
            return canMoveInto;
        }

    }

}
