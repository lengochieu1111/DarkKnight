using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.Map;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using HIEU_NL.Utilities;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Entity
{
    public abstract class BaseEntity_Puzzle : Prefab_Puzzle
    {
        [field: SerializeField] public EMapHouseType_Puzzle MapHouseType { get; private set; }
        [SerializeField] protected bool canMoveInto;

        #region Interaction

        public abstract void SendInteract(BaseEntity_Puzzle receverEntity, Vector2 senderDirection);
        public abstract void ReceiverInteract(BaseEntity_Puzzle senderEntity, Vector2 receverDirection);

        #endregion

        protected virtual void DestroySelf()
        {
            gameObject.SetActive(false);
        }

        public virtual bool CanMoveInto()
        {
            return canMoveInto;
        }

        #region Effect

        protected virtual void PlayBigImpactEffect()
        {
            Prefab_Puzzle impactPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.EFFECT_BigImpact, position:transform.position, rotation: Quaternion.identity);
            impactPrefab.Activate();
        }
        
        protected virtual void PlaySmallImpactEffect()
        {
            Vector3 appearPosition = transform.position;
            Vector2 randomOffset = Random.insideUnitCircle * 0.25f;
            appearPosition += (Vector3)randomOffset;
            Prefab_Puzzle impactPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.EFFECT_SmallImpact, position:appearPosition, rotation: Quaternion.identity);
            impactPrefab.Activate();
        }
                
        #endregion

    }

}
