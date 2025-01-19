using System.Collections.Generic;
using HIEU_NL.DesignPatterns.ObjectPool.Multiple;
using HIEU_NL.DesignPatterns.Singleton;
using HIEU_NL.Platformer.Script.Entity;
using HIEU_NL.Platformer.Script.Map;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.GameMode
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
            foreach (MapHouse mapHouse in MapSection.Map.MapHouseList)
            {
                List<BaseEntity> entityList = new();

                foreach (PoolPrefabAsset poolPrefabAsset in _poolPrefabAssetListSO.PoolPrefabAssetList)
                {
                    if (poolPrefabAsset.PoolPrefab is BaseEntity entity && entity.MapHouseType == mapHouse.MapHouseType)
                    {
                        entityList.Add(entity);
                    }
                }

                foreach (MapPlacementPoint mapPlacementPoint in mapHouse.MapPlacementPointList)
                {
                    int randomIndex = Random.Range(0, entityList.Count);
                    PoolPrefab poolPrefab = MultipleObjectPool.Instance.GetPoolObject(entityList[randomIndex].PoolPrefabType);
                    poolPrefab.Activate();
                }
                
            }
        }
        
    }

}
