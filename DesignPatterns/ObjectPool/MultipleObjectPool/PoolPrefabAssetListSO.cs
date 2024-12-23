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
    Slash_1_PLATFORMER,
    Slash_2_PLATFORMER,
    
        //## Effect
        Blood_1_PLATFORMER,
    
        //## Bot
        Golem_1_PLATFORMER,
        Golem_2_PLATFORMER,
        Golem_3_PLATFORMER,

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

