using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Puzzle.Script.ObjectPool.Multiple;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Puzzle.Script.Map
{
    public class Map_Puzzle : RyoMonoBehaviour
    {
        [field: SerializeField] public Transform StartingPlayerPointTransform { get; private set; }
        [field: SerializeField] public Transform StartingSpacePortalPointTransform { get; private set; }
        [field: SerializeField, ReadOnly] public List<MapHouse_Puzzle> MapHouseList;

        #region LoadMapHouse

        [Button]
        private void LoadMapHouse()
        {
            MapHouseList.Clear();

            foreach (Transform childTransform in transform)
            {
                if (childTransform.TryGetComponent(out MapHouse_Puzzle mapHouse) && !MapHouseList.Contains(mapHouse))
                {
                    MapHouseList.Add(mapHouse);
                }
            }
        }
        
        #endregion

    }

}
