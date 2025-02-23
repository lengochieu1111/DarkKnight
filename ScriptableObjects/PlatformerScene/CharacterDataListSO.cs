using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Map;
using HIEU_NL.Puzzle.Script.Map;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace HIEU_NL.SO.Character
{
    /// <summary>
    /// Character Data
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        //## INFORMATION
        [BoxGroup("INFORMATION")] public string CharacterName;
        [BoxGroup("INFORMATION")] public int CharacterCost;
        [BoxGroup("INFORMATION")] public string CharacterDetail;
        [BoxGroup("INFORMATION"), ShowAssetPreview] public Sprite CharacterSprite;
    }


    /// /// <summary>
    /// Character Data List SO
    /// </summary>

    [CreateAssetMenu(fileName = "Character Asset List SO", menuName = "Scriptable Object / Platformer Scene / Character Asset List")]
    public class CharacterDataListSO : ScriptableObject
    {
        [field: SerializeField] public List<CharacterData> CharacterDataList { get; private set; }
    }

}

