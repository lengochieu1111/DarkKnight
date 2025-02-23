using System;
using System.Collections.Generic;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Puzzle.Script.Entity;
using HIEU_NL.Puzzle.Script.Entity.Player;
using HIEU_NL.Puzzle.Script.Map;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using HIEU_NL.SO.Map;
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
        
        [field: SerializeField] public HUD_Puzzle HUD { get; private set; }
        [field: SerializeField] public MapSection_Puzzle MapSection { get; private set; }
        [field: SerializeField] public PlayerSection_Puzzle PlayerSection { get; private set; }
        
        [SerializeField] private PrefabAssetListSO_Puzzle _prefabAssetListSO;
        [SerializeField] private MapDataListSO _mapDataListSO;
        
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

        [Header("Level")]
        [ShowNonSerializedField] private int _currentLevelIndex;

        protected override void SetupValues()
        {
            base.SetupValues();
            
            //##
            _currentLevelIndex = FirebaseManager.Instance.CurrentUser.CurrentLevelIndex;
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
            Player_Puzzle.OnPlayerLoses += Player_Puzzle_OnPlayerLoses;
            Player_Puzzle.OnPlayerWins += Player_Puzzle_OnPlayerWins;
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

                    if (_gamePlayTimer < 0f || _gameActionCounter < 0 || _isGameWon)
                    {
                        _state = State.GameOver;

                        OnChangedState?.Invoke(this, EventArgs.Empty);
                    }

                    break; 
                case State.GameOver:
                    break; 
                
            }

        }

        #region ACTION EVENT

        private void Player_Puzzle_OnPlayerActed(object sender, EventArgs e)
        {
            _gameActionCounter--;
        }

        private void Player_Puzzle_OnPlayerPause(object sender, EventArgs e)
        {
            TogglePauseGame();
        }

        private void Player_Puzzle_OnPlayerLoses(object sender, EventArgs e)
        {
            _gameActionCounter = -1;
        }
        
        private void Player_Puzzle_OnPlayerWins(object sender, EventArgs e)
        {
            _isGameWon = true;
            
            FirebaseManager.Instance.UnlockPuzzleUserSaved();

        }
        
        #endregion

        #region MAIN

        private void SpawnMap()
        {
            MapSection.Map = Instantiate(_mapDataListSO.MapAssetList[_currentLevelIndex].MapPrefab_Puzzle);
            _gamePlayTimer = _mapDataListSO.MapAssetList[_currentLevelIndex].MaxTime;
            _gameActionCounter = _mapDataListSO.MapAssetList[_currentLevelIndex].MaxAction;
        }
        
        private void SpawnEntities()
        {
            Prefab_Puzzle playerPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.Player
                , MapSection.Map.StartingPlayerPointTransform.transform.position);
            playerPrefab.Activate();
            
            Prefab_Puzzle spacePortalPrefab = ObjectPool_Puzzle.Instance.GetPoolObject(
                PrefabType_Puzzle.SpacePortal
                , MapSection.Map.StartingSpacePortalPointTransform.transform.position);
            spacePortalPrefab.Activate();

            //##
            if (MapSection.Map.MapHouseList.IsNullOrEmpty())
            {
                Debug.LogWarning("Map House List is null or empty!");
                return;
            }
            
            foreach (MapHouse_Puzzle mapHouse in MapSection.Map.MapHouseList)
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
        
        public int GetLevelIndex()
        {
            return _currentLevelIndex;
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

        
        
        
    }

}
