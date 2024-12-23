using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.ObjectPool.Single;

public class LevelPrefab_Puzzle : RyoMonoBehaviour
{
    //[Header("Entity Holder")]
    //[SerializeField] private Transform _startingPlayerPosition;
    //[SerializeField] private Transform _startingSpacePortalPosition;
    //[SerializeField] private Transform _stoneHolderTransform;
    //[SerializeField] private Transform _trapHolderTransform;
    //[SerializeField] private Transform _enemyHolderTransform;
    //[SerializeField] private Transform _keyHolderTransform;
    //[SerializeField] private Transform _clockHolderTransform;

    //protected override void SetupComponents()
    //{
    //    base.SetupComponents();

    //    if (this._startingPlayerPosition == null)
    //    {
    //        this._startingPlayerPosition = this.transform.Find("StartingPlayerPosition");
    //    }

    //    if (this._startingSpacePortalPosition == null)
    //    {
    //        this._startingSpacePortalPosition = this.transform.Find("StartingSpacePortalPosition");
    //    }

    //    if (this._stoneHolderTransform == null)
    //    {
    //        this._stoneHolderTransform = this.transform.Find("StoneHolder");
    //    }

    //    if (this._trapHolderTransform == null)
    //    {
    //        this._trapHolderTransform = this.transform.Find("TrapHolder");
    //    }

    //    if (this._enemyHolderTransform == null)
    //    {
    //        this._enemyHolderTransform = this.transform.Find("EnemyHolder");
    //    }

    //    if (this._keyHolderTransform == null)
    //    {
    //        this._keyHolderTransform = this.transform.Find("KeyHolder");
    //    }

    //    if (this._clockHolderTransform == null)
    //    {
    //        this._clockHolderTransform = this.transform.Find("ClockHolder");
    //    }

    //}

    //protected override void Start()
    //{
    //    base.Start();

    //    this.SpawnEntities();
    //}

    //private void SpawnEntities()
    //{
    //    EntityManager_Puzzle.Instance.SpawnPlayer(this._startingPlayerPosition.position, Quaternion.identity, this._startingPlayerPosition);

    //    EntityManager_Puzzle.Instance.SpawnSpacePortal(this._startingSpacePortalPosition.position, Quaternion.identity, this._startingSpacePortalPosition);

    //    foreach (Transform child in this._stoneHolderTransform)
    //    {
    //        EntityManager_Puzzle.Instance.SpawnStone(child.position, Quaternion.identity, child);
    //    }

    //    foreach (Transform child in this._trapHolderTransform)
    //    {
    //        EntityManager_Puzzle.Instance.SpawnTrap_1(child.position, Quaternion.identity, child);
    //    }

    //    foreach (Transform child in this._enemyHolderTransform)
    //    {
    //        EntityManager_Puzzle.Instance.SpawnEnemy(child.position, Quaternion.identity, child);
    //    }

    //    foreach (Transform child in this._keyHolderTransform)
    //    {
    //        EntityManager_Puzzle.Instance.SpawnKey(child.position, Quaternion.identity, child);
    //    }

    //    foreach (Transform child in this._clockHolderTransform)
    //    {
    //        EntityManager_Puzzle.Instance.SpawnLock_1(child.position, Quaternion.identity, child);
    //    }

    //}


}
