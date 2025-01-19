using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.Script.Map
{
    public class Map : RyoMonoBehaviour
    {
        [field: SerializeField] public Transform StartingPlayerPointTransform { get; private set; }
        [field: SerializeField, ReadOnly] public List<MapHouse> MapHouseList;

        #region LoadMapHouse

        [Button]
        private void LoadMapHouse()
        {
            MapHouseList.Clear();

            foreach (Transform childTransform in transform)
            {
                if (childTransform.TryGetComponent(out MapHouse mapHose) && !MapHouseList.Contains(mapHose))
                {
                    MapHouseList.Add(mapHose);
                }
            }
        }
        
        #endregion

    }

}
