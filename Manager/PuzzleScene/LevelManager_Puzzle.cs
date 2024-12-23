using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;

public class LevelManager_Puzzle : Singleton<LevelManager_Puzzle>
{
    [SerializeField] private LevelSOList_Puzzle _levelSOList;

    public void SpawnLevel(int levelIndex)
    {
        //Transform levelTransform = LevelPool_Puzzle.Instance.SpawnObjectPool(this._levelSOList.LevelSOList[levelIndex], Vector3.zero, Quaternion.identity);
        //levelTransform.gameObject.SetActive(true);
    }

    public LevelSO_Puzzle GetLevelSOWithIndex(int levelIndex)
    {
        return this._levelSOList.LevelSOList[levelIndex];
    }

}
