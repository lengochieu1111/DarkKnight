using HIEU_NL.DesignPatterns.Singleton;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace HIEU_NL.DesignPatterns.ObjectPool.Multiple
{
    public class MultipleObjectPool : Singleton<MultipleObjectPool> 
    {
        public readonly Dictionary<PoolPrefabType, IObjectPool<PoolPrefab>> _objectPoolAll = new();
        private readonly Dictionary<PoolPrefabType, List<PoolPrefab>>  _activePoolObjectAll = new();

        [SerializeField] private PoolPrefabAssetListSO _poolPrefabAssetListSO;
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;

        private PoolPrefab _currentPoolPrefab;
        private PoolPrefabType _currentPoolPrefabType;
        private IObjectPool<PoolPrefab> _currentObjectPool;

        #region GET/RETURN POOL OBJECT

        public PoolPrefab GetPoolObject(PoolPrefabType poolPrefabType, Vector3 position = default,
            Quaternion rotation = default, Transform parent = default)
        {
            _currentPoolPrefabType = poolPrefabType;

            //## Get Pool Prefab
            _currentPoolPrefab = GetPoolPrefab(poolPrefabType);
            if (_currentPoolPrefab is null)
            {
                Debug.Assert(_currentPoolPrefab is not null, $"Pool prefab : {nameof(_currentPoolPrefab)} not exist.");
                return null;
            }

            //## Get Object Pool
            _currentObjectPool = GetObjectPool(poolPrefabType);
            if (_currentObjectPool is null)
            {
                //## Create Object Pool
                CreateObjectPool(poolPrefabType);
            }

            //## Get Pool Object
            _currentPoolPrefab = _currentObjectPool?.Get();
            if (_currentPoolPrefab is null)
            {
                Debug.Assert(_currentPoolPrefab is not null, $"Pool prefab : {nameof(_currentPoolPrefab)} is null");
                return null;
            }
            
            //## Setup Pool Prefab
            parent = parent == null ? transform : parent;
            _currentPoolPrefab.transform.SetParent(parent);
            _currentPoolPrefab.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (position != default)
            {
                _currentPoolPrefab.transform.SetPositionAndRotation(position, _currentPoolPrefab.transform.rotation);
            }

            if (rotation != default)
            {
                _currentPoolPrefab.transform.SetPositionAndRotation(_currentPoolPrefab.transform.position, rotation);
            }

            return _currentPoolPrefab;

        }

        public void ReturnToPool(PoolPrefab poolObject)
        {
            _currentPoolPrefabType = poolObject.PoolPrefabType;
            
            poolObject.transform.SetParent(transform);
            
            //## Get Object Pool
            _currentObjectPool = GetObjectPool(poolObject.PoolPrefabType);
            _currentObjectPool?.Release(poolObject);
        }

        #endregion

        #region OTHERS

        IObjectPool<PoolPrefab> GetObjectPool(PoolPrefabType poolPrefabType)
        {
            if (_objectPoolAll.TryGetValue(poolPrefabType, out var objectPool))
                return objectPool;

            return null;
        }

        List<PoolPrefab> GetActivePoolObjectList(PoolPrefabType poolPrefabType)
        {
            if (_activePoolObjectAll.TryGetValue(poolPrefabType, out var activePoolObjectList))
                return activePoolObjectList;

            return null;
        }

        PoolPrefab GetPoolPrefab(PoolPrefabType poolPrefabType)
        {
            foreach (PoolPrefabAsset poolPrefabAsset in _poolPrefabAssetListSO.PoolPrefabAssetList)
            {
                if (poolPrefabAsset.PrefabType == poolPrefabType)
                    return poolPrefabAsset.PoolPrefab;
            }

            return null;
        }

        #endregion

        #region INITIALIZE POOL

        void CreateObjectPool(PoolPrefabType poolPrefabType)
        {
            _currentObjectPool = new UnityEngine.Pool.ObjectPool<PoolPrefab>
            (
                CreatePoolObject,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize
            );
            
            _objectPoolAll.Add(poolPrefabType, _currentObjectPool);

        }

        PoolPrefab CreatePoolObject()
        {
            var prefab = Instantiate(_currentPoolPrefab);
            prefab.gameObject.SetActive(false);
            
            //## Create new ActivePoolObjectList
            if (!_activePoolObjectAll.TryGetValue(_currentPoolPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList = new List<PoolPrefab>();
                _activePoolObjectAll.Add(_currentPoolPrefabType, activePoolObjectList);
            }
            
            return prefab;
        }

        void OnTakeFromPool(PoolPrefab prefab)
        {
            //## Add 
            if (_activePoolObjectAll.TryGetValue(_currentPoolPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList.Add(prefab);
            }
        }

        void OnReturnedToPool(PoolPrefab prefab)
        {
            prefab.gameObject.SetActive(false);

            //## Remove 
            if (_activePoolObjectAll.TryGetValue(_currentPoolPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList.Remove(prefab);
            }
        }

        void OnDestroyPoolObject(PoolPrefab prefab)
        {
            Destroy(prefab.gameObject);
        }

        #endregion

    }
}


