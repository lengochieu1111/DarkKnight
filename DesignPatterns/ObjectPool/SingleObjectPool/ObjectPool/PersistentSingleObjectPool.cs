using System.Collections.Generic;
using HIEU_NL.DesignPatterns.Singleton;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Pool;

namespace HIEU_NL.DesignPatterns.ObjectPool.Single
{
    /// <summary>
    /// PERSISTENT OBJECT POOL DATA
    /// </summary>

    public abstract class PersistentSingleObjectPool<TPrefab, TData, TBuilder> : PersistentSingleton<PersistentSingleObjectPool<TPrefab, TData, TBuilder>>
        where TPrefab : PoolPrefab<TData>
        where TData : PoolData
        where TBuilder : PoolBuilder<TPrefab, TData>
    {
        private IObjectPool<TPrefab> _objectPool;
        private readonly List<TPrefab> _activePoolObjects = new();
        public readonly Dictionary<TData, int> PoolDataCounts = new();

        [SerializeField][Required("PREFAB")] private TPrefab _poolPrefab;
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;

        [SerializeField] private int _maxPoolObjectInstances = 30;

        public TBuilder CreatePoolBuilder(TData data)
        {
            if (!CanSpawnPoolObject(data)) return null;
            TPrefab prefab = GetPoolObject();
            PoolDataCounts[data] = PoolDataCounts.TryGetValue(data, out var count) ? count + 1 : 1;
            return GetNewPoolBuilder(prefab, data);
        }

        protected abstract TBuilder GetNewPoolBuilder(TPrefab prefab, TData data);


        protected override void Start()
        {
            base.Start();

            InitializePool();
        }

        #region GET/RETURN POOL OBJECT

        public bool CanSpawnPoolObject(TData data)
        {
            return !PoolDataCounts.TryGetValue(data, out var count) || count < _maxPoolObjectInstances;
        }

        public TPrefab GetPoolObject()
        {
            return _objectPool.Get();
        }

        public void ReturnToPool(TPrefab prefab)
        {
            _objectPool.Release(prefab);
        }

        #endregion

        #region INITIALIZE POOL

        private void InitializePool()
        {
            _objectPool = new UnityEngine.Pool.ObjectPool<TPrefab>
            (
                CreatePoolObject,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize
            );

        }

        private TPrefab CreatePoolObject()
        {
            var prefab = Instantiate(_poolPrefab);
            prefab.gameObject.SetActive(false);
            return prefab;
        }

        private void OnTakeFromPool(TPrefab prefab)
        {
            prefab.gameObject.SetActive(true);
            _activePoolObjects.Add(prefab);
        }

        private void OnReturnedToPool(TPrefab prefab)
        {
            if (PoolDataCounts.TryGetValue(prefab.Data, out var count))
            {
                PoolDataCounts[prefab.Data] -= count > 0 ? 1 : 0;
            }

            prefab.gameObject.SetActive(false);
            _activePoolObjects.Remove(prefab);
        }

        private void OnDestroyPoolObject(TPrefab prefab)
        {
            Destroy(prefab.gameObject);
        }

        #endregion

    }
}