using System;
using UnityEngine;

namespace HIEU_NL.DesignPatterns.ObjectPool.Single
{
    public abstract class PoolPrefab<TData> : RyoMonoBehaviour where TData : PoolData
    {
        public TData Data { get; private set; }

        //

        public virtual void Initialize(TData data)
        {
            Data = data;
        }

        //

        public abstract void Activate();

        public abstract void Deactivate();

    }

}
