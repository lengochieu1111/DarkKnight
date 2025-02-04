using System;
using HIEU_NL.DesignPatterns.Singleton;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace HIEU_NL.Puzzle.Script.ObjectPool.Multiple
{
    public class ObjectPool_Puzzle : Singleton<ObjectPool_Puzzle> 
    {
        public readonly Dictionary<PrefabType_Puzzle, IObjectPool<Prefab_Puzzle>> _objectPoolAll = new();
        private readonly Dictionary<PrefabType_Puzzle, List<Prefab_Puzzle>>  _activePoolObjectAll = new();

        [SerializeField] private PrefabAssetListSO_Puzzle _prefabAssetListSO;
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;

        private Prefab_Puzzle _currentPrefab;
        private PrefabType_Puzzle _currentPrefabType;
        private IObjectPool<Prefab_Puzzle> _currentObjectPool;

        #region GET/RETURN POOL OBJECT

        public Prefab_Puzzle GetPoolObject(PrefabType_Puzzle puzzlePrefabType, Vector3 position = default,
            Quaternion rotation = default, Transform parent = default)
        {
            _currentPrefabType = puzzlePrefabType;

            //## Get Pool Prefab
            _currentPrefab = GetPoolPrefab(puzzlePrefabType);
            if (_currentPrefab is null)
            {
                Debug.Assert(_currentPrefab is not null, $"Pool prefab : {nameof(_currentPrefab)} not exist.");
                return null;
            }

            //## Get Object Pool
            _currentObjectPool = GetObjectPool(puzzlePrefabType);
            if (_currentObjectPool is null)
            {
                //## Create Object Pool
                CreateObjectPool(puzzlePrefabType);
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

        public void ReturnToPool(Prefab_Puzzle puzzleObject)
        {
            _currentPrefabType = puzzleObject.PrefabType;
            
            //## Check  
            if (_activePoolObjectAll.TryGetValue(_currentPrefabType, out var activePoolObjectList))
            {
                if (activePoolObjectList.Contains(puzzleObject))
                {
                    puzzleObject.transform.SetParent(transform);

                    //## Get Object Pool
                    _currentObjectPool = GetObjectPool(_currentPrefabType);
                    _currentObjectPool?.Release(puzzleObject);
                }
            }

        }

        #endregion

        #region OTHERS

        IObjectPool<Prefab_Puzzle> GetObjectPool(PrefabType_Puzzle puzzlePrefabType)
        {
            if (_objectPoolAll.TryGetValue(puzzlePrefabType, out var objectPool))
                return objectPool;

            return null;
        }

        List<Prefab_Puzzle> GetActivePoolObjectList(PrefabType_Puzzle puzzlePrefabType)
        {
            if (_activePoolObjectAll.TryGetValue(puzzlePrefabType, out var activePoolObjectList))
                return activePoolObjectList;

            return null;
        }

        Prefab_Puzzle GetPoolPrefab(PrefabType_Puzzle puzzlePrefabType)
        {
            foreach (PrefabAsset_Puzzle poolPrefabAsset in _prefabAssetListSO.PoolPrefabAssetList)
            {
                if (poolPrefabAsset.PrefabType == puzzlePrefabType)
                    return poolPrefabAsset.Prefab;
            }

            return null;
        }

        #endregion

        #region INITIALIZE POOL

        void CreateObjectPool(PrefabType_Puzzle puzzlePrefabType)
        {
            _currentObjectPool = new UnityEngine.Pool.ObjectPool<Prefab_Puzzle>
            (
                CreatePoolObject,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize
            );
            
            _objectPoolAll.Add(puzzlePrefabType, _currentObjectPool);

        }

        Prefab_Puzzle CreatePoolObject()
        {
            var prefab = Instantiate(_currentPrefab);
            prefab.gameObject.SetActive(false);
            
            //## Create new ActivePoolObjectList
            if (!_activePoolObjectAll.TryGetValue(_currentPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList = new List<Prefab_Puzzle>();
                _activePoolObjectAll.Add(_currentPrefabType, activePoolObjectList);
            }
            
            return prefab;
        }

        void OnTakeFromPool(Prefab_Puzzle prefab)
        {
            //## Add 
            if (_activePoolObjectAll.TryGetValue(_currentPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList.Add(prefab);
            }
        }

        void OnReturnedToPool(Prefab_Puzzle prefab)
        {
            prefab.gameObject.SetActive(false);

            //## Remove 
            if (_activePoolObjectAll.TryGetValue(_currentPrefabType, out var activePoolObjectList))
            {
                activePoolObjectList.Remove(prefab);
            }
        }

        void OnDestroyPoolObject(Prefab_Puzzle prefab)
        {
            Destroy(prefab.gameObject);
        }

        #endregion

        //## TEST
        
        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                PoolPrefab poolPrefab = MultipleObjectPool.Instance.GetPoolObject(PoolPrefabType.BOT_Mosquito_PLATFORMER);
                poolPrefab.Activate();
            }

        }*/
    }
}


