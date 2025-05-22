using System;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.ObjectPool.Multiple
{
    public class Prefab_Platformer : RyoMonoBehaviour
    {
        [field: SerializeField] public PrefabType_Platformer PrefabType { get; private set; }

        public virtual void Activate()
        {
            try
            {
                gameObject.SetActive(true);
            }
            catch (Exception e)
            {
            }
        }

        public virtual void Deactivate()
        {
            ObjectPool_Platformer.Instance.ReturnToPool(this);
        }

    }

}
