using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace HIEU_NL.ObjectPool.Audio
{
    public enum SoundType
    {
        //## MOUSE
        Button_Menu_Highlight,
        Button_Menu_Confirm,
        
        //## SWORD
        Sword_Slash_Normal_1,
        Sword_Slash_Normal_2,
        
        Sword_Hit_Normal_1,
        Sword_Hit_Normal_2,
        
        Sword_Block_Normal_1,
        Sword_Block_Normal_2,
        
        Sword_Parry_Normal_1,
        Sword_Parry_Normal_2,
        
        //## CHARACTER
        Dash_1,
        Dash_2,
        
        //## SPIKE
        Spike_Damage_1,
        Spike_Damage_2,
        
        //## STONE
        Stone_Kick_1,
        Stone_Kick_2,
        Stone_Kick_3,
        
        //## ENEMY
        Enemy_Kick_1,
        Enemy_Kick_2,
        Enemy_Kick_3,
        
        Enemy_Dead_1,
        Enemy_Dead_2,
        Enemy_Dead_3,
        
        //## DOOR
        Door_Kick_1,
        Door_Kick_2,
        Door_Kick_3,
        
        Door_Open,
        
        //## KEY
        Key_PickUp,
        
        //## Slash
        Sword_Slash_Special,
        Sword_Hit_Special,

        NONE,
        
        Jump,
        Landing,
        
        TakeDamage,
        Dead,
        
        Man_1_Attack_1,
        Man_1_Attack_2,
        Man_1_Pain,
        Man_1_Dead,
        
        Man_2_Attack_1,
        Man_2_Attack_2,
        Man_2_Pain,
        Man_2_Dead,
        
        Man_3_Attack_1,
        Man_3_Attack_2,
        Man_3_Pain,
        Man_3_Dead,
        
        Woman_1_Attack_1,
        Woman_1_Attack_2,
        Woman_1_Pain,
        Woman_1_Dead,
        
        Monster_1_Attack_1,
        Monster_1_Attack_2,
        Monster_1_Pain,
        Monster_1_Dead,
        
    }

    [Serializable]
    public class SoundAsset
    {
        public SoundType SoundType;
        public SoundData SoundData;
    }

    [CreateAssetMenu(fileName = "Sound Data List SO", menuName = "Scriptable Object / Sound Data List")]

    public class SoundDataListSO : ScriptableObject
    {
        [field: SerializeField] public List<SoundAsset> SoundDataList { get; private set; }
    }

}
