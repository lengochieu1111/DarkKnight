using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Map
{
    public class MapHouse_Platformer : RyoMonoBehaviour
    {
        [field: SerializeField, BoxGroup("TYPE")] public EMapHouseType_Platformer MapHouseType { get; private set; }
        [field: SerializeField, BoxGroup("TYPE")] public EBotPlacementType_Platformer BotPlacementType { get; private set; }
        [field: SerializeField, BoxGroup("TYPE"), ReadOnly] public List<MapPlacementPoint_Platformer> MapPlacementPointList;

        protected override void SetupValues()
        {
            base.SetupValues();
            
            //##
            LoadMapPlacementPoint();

        }

        #region MapPlacementPoint

        [Button]
        private void LoadMapPlacementPoint()
        {
            MapPlacementPointList.Clear();
            
            foreach (Transform childTransform in transform)
            {
                if (childTransform.TryGetComponent(out MapPlacementPoint_Platformer mapPlacementPoint)&& !MapPlacementPointList.Contains(mapPlacementPoint))
                {
                    MapPlacementPointList.Add(mapPlacementPoint);
                }
            }
        }

        #endregion
        

    }
}
