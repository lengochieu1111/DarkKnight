using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace HIEU_NL.SO.Weapon
{
    [Serializable]
    public class WeaponAttackData
    {
        [BoxGroup("PLATFORMER")] public PrefabType_Platformer AttackEffectOne;
        [BoxGroup("PLATFORMER")] public PrefabType_Platformer AttackEffectTwo;
        [BoxGroup("PLATFORMER")] public PrefabType_Platformer SpecialAttackEffect;
        [BoxGroup("PLATFORMER")] public PrefabType_Platformer HitAttackEffect;
    }
    

    /// <summary>
    /// Map Data
    /// </summary>
    [Serializable]
    public class WeaponData
    {
        //## PLATFORMER
        [BoxGroup("PLATFORMER")] public WeaponAttackData WeaponPrefab_Platformer;
        
        [BoxGroup("INFORMATION")] public int WeaponNormalDamage;
        [BoxGroup("INFORMATION")] public int WeaponSpecialDamage;
        
        [BoxGroup("INFORMATION")] public int WeaponSpecialEnergyReduct;
        
        [BoxGroup("INFORMATION")] public int WeaponNormalEnergyRecovery;
        
        [BoxGroup("INFORMATION")] public string WeaponName;
        [BoxGroup("INFORMATION")] public int WeaponIndex;
        [BoxGroup("INFORMATION")] public int WeaponCost;
        [BoxGroup("INFORMATION")] public string WeaponDetail;
        [BoxGroup("INFORMATION"), ShowAssetPreview] public Sprite WeaponSprite;
    }


    /// /// <summary>
    /// Map Data List SO
    /// </summary>

    [CreateAssetMenu(fileName = "Weapon Asset List SO", menuName = "Scriptable Object / Platformer Scene / Weapon Asset List")]
    public class WeaponDataListSO : ScriptableObject
    {
        [field: SerializeField] public List<WeaponData> WeaponDataList { get; private set; }
    }

}

