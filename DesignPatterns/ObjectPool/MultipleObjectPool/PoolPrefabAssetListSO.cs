using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.DesignPatterns.ObjectPool.Multiple
{
    
    ///<summary>
    /// POOL PREFAB TYPE
    /// </summary>
    public enum PoolPrefabType
    {
    NONE,

    //# ____LOGIN_____
    Profile_LOGIN,

    //# ____PUZZLE_____
    Player_PUZZLE,

    //# ____PLATFORMER_____
    
    //## Effect
        EFFECT_Slash_1_Normal_PLATFORMER,
        EFFECT_Slash_2_Normal_PLATFORMER,
        
        EFFECT_Blood_1_PLATFORMER,
    
        //## Bot
        BOT_Golem_1_PLATFORMER,
        BOT_Golem_2_PLATFORMER,
        BOT_Golem_3_PLATFORMER,
        
        BOT_Skeleton_Axe_PLATFORMER,
        BOT_Skeleton_Shield_PLATFORMER,
        BOT_Skeleton_Sword_PLATFORMER,
        
        BOT_Bush_PLATFORMER,
        BOT_Mushroon_PLATFORMER,
        BOT_Slime_PLATFORMER,
        
        BOT_Mosquito_PLATFORMER,

    }
    
    
    /// <summary>
    /// POOL PREFAB ASSET
    /// </summary>
    [Serializable]
    public class PoolPrefabAsset
    {
        public PoolPrefabType PrefabType;
        public PoolPrefab PoolPrefab;
    }
    
    
    /// /// <summary>
    /// POOL PREFAB ASSET LIST SO
    /// </summary>
    
    [CreateAssetMenu(fileName = "Pool Prefab Asset List SO", menuName = "Scriptable Object / Pool Prefab Asset List")]
    public class PoolPrefabAssetListSO : ScriptableObject
    {
        [field: SerializeField] public List<PoolPrefabAsset> PoolPrefabAssetList { get; private set; }
    }

}

