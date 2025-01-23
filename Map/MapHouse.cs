using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Map
{
    public partial class MapHouse : RyoMonoBehaviour
    {
        [field: SerializeField, BoxGroup("TYPE")] public EMapHouseType MapHouseType { get; private set; }
        [field: SerializeField, BoxGroup("TYPE")] public EBotPlacementType BotPlacementType { get; private set; }
        [field: SerializeField, BoxGroup("TYPE"), ReadOnly] public List<MapPlacementPoint> MapPlacementPointList;

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
                if (childTransform.TryGetComponent(out MapPlacementPoint mapPlacementPoint)&& !MapPlacementPointList.Contains(mapPlacementPoint))
                {
                    MapPlacementPointList.Add(mapPlacementPoint);
                }
            }
        }

        #endregion
        

    }
}
