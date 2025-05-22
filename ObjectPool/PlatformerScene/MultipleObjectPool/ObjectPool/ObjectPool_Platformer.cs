using System;
using HIEU_NL.DesignPatterns.Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace HIEU_NL.Platformer.Script.ObjectPool.Multiple
{
    public class ObjectPool_Platformer : Singleton<ObjectPool_Platformer> 
    {
        public readonly Dictionary<PrefabType_Platformer, IObjectPool<Prefab_Platformer>> _objectPoolAll = new();
        private readonly Dictionary<PrefabType_Platformer, List<Prefab_Platformer>>  _activePoolObjectAll = new();

        [SerializeField] private PrefabAssetListSO_Platformer _prefabAssetListSo;
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;

        private Prefab_Platformer _currentPrefab;
        private PrefabType_Platformer _currentPrefabType;
        private IObjectPool<Prefab_Platformer> _currentObjectPool;

        #region GET/RETURN POOL OBJECT

        public Prefab_Platformer GetPoolObject(PrefabType_Platformer platformerPrefabType, Vector3 position = default,
            Quaternion rotation = default, Transform parent = default)
        {
            _currentPrefabType = platformerPrefabType;

            //## Get Pool Prefab
            _currentPrefab = GetPoolPrefab(platformerPrefabType);
            if (_currentPrefab is null)
            {
                Debug.Assert(_currentPrefab is not null, $"Pool prefab : {nameof(_currentPrefab)} not exist.");
                return null;
            }

            //## Get Object Pool
            _currentObjectPool = GetObjectPool(platformerPrefabType);
            if (_currentObjectPool is null)
            {
                //## Create Object Pool
                CreateObjectPool(platformerPrefabType);
            }

            //## Get Pool Object
            _currentPrefab = _currentObjectPool?.Get();
            if (_currentPrefab is null)
            {
                Debug.Assert(_currentPrefab is not null, $"Pool prefab : {nameof(_currentPrefab)} is null");
                return null;
            }
            
            //## Setup Pool Prefab
            parent = parent == null ? transform : parent;
            _currentPrefab.transform.SetParent(parent);
            _currentPrefab.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            if (position != default)
            {
                _currentPrefab.transform.SetPositionAndRotation(position, _currentPrefab.transform.rotation);
            }

            if (rotation != default)
            {
                _currentPrefab.transform.SetPositionAndRotation(_currentPrefab.transform.position, rotation);
            }

            return _currentPrefab;

        }

        public void ReturnToPool(Prefab_Platformer platformerObject)
        {
            _currentPrefabType = platformerObject.PrefabType;
            
            //## Check  
            if (_activePoolObjectAll.TryGetValue(_currentPrefabType, out var activePoolObjectList))
            {
                if (activePoolObjectList.Contains(platformerObject))
                {
                    platformerObject.transform.SetParent(transform);

                    //## Get Object Pool
                    _currentObjectPool = GetObjectPool(_currentPrefabType);
                    _currentObjectPool?.Release(platformerObject);
                }
            }

        }

        #endregion

        #region OTHERS

        IObjectPool<Prefab_Platformer> GetObjectPool(PrefabType_Platformer platformerPrefabType)
        {
            if (_objectPoolAll.TryGetValue(platformerPrefabType, out var objectPool))
                return objectPool;

            return null;
        }

        List<Prefab_Platformer> GetActivePoolObjectList(PrefabType_Platformer platformerPrefabType)
        {
            if (_activePoolObjectAll.TryGetValue(platformerPrefabType, out var activePoolObjectList))
                return activePoolObjectList;

            return null;
        }

        Prefab_Platformer GetPoolPrefab(PrefabType_Platformer platformerPrefabType)
        {
            foreach (PrefabAsset_Platformer poolPrefabAsset in _prefabAssetListSo.PoolPrefabAssetList)
            {
                if (poolPrefabAsset.PrefabType == platformerPrefabType)
                    return poolPrefabAsset.Prefab;
            }

            return null;
        }

        #endregion

        #region INITIALIZE POOL

        void CreateObjectPool(PrefabType_Platformer platformerPrefabType)
        {
            _currentObjectPool = new UnityEngine.Pool.ObjectPool<Prefab_Platformer>
            (
                CreatePoolObject,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize
            );
            
            _objectPoolAll.Add(platformerPrefabType, _currentObjectPool);

        }

        Prefab_Platformer CreatePoolObject()
        {
            var prefab = Instantiate(_currentPrefab);
            prefab.gameObject.SetActive(false);
            
            //## Create new ActivePoolObjectList
            if (!_activePoolObjectAll.TryGetValue(_currentPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList = new List<Prefab_Platformer>();
                _activePoolObjectAll.Add(_currentPrefabType, activePoolObjectList);
            }
            
            return prefab;
        }

        void OnTakeFromPool(Prefab_Platformer prefab)
        {
            //## Add 
            if (_activePoolObjectAll.TryGetValue(_currentPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList.Add(prefab);
            }
        }

        void OnReturnedToPool(Prefab_Platformer prefab)
        {
            prefab.gameObject.SetActive(false);

            //## Remove 
            if (_activePoolObjectAll.TryGetValue(_currentPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList.Remove(prefab);
            }
        }

        void OnDestroyPoolObject(Prefab_Platformer prefab)
        {
            Destroy(prefab.gameObject);
        }

        #endregion

        //## TEST
        
        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Prefab_Platformer poolPrefab = GetPoolObject(PrefabType_Platformer.ITEM_Medicien_Health_30Per, transform.position,
                        Quaternion.identity);
                poolPrefab.Activate();
            }

        }*/
    }
}


