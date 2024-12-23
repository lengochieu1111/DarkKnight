using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level SO List", menuName = "Scriptable Object / Puzzel Scene / Level SO List")]

public class LevelSOList_Puzzle : ScriptableObject
{
    [field: SerializeField] public List<LevelSO_Puzzle> LevelSOList { get; private set; }
}
