using System;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.DesignPatterns.ObjectPool.Single
{
    public class PoolBuilder<TPrefab, TData>
        where TPrefab : PoolPrefab<TData>
        where TData : PoolData
    {
        protected TPrefab _prefab;
        protected TData _data;
        protected Transform _parent;
        protected Vector3 _position;
        protected Quaternion _rotation;

        public PoolBuilder(TPrefab prefab, TData data, Vector3 position = default, Quaternion rotation = default, Transform parent = default)
        {
            this._prefab = prefab;
            this._data = data;
            this._parent = parent;
            this._position = position;
            this._rotation = rotation;
        }

        //public virtual PoolBuilder<TPrefab, TData> WithPrefab(TPrefab prefab)
        //{
        //    this._prefab = prefab;
        //    return this;
        //}

        //public virtual PoolBuilder<TPrefab, TData> WithData(TData data)
        //{
        //    this._data = data;
        //    return this;
        //}

        public virtual PoolBuilder<TPrefab, TData> WithParent(Transform parent)
        {
            this._parent = parent;
            return this;
        }

        public virtual PoolBuilder<TPrefab, TData> WithPosition(Vector3 position)
        {
            this._position = position;
            return this;
        }

        public virtual PoolBuilder<TPrefab, TData> WithRotation(Quaternion rotation)
        {
            this._rotation = rotation;
            return this;
        }

        public virtual void Activate()
        {
            //##
            SetupPoolObject();

            //##
            ActivatePoolObject();

        }

        //

        protected virtual void SetupPoolObject()
        {
            _prefab.Initialize(_data);
            _prefab.transform.SetParent(_parent);
            _prefab.transform.SetPositionAndRotation(_position, _rotation);
        }

        protected virtual void ActivatePoolObject()
        {
            _prefab.Activate();
        }

    }
}