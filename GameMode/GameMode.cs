using System.Collections.Generic;
using HIEU_NL.DesignPatterns.ObjectPool.Multiple;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Platformer.Script.Entity;
using HIEU_NL.Platformer.Script.Map;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Game
{
    public class GameMode : Singleton<GameMode>
    {
        [field: SerializeField] public PlayerSection PlayerSection;
        [field: SerializeField] public MapSection MapSection;
        [field: SerializeField] public HUD HUD;
        
        [SerializeField] private PoolPrefabAssetListSO _poolPrefabAssetListSO;


        protected override void Start()
        {
            base.Start();
            
            //##

            SpawnEntities();
        }

        private void SpawnEntities()
        {
            if (MapSection.Map.MapHouseList.IsNullOrEmpty())
            {
                Debug.LogWarning("Map House List is null or empty!");
                return;
            }
            
            foreach (MapHouse mapHouse in MapSection.Map.MapHouseList)
            {
                if (mapHouse.MapPlacementPointList.IsNullOrEmpty())
                {
                    Debug.LogWarning($"{mapHouse.MapHouseType} : Map Placement Point List is null or empty!");
                    continue;
                }
                
                List<PoolPrefabType> botTypeList = new();
                foreach (PoolPrefabAsset poolPrefabAsset in _poolPrefabAssetListSO.PoolPrefabAssetList)
                {
                    if (poolPrefabAsset.PoolPrefab is BaseEntity entity && entity.MapHouseType == mapHouse.MapHouseType)
                    {
                        botTypeList.Add(poolPrefabAsset.PrefabType);
                    }
                }
                
                if (botTypeList.IsNullOrEmpty())
                {
                    Debug.LogWarning($"{mapHouse.MapHouseType} : Entity List is null or empty!");
                    continue;
                }

                foreach (MapPlacementPoint mapPlacementPoint in mapHouse.MapPlacementPointList)
                {
                    int randomIndex = Random.Range(0, botTypeList.Count);
                    PoolPrefab poolPrefab = MultipleObjectPool.Instance.GetPoolObject(
                        botTypeList[randomIndex]
                        , mapPlacementPoint.transform.position);

                    if (poolPrefab is BaseEntity entity)
                    {
                        entity.SetMapPlacementPoint(mapPlacementPoint);
                    }

                    poolPrefab.Activate();
                }
                
            }
        }
        
    }

}
