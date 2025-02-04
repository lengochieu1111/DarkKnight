using System;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.ObjectPool.Multiple
{
    public class Prefab_Puzzle : RyoMonoBehaviour
    {
        [field: SerializeField] public PrefabType_Puzzle PrefabType { get; private set; }

        public virtual void Activate(Type data = default)
        {
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            ObjectPool_Puzzle.Instance.ReturnToPool(this);
        }

    }

}
