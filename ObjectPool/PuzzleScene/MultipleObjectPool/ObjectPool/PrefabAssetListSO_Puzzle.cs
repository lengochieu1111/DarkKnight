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

        //## Enemy
        Enemy_1,
        
        //## Stone
        Stone,
        
        //## Space Portal
        SpacePortal,
        
        //## Key
        Key,
        
        //## Lock
        Lock_1,
        
        //## Trap
        Trap_1,
        
        //## Player
        Player_1,
        
        //## Effect
        EFFECT_Slash_1_Normal,
        EFFECT_Slash_2_Normal,
        
        //## Lock
        Lock_2,
        Lock_3,
        Lock_4,

        //## Trap
        Trap_2,
        Trap_3,
        Trap_4,
        
        //## Player
        Player_2,
        
        //## Effect
        EFFECT_Slash_1_Blue,
        EFFECT_Slash_2_Blue,
        
        EFFECT_Slash_1_Red,
        EFFECT_Slash_2_Red,
        
        EFFECT_Slash_1_Purple,
        EFFECT_Slash_2_Purple,
        
        //## Enemy
        Enemy_2,
        Enemy_3,
        Enemy_4,
        
        //## Effect
        EFFECT_BigImpact,
        EFFECT_SmallImpact,
        EFFECT_Blood,
        EFFECT_DustTrail,
        
        EFFECT_LightPillar_Up,
        EFFECT_LightPillar_Down,
        
        EFFECT_Lock_PickUp

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

