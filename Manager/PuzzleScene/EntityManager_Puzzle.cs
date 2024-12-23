using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIEU_NL.DesignPatterns.Singleton;

public class EntityManager_Puzzle : Singleton<EntityManager_Puzzle>
{
    [SerializeField] private EntityPoolSO_Puzzle _playerSO;
    [SerializeField] private EntityPoolSO_Puzzle _enemySO;
    [SerializeField] private EntityPoolSO_Puzzle _stoneSO;
    [SerializeField] private EntityPoolSO_Puzzle _spacePortalSO;
    [SerializeField] private EntityPoolSO_Puzzle _keySO;

    [SerializeField] private EntityPoolSO_Puzzle _trap_1_SO;

    [SerializeField] private EntityPoolSO_Puzzle _lock_1_SO;

    private void SpawnEntity(EntityPoolSO_Puzzle entitySO, Vector3 position, Quaternion rotation, Transform parrent)
    {
        //Transform entityPrefabTransform = EntityPool_Puzzle.Instance.SpawnObjectPool(entitySO, position, rotation, parrent);
        //entityPrefabTransform.gameObject.SetActive(true);
    }

    /*
     * 
     */

    public void SpawnPlayer(Vector3 position, Quaternion rotation, Transform parrent)
    {
        this.SpawnEntity(this._playerSO, position, rotation, parrent);
    }
    
    public void SpawnEnemy(Vector3 position, Quaternion rotation, Transform parrent)
    {
        this.SpawnEntity(this._enemySO, position, rotation, parrent);
    }
    
    public void SpawnStone(Vector3 position, Quaternion rotation, Transform parrent)
    {
        this.SpawnEntity(this._stoneSO, position, rotation, parrent);
    }
    
    public void SpawnSpacePortal(Vector3 position, Quaternion rotation, Transform parrent)
    {
        this.SpawnEntity(this._spacePortalSO, position, rotation, parrent);
    }
    
    public void SpawnKey(Vector3 position, Quaternion rotation, Transform parrent)
    {
        this.SpawnEntity(this._keySO, position, rotation, parrent);
    }

    // trap
    public void SpawnTrap_1(Vector3 position, Quaternion rotation, Transform parrent)
    {
        this.SpawnEntity(this._trap_1_SO, position, rotation, parrent);
    }
    
    // lock
    public void SpawnLock_1(Vector3 position, Quaternion rotation, Transform parrent)
    {
        this.SpawnEntity(this._lock_1_SO, position, rotation, parrent);
    }

}
