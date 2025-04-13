using System;
using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Entity.Player;
using HIEU_NL.Puzzle.Script.Entity.Player;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.SO.Character
{
    /// <summary>
    /// Character Data
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        //## PUZZLE
        [BoxGroup("PUZZLE")] public Player_Puzzle PlayPrefab_Puzzle;
        
        //## PLATFORMER
        [BoxGroup("PLATFORMER")] public Player_Platformer PlayPrefab_Platformer;
        
        //## INFORMATION
        [BoxGroup("INFORMATION")] public string CharacterName;
        [BoxGroup("INFORMATION")] public int CharacterIndex;
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

