using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Puzzle.Script.Effect;
using HIEU_NL.Puzzle.Script.Entity;
using HIEU_NL.Puzzle.Script.Entity.Player;
using HIEU_NL.Puzzle.Script.Map;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using HIEU_NL.SO.Character;
using HIEU_NL.SO.Map;
using HIEU_NL.SO.Weapon;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HIEU_NL.Puzzle.Script.Game
{
    public class GameMode_Puzzle : Singleton<GameMode_Puzzle>
    {
        public event EventHandler OnChangedState;
        public event EventHandler OnPauseGame;
        
        [field: SerializeField] public Map_Puzzle Map { get; private set; }
        [field: SerializeField] public Player_Puzzle Player { get; private set; }
        
        [SerializeField] private PrefabAssetListSO_Puzzle _prefabAssetListSO;
        [SerializeField] private MapDataListSO _mapDataListSO;
        [SerializeField] private CharacterDataListSO _characterDataListSO;
        [SerializeField] private WeaponDataListSO _weaponDataListSO;
        
        private enum State
        {
            WaitingToStart,
            GamePlaying,
            GameOver
        }

        [Header("State")]
        [SerializeField] private State _state;
        [SerializeField] private float _waitingTimeToStartTimer = 1f;
        [SerializeField] private float _gamePlayTimer = 30f;
        [SerializeField] private int _gameActionCounter = 20;

        [ShowNonSerializedField] private bool _isGamePaused;
        [ShowNonSerializedField] private bool _isGameWon;

        protected override void SetupValues()
        {
            base.SetupValues();
            
            //##
            _isGameWon = false;
            _isGamePaused = false;
        }

        protected override void Start()
        {
            base.Start();

            //##
            SpawnMap();
            SpawnEntities();
            
            //##
            Player_Puzzle.OnPlayerActed += Player_Puzzle_OnPlayerActed;
            Player_Puzzle.OnPlayerPause += Player_Puzzle_OnPlayerPause;
            Player_Puzzle.OnPlayerWin += Player_Puzzle_OnPlayerWin;
        }

        private void Update()
        {
            switch (_state)
            {
                case State.WaitingToStart:
                    _waitingTimeToStartTimer -= Time.deltaTime;

                    if (_waitingTimeToStartTimer < 0f)
                    {
                        _state = State.GamePlaying;

                        OnChangedState?.Invoke(this, EventArgs.Empty);
                    }

                    break; 
                case State.GamePlaying:
                    _gamePlayTimer -= Time.deltaTime;

                    if (_gamePlayTimer <= 0f || _gameActionCounter <= 0)
                    {
                        _state = State.GameOver;
                        SpawnLightPillarDownEffect();
                    }
                    else if (_isGameWon)
                    {
                        _state = State.GameOver;
                        SpawnLightPillarUpEffect();
                    }

                    break; 
                case State.GameOver:
                    break; 
                
            }

        }

        #region ACTION EVENT

        private void Player_Puzzle_OnPlayerActed(object sender, int e)
        {
            _gameActionCounter -= e;
        }

        private void Player_Puzzle_OnPlayerPause(object sender, EventArgs e)
        {
            TogglePauseGame();
        }

        private void Player_Puzzle_OnPlayerWin(object sender, EventArgs e)
        {
            _isGameWon = true;
            FirebaseManager.Instance.UnlockPuzzleUserSaved();
        }
        
        #endregion

        #region MAIN

        private void SpawnMap()
        {
            int levelIndex = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex;
            Map = Instantiate(_mapDataListSO.MapAssetList[levelIndex].MapPrefab_Puzzle);
            _gamePlayTimer = _mapDataListSO.MapAssetList[levelIndex].MaxTime;
            _gameActionCounter = _mapDataListSO.MapAssetList[levelIndex].MaxAction;
        }
        
        private void SpawnEntities()
        {
            int characterIndex = FirebaseManager.Instance.CurrentUser.CurrentCharacterIndex;
            PrefabType_Puzzle playerPrefabType =
                _characterDataListSO.CharacterDataList[characterIndex].PlayPrefab_Puzzle.PrefabType;
            Prefab_Puzzle playerPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                playerPrefabType
                , Map.StartingPlayerPointTransform.position);
            playerPrefab.Activate();
            Player = playerPrefab as Player_Puzzle;

            Prefab_Puzzle spacePortalPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.SpacePortal
                , Map.StartingSpacePortalPointTransform.position);
            spacePortalPrefab.Activate();

            //##
            if (Map.MapHouseList.IsNullOrEmpty())
            {
                Debug.LogWarning("Map House List is null or empty!");
                return;
            }
            
            foreach (MapHouse_Puzzle mapHouse in Map.MapHouseList)
            {
                if (mapHouse.MapPlacementPointList.IsNullOrEmpty())
                {
                    Debug.LogWarning($"{mapHouse.MapHouseType} : Map Placement Point List is null or empty!");
                    continue;
                }
                
                List<PrefabType_Puzzle> botTypeList = new();
                foreach (PrefabAsset_Puzzle prefabAsset in _prefabAssetListSO.PoolPrefabAssetList)
                {
                    if (prefabAsset.Prefab is BaseEntity_Puzzle entity 
                        && entity.MapHouseType == mapHouse.MapHouseType)
                    {
                        botTypeList.Add(prefabAsset.PrefabType);
                    }
                }
                
                if (botTypeList.IsNullOrEmpty())
                {
                    Debug.LogWarning($"{mapHouse.MapHouseType} : Entity List is null or empty!");
                    continue;
                }

                int currentLevelIndex = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex;
                //## Enemy
                if (mapHouse.MapHouseType is EMapHouseType_Puzzle.Enemy)
                {
                    botTypeList.Clear();
                    switch (currentLevelIndex)
                    {
                        case 0:
                            botTypeList.Add(PrefabType_Puzzle.Enemy_1);
                            break;
                        case 1:
                            botTypeList.Add(PrefabType_Puzzle.Enemy_2);
                            break;
                        case 2:
                            botTypeList.Add(PrefabType_Puzzle.Enemy_3);
                            break;
                        case 3:
                            botTypeList.Add(PrefabType_Puzzle.Enemy_4);
                            break;
                    }
                }
                
                //## Lock
                if (mapHouse.MapHouseType is EMapHouseType_Puzzle.Lock)
                {
                    botTypeList.Clear();
                    switch (currentLevelIndex)
                    {
                        case 0:
                            botTypeList.Add(PrefabType_Puzzle.Lock_1);
                            break;
                        case 1:
                            botTypeList.Add(PrefabType_Puzzle.Lock_2);
                            break;
                        case 2:
                            botTypeList.Add(PrefabType_Puzzle.Lock_3);
                            break;
                        case 3:
                            botTypeList.Add(PrefabType_Puzzle.Lock_4);
                            break;
                    }
                }
                
                //## Trap
                if (mapHouse.MapHouseType is EMapHouseType_Puzzle.Trap)
                {
                    botTypeList.Clear();
                    switch (currentLevelIndex)
                    {
                        case 0:
                            botTypeList.Add(PrefabType_Puzzle.Trap_1);
                            break;
                        case 1:
                            botTypeList.Add(PrefabType_Puzzle.Trap_2);
                            break;
                        case 2:
                            botTypeList.Add(PrefabType_Puzzle.Trap_3);
                            break;
                        case 3:
                            botTypeList.Add(PrefabType_Puzzle.Trap_4);
                            break;
                    }
                }
                
                //## Spawn bot
                foreach (MapPlacementPoint_Puzzle mapPlacementPoint in mapHouse.MapPlacementPointList)
                {
                    int randomIndex = Random.Range(0, botTypeList.Count);
                    Prefab_Puzzle poolPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                        botTypeList[randomIndex]
                        , mapPlacementPoint.transform.position);

                    poolPrefab.Activate();
                }
                
            }
        }


        public void TogglePauseGame()
        {
            _isGamePaused = !_isGamePaused;

            if (_isGamePaused)
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

        #region SETTER/GETTER

        public int GetGameActionCounter()
        {
            return _gameActionCounter;
        }
        
        public float GetGamePlayTimer()
        {
            return _gamePlayTimer;
        }
        
        public bool IsGamePaused()
        {
            return _isGamePaused;
        }
        
        public bool IsGamePlaying()
        {
            return _state is State.GamePlaying;
        }
        
        public bool IsGameOver()
        {
            return _state is State.GameOver;
        }
        
        public bool IsGameWon()
        {
            return _isGameWon;
        }
        
        #endregion

        private void SpawnLightPillarUpEffect()
        {
            Prefab_Puzzle poolPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.EFFECT_LightPillar_Up, 
                Map.StartingSpacePortalPointTransform.position);

            if (poolPrefab is BaseEffect_Puzzle effect)
            {
                effect.OnDeactive += Effect_OnDeactive;

                void Effect_OnDeactive(object sender, EventArgs e)
                {
                    OnChangedState?.Invoke(this, EventArgs.Empty);
                    
                    effect.OnDeactive -= Effect_OnDeactive;
                }
            }
            
            poolPrefab.Activate();
        }
        
        private void SpawnLightPillarDownEffect()
        {
            Prefab_Puzzle poolPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.EFFECT_LightPillar_Down, 
                Player.transform.position);
            
            if (poolPrefab is BaseEffect_Puzzle effect)
            {
                effect.OnDeactive += Effect_OnDeactive;

                void Effect_OnDeactive(object sender, EventArgs e)
                {
                    OnChangedState?.Invoke(this, EventArgs.Empty);
                    
                    effect.OnDeactive -= Effect_OnDeactive;
                }
            }
            
            poolPrefab.Activate();
        }
        
        
    }

}
