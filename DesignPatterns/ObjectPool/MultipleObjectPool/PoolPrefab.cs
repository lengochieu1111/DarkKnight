/*
using System;
using UnityEngine;

namespace HIEU_NL.DesignPatterns.ObjectPool.Multiple
{
    public class PoolPrefab : RyoMonoBehaviour
    {
        [field: SerializeField] public PoolPrefabType PoolPrefabType { get; private set; }

        public virtual void Activate(Type data = default)
        {
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            MultipleObjectPool.Instance.ReturnToPool(this);
        }

    }

}
*/
