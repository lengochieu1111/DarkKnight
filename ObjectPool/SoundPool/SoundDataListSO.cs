using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace HIEU_NL.ObjectPool.Audio
{
    public enum SoundType
    {
        //## MOUSE
        Click,
        Hover,
        
        //## SWORD
        Sword_Slash_Normal_1,
        Sword_Slash_Normal_2,
        
        Sword_Hit_Normal_1,
        Sword_Hit_Normal_2,
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
