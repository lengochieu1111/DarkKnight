using System;
using System.Collections.Generic;
using System.Linq;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Platformer.Script.Entity;
using HIEU_NL.Platformer.Script.Entity.Enemy;
using HIEU_NL.Platformer.Script.Entity.Player;
using HIEU_NL.Platformer.Script.Interface;
using HIEU_NL.Platformer.Script.Map;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using HIEU_NL.SO.Map;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HIEU_NL.Platformer.Script.Game
{
    public class GameMode_Platformer : Singleton<GameMode_Platformer>
    {
        public event EventHandler OnChangedState;
        public event EventHandler OnPauseGame;
        
        private enum State
        {
            WaitingToStart,
            GamePlaying,
            GameOver
        }
        
        [Header("Stats")]
        [SerializeField] private MapDataListSO _mapDataListSO;
        [SerializeField] private PrefabAssetListSO_Platformer _prefabAssetListSO;
        [SerializeField] private CinemachineCamera _followPlayerCamera;

        [SerializeField, ReadOnly] public Player_Platformer Player;
        [SerializeField, ReadOnly] public Map_Platformer Map;
        
        [Header("State")]
        [SerializeField] private State _state;
        [SerializeField, ReadOnly] public bool IsGamePaused;
        [SerializeField, ReadOnly] public bool IsGameWon;
        
        [Header("Level")]
        [ShowNonSerializedField] private int _currentLevelIndex;

        private List<BaseEnemy> _enemyList = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            
            //##
            BaseEnemy.OnAnyDeadEnemy += BaseEnemy_OnAnyDeadEnemy;
        }

        protected override void Start()
        {
            base.Start();
            
            //##
            SpawnMap();
            SpawnEntities();
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (var enemy in _enemyList)
                {
                    enemy.ITakeDamage(new HitData(null, Vector3.zero, enemy.Health, false));
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            //##
            BaseEnemy.OnAnyDeadEnemy -= BaseEnemy_OnAnyDeadEnemy;
        }

        #region RESET

        protected override void ResetValues()
        {
            base.ResetValues();
            
            //##
            _state = State.WaitingToStart;
            IsGamePaused = false;
            IsGameWon = false;
            
            _currentLevelIndex = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex;
        }

        #endregion

        #region EVENT ACTION

        private void BaseEnemy_OnAnyDeadEnemy(object sender, EventArgs e)
        {
            if (_enemyList.IsNullOrEmpty()) return;
            bool hasEnemyAlive = _enemyList.Any(enemy => !enemy.IsDead);
            if (!hasEnemyAlive)
            {
                IsGameWon = true;
                _state = State.GameOver;

                int currentSelectedLevel = FirebaseManager.Instance.CurrentSelectedLevel;
                int CurrentLevel = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex;

                if (currentSelectedLevel.Equals(CurrentLevel) && currentSelectedLevel < _mapDataListSO.MapAssetList.Count - 1)
                {
                    FirebaseManager.Instance.UpgradeLevel(currentSelectedLevel + 1);
                    FirebaseManager.Instance.SetCurrentSelectedLevel(currentSelectedLevel + 1);

                }
                else if (!currentSelectedLevel.Equals(CurrentLevel))
                {
                    FirebaseManager.Instance.SetCurrentSelectedLevel(currentSelectedLevel + 1);
                }

                OnChangedState?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion


        #region MAIN

        private void SpawnMap()
        {
            Map = Instantiate(_mapDataListSO.MapAssetList[_currentLevelIndex].MapPrefab_Platformer);
            // _gamePlayTimer = _mapDataListSO.MapAssetList[_currentLevelIndex].MaxTime;
        }
        
        private void SpawnEntities()
        {
            Prefab_Platformer playerPrefab = ObjectPool_Platformer.Instance.GetPoolObject(
                PrefabType_Platformer.Player
                , Map.StartingPlayerPointTransform.transform.position);
            playerPrefab.Activate();
            
            Player = playerPrefab as Player_Platformer;

            _followPlayerCamera.Follow = Player.transform;
            
            //##
            _enemyList.Clear();

            if (Map.MapHouseList.IsNullOrEmpty())
            {
                Debug.LogWarning("Map House List is null or empty!");
                return;
            }

            foreach (MapHouse_Platformer mapHouse in Map.MapHouseList)
            {
                if (mapHouse.MapPlacementPointList.IsNullOrEmpty())
                {
                    Debug.LogWarning($"{mapHouse.MapHouseType} : Map Placement Point List is null or empty!");
                    continue;
                }

                List<PrefabType_Platformer> botTypeList = new();
                foreach (PrefabAsset_Platformer prefabAsset in _prefabAssetListSO.PoolPrefabAssetList)
                {
                    if (prefabAsset.Prefab is BaseEntity entity
                        && entity.MapHouseTypePlatformer == mapHouse.MapHouseType
                        && entity.BotPlacementTypePlatformer == mapHouse.BotPlacementType)
                    {
                        botTypeList.Add(prefabAsset.PrefabType);
                    }
                }

                if (botTypeList.IsNullOrEmpty())
                {
                    Debug.LogWarning($"{mapHouse.MapHouseType} : Entity List is null or empty!");
                    continue;
                }

                foreach (MapPlacementPoint_Platformer mapPlacementPoint in mapHouse.MapPlacementPointList)
                {
                    int randomIndex = Random.Range(0, botTypeList.Count);
                    Prefab_Platformer poolPrefab = ObjectPool_Platformer.Instance.GetPoolObject(
                        botTypeList[randomIndex]
                        , mapPlacementPoint.transform.position);

                    if (poolPrefab is BaseEntity entity)
                    {
                        entity.SetMapPlacementPoint(mapPlacementPoint);
                    }

                    if (poolPrefab is BaseEnemy enemy)
                    {
                        _enemyList.Add(enemy);
                    }

                    poolPrefab.Activate();
                }
                
            }
            
            //## Start Game
            _state = State.GamePlaying;
            OnChangedState?.Invoke(this, EventArgs.Empty);
        }

        public void TogglePauseGame()
        {
            IsGamePaused = !IsGamePaused;

            if (IsGamePaused)
            {
                Time.timeScale = 0f; 
            }
            else
            {
                Time.timeScale = 1f;
            }

            OnPauseGame?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion

        
        /*
         * Setter - Getter
         */
        
        public bool IsGamePlaying()
        {
            return _state is State.GamePlaying;
        }
        
        public bool IsGameOver()
        {
            return _state is State.GameOver;
        }
        
    }

}
