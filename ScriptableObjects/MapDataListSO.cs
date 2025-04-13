using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Map;
using HIEU_NL.Puzzle.Script.Map;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.SO.Map
{
    /// <summary>
    /// Map Data
    /// </summary>
    [Serializable]
    public class MapData
    {
        //## PUZZLE
        [Header("PUZZLE")]
        [BoxGroup("PUZZLE")] public Map_Puzzle MapPrefab_Puzzle;
        [BoxGroup("PUZZLE")] public int MaxTime;
        [BoxGroup("PUZZLE")] public int MaxAction;
        
        //## PLATFORMER
        [Header("PLATFORMER")]
        [BoxGroup("PLATFORMER")] public Map_Platformer MapPrefab_Platformer;
        // [BoxGroup("PLATFORMER")] public int MaxHealthMedicienRate;
        // [BoxGroup("PLATFORMER")] public int MaxEnergyMedicien;

        //## INFORMATION
        [Header("INFORMATION")]
        [BoxGroup("INFORMATION"), ShowAssetPreview] public Sprite MapSprite;
        [BoxGroup("INFORMATION")] public int MapIndex;
        [BoxGroup("INFORMATION")] public string MapName;

    }


    /// /// <summary>
    /// Map Data List SO
    /// </summary>

    [CreateAssetMenu(fileName = "Map Asset List SO", menuName = "Scriptable Object / Puzzel Scene / Map Asset List")]
    public class MapDataListSO : ScriptableObject
    {
        [field: SerializeField] public List<MapData> MapAssetList { get; private set; }
    }

}

