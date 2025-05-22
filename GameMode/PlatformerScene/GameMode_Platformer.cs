using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Platformer.Script.Entity;
using HIEU_NL.Platformer.Script.Entity.Enemy;
using HIEU_NL.Platformer.Script.Entity.Enemy.Boss;
using HIEU_NL.Platformer.Script.Entity.Player;
using HIEU_NL.Platformer.Script.Map;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using HIEU_NL.SO.Character;
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
        public static event EventHandler<int> OnSetupSuccess;
        public event EventHandler OnChangedState;
        public event EventHandler OnPauseGame;
        public event EventHandler OnBossBattle;
        
        private enum State
        {
            WaitingToStart,
            GamePlaying,
            GameOver
        }
        
        [Header("Stats")]
        [SerializeField] private MapDataListSO _mapDataListSO;
        [SerializeField] private CharacterDataListSO _characterDataListSO;
        [SerializeField] private PrefabAssetListSO_Platformer _prefabAssetListSO;
        [SerializeField] private CinemachineCamera _followPlayerCamera;
        [SerializeField] private CinemachineConfiner2D _cinemachineConfiner;

        [SerializeField, ReadOnly] public Player_Platformer Player;
        [SerializeField, ReadOnly] public Map_Platformer Map;
        
        [Header("State")]
        [SerializeField] private State _state;
        [SerializeField, ReadOnly] public bool IsGamePaused;
        [SerializeField, ReadOnly] public bool IsGameWon;
        
        [Header("Level")]
        [ShowNonSerializedField] private int _currentLevelIndex;
        [ShowNonSerializedField] private int _currentCharacterIndex;

        private List<BaseEnemy> _enemyList = new();
        public List<BaseEnemy> EnemyList => _enemyList;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            //##
            BaseEnemy.OnAnyDeadEnemy += BaseEnemy_OnAnyDeadEnemy;
            Player_Platformer.OnHealthChange += Player_OnOnHealthChange;
            PlatformerCanvas.Instance.OnBossComing += PlatformerCanvas_OnBossComing;
            Player_Platformer.OnPlayerPause += Player_OnPlayerPause;
        }


        protected override void Start()
        {
            base.Start();
            
            //##
            SpawnMap();
            SpawnEntities();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            //##
            /*BaseEnemy.OnAnyDeadEnemy -= BaseEnemy_OnAnyDeadEnemy;
            Player_Platformer.OnHealthChange -= Player_OnOnHealthChange;
            PlatformerCanvas.Instance.OnBossComing -= PlatformerCanvas_OnBossComing;*/
        }

        #region RESET

        protected override void ResetValues()
        {
            base.ResetValues();
            
            //##
            IsGameWon = false;
            IsGamePaused = false;
            _state = State.WaitingToStart;
            
            _currentLevelIndex = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex;
            _currentCharacterIndex = FirebaseManager.Instance.CurrentUser.CurrentCharacterIndex;

        }

        #endregion

        #region EVENT ACTION
        
        private void Player_OnPlayerPause(object sender, EventArgs e)
        {
            TogglePauseGame();
        }
        
        private void PlatformerCanvas_OnBossComing(object sender, EventArgs e)
        {
            PlayerAppearInBossRoom();
        }

        private void BaseEnemy_OnAnyDeadEnemy(object sender, EventArgs e)
        {
            if (_enemyList.IsNullOrEmpty()) return;

            if (sender is BaseBoss)
            {
                FirebaseManager.Instance.UpdateMissionAmount(3, 1);
            }
            
            bool hasBotAlive = _enemyList.Any(enemy => !enemy.IsDead && enemy is not BaseBoss);
            bool hasBossAlive = _enemyList.Any(enemy => !enemy.IsDead && enemy is BaseBoss);

            if (!hasBotAlive)
            {
                if (hasBossAlive)
                {
                    HandleBossBattle();
                }
                else
                {
                    HandleGameWin();
                }
            }
            
        }
        
        private void Player_OnOnHealthChange(object sender, float e)
        {
            if (e <= 0)
            {
                HandleGameLoss();
            }
        }
        

        #endregion


        #region MAIN

        private void SpawnMap()
        {
            Map = Instantiate(_mapDataListSO.MapAssetList[_currentLevelIndex].MapPrefab_Platformer);
        }
        
        private void SpawnEntities()
        {
            PrefabType_Platformer playerPrefabType =
                _characterDataListSO.CharacterDataList[_currentCharacterIndex].PlayPrefab_Platformer.PrefabType;
            Prefab_Platformer playerPrefab = ObjectPool_Platformer.Instance.GetPoolObject(
                playerPrefabType
                , Map.StartingPlayerPointTransform.position);
            playerPrefab.Activate();
            
            Player = playerPrefab as Player_Platformer;
            Player.SetHealth(_mapDataListSO.MapAssetList[_currentLevelIndex].PlayerHealth);

            //##
            _followPlayerCamera.Follow = Player.transform;
            _cinemachineConfiner.enabled = true;
            _cinemachineConfiner.BoundingShape2D = Map.CameraColliderBouds;
            _cinemachineConfiner.InvalidateBoundingShapeCache();
            
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
            OnSetupSuccess?.Invoke(this, _enemyList.Count);
                
            _state = State.GamePlaying;
            OnChangedState?.Invoke(this, EventArgs.Empty);
        }

        private void PlayerAppearInBossRoom()
        {
            //##
            _cinemachineConfiner.enabled = false;
            _followPlayerCamera.Follow = Map.PlayerSpawnPointInBossRoomTransform;
            
            //##
            Player.transform.position = Map.PlayerSpawnPointInBossRoomTransform.position;
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


        private void HandleBossBattle()
        {
            OnBossBattle?.Invoke(this, EventArgs.Empty);
        }

        private void HandleGameWin()
        {
            IsGameWon = true;
            _state = State.GameOver;

            int currentSelectedLevel = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex;
            int currentLevel = FirebaseManager.Instance.CurrentUser.CurrentMaxLevelIndex;

            if (currentSelectedLevel.Equals(currentLevel) && currentSelectedLevel < _mapDataListSO.MapAssetList.Count - 1)
            {
                FirebaseManager.Instance.UpgradeLevel(currentSelectedLevel + 1);
                FirebaseManager.Instance.UseLevel(currentSelectedLevel + 1);

            }
            else if (!currentSelectedLevel.Equals(currentLevel))
            {
                FirebaseManager.Instance.UseLevel(currentSelectedLevel + 1);
            }
            
            UT_GameOver().Forget();
        }

        private void HandleGameLoss()
        {
            IsGameWon = false;
            _state = State.GameOver;
            
            UT_GameOver().Forget();
        }


        private async UniTask UT_GameOver()
        {
            await UniTask.WaitForSeconds(2f);
            OnChangedState?.Invoke(this, EventArgs.Empty);
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
