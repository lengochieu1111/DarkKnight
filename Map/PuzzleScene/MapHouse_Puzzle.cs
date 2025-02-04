using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Map
{
    public class MapHouse_Puzzle : RyoMonoBehaviour
    {
        [field: SerializeField, BoxGroup("TYPE")] public EMapHouseType_Puzzle MapHouseType { get; private set; }
        [field: SerializeField, BoxGroup("TYPE"), ReadOnly] public List<MapPlacementPoint_Puzzle> MapPlacementPointList;

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
                if (childTransform.TryGetComponent(out MapPlacementPoint_Puzzle mapPlacementPoint)&& !MapPlacementPointList.Contains(mapPlacementPoint))
                {
                    MapPlacementPointList.Add(mapPlacementPoint);
                }
            }
        }

        #endregion
        

    }
}
