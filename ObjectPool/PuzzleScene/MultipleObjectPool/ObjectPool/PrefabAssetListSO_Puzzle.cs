using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HIEU_NL.Puzzle.Script.ObjectPool.Multiple
{

    ///<summary>
    /// POOL PREFAB TYPE
    /// </summary>
    public enum PrefabType_Puzzle
    {
        NONE,

        Enemy,
        Stone,
        SpacePortal,
        Key,
        Lock_1,
        Trap_1,
        
        Player,
        
        //## Effect
        EFFECT_Slash_1_Normal,
        EFFECT_Slash_2_Normal,

    }


    /// <summary>
    /// POOL PREFAB ASSET
    /// </summary>
    [Serializable]
    public class PrefabAsset_Puzzle
    {
        public PrefabType_Puzzle PrefabType;
        public Prefab_Puzzle Prefab;
    }


    /// /// <summary>
    /// POOL PREFAB ASSET LIST SO
    /// </summary>

    [CreateAssetMenu(fileName = "Prefab Asset List SO", menuName = "Scriptable Object / Puzzel Scene / Prefab Asset List")]
    public class PrefabAssetListSO_Puzzle : ScriptableObject
    {
        [field: SerializeField] public List<PrefabAsset_Puzzle> PoolPrefabAssetList { get; private set; }
    }

}

