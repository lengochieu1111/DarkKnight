using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Map;
using HIEU_NL.Puzzle.Script.Map;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace HIEU_NL.SO.Weapon
{
    /// <summary>
    /// Map Data
    /// </summary>
    [Serializable]
    public class WeaponData
    {
        [BoxGroup("INFORMATION")] public string WeaponName;
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

