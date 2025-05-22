using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


/// <summary>
/// Map Data
/// </summary>
[Serializable]
public class MissionData
{
    [BoxGroup("INFORMATION")] public int MissionIndex;
    [BoxGroup("INFORMATION")] public string MissionText;
    [BoxGroup("INFORMATION")] public int MissionAmmount;
    [BoxGroup("INFORMATION")] public int RewardCoin;
    [BoxGroup("INFORMATION"), ShowAssetPreview] public Sprite MissionSprite;

}


/// /// <summary>
/// Map Data List SO
/// </summary>

[CreateAssetMenu(fileName = "Mission Asset List SO", menuName = "Scriptable Object / Platformer Scene / Mission Asset List")]
public class MissionListSO : ScriptableObject
{
    [field: SerializeField] public List<MissionData> MissionDataList { get; private set; }
}
