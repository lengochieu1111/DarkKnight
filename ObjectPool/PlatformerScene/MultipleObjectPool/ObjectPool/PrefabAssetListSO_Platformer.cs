using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HIEU_NL.Platformer.Script.ObjectPool.Multiple
{
    
    ///<summary>
    /// POOL PREFAB TYPE
    /// </summary>
    public enum PrefabType_Platformer
    {
        NONE,

        //## Effect
        EFFECT_Slash_1_Normal,
        EFFECT_Slash_2_Normal,
        
        EFFECT_Blood_1,
    
        //## Bot
        BOT_Golem_1,
        BOT_Golem_2,
        BOT_Golem_3,
        
        BOT_Skeleton_Axe,
        BOT_Skeleton_Shield,
        BOT_Skeleton_Sword,
        
        BOT_Bush,
        BOT_Mushroon,
        BOT_Slime,
        
        BOT_Mosquito,
        Enemy_2,
        Enemy_3,
        
        //## Player
        Player_1,
        Player_2,
        
        //## Effect
        EFFECT_Slash_1_Blue,
        EFFECT_Slash_2_Blue,
        
        EFFECT_Slash_1_Red,
        EFFECT_Slash_2_Red,
        
        EFFECT_Slash_1_Purple,
        EFFECT_Slash_2_Purple,
        
        //## ITEM
        
        ITEM_Medicien_Health_30Per,
        ITEM_Medicien_Health_70Per,
        ITEM_Medicien_Health_100Per,
        
        ITEM_Medicien_Energy_30Per,
        ITEM_Medicien_Energy_70Per,
        ITEM_Medicien_Energy_100Per,
        
        //## Effect
        EFFECT_PickUp_Medicine_Health_Effect,
        EFFECT_PickUp_Medicine_Energy_Effect,
        
        //## Bot
        BOT_Beetle,
        
        //## Others
        Projectile_Beetle,
        
        //## Boss
        BOSS_Bringer,
        BOSS_Viking,
        BOSS_Warrior,
        BOSS_Axe__,
        
        //## Others
        Spell_Bringer,
        
    }
    
    
    /// <summary>
    /// POOL PREFAB ASSET
    /// </summary>
    [Serializable]
    public class PrefabAsset_Platformer
    {
        public PrefabType_Platformer PrefabType;
        public Prefab_Platformer Prefab;
    }
    
    
    /// /// <summary>
    /// POOL PREFAB ASSET LIST SO
    /// </summary>
    
    [CreateAssetMenu(fileName = "Prefab Asset List SO", menuName = "Scriptable Object / Platformer Scene / Prefab Asset List")]
    public class PrefabAssetListSO_Platformer : ScriptableObject
    {
        [field: SerializeField] public List<PrefabAsset_Platformer> PoolPrefabAssetList { get; private set; }
    }

}

