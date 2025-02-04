using System.Collections.Generic;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Platformer.Script.Entity;
using HIEU_NL.Platformer.Script.Map;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using HIEU_NL.Utilities;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Game
{
    public class GameMode_Platformer : Singleton<GameMode_Platformer>
    {
        [field: SerializeField] public PlayerSection_Platformer PlayerSection;
        [field: SerializeField] public MapSection_Platformer MapSection;
        [field: SerializeField] public HUD_Platformer HUD;
        
        [SerializeField] private PrefabAssetListSO_Platformer _prefabAssetListSO;


        protected override void Start()
        {
            base.Start();
            
            //##
            SpawnEntities();
            
        }

        private void SpawnEntities()
        {
            if (MapSection.mapPlatformer.MapHouseList.IsNullOrEmpty())
            {
                Debug.LogWarning("Map House List is null or empty!");
                return;
            }
            
            foreach (MapHouse_Platformer mapHouse in MapSection.mapPlatformer.MapHouseList)
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

                    poolPrefab.Activate();
                }
                
            }
            
        }
        
    }

}
