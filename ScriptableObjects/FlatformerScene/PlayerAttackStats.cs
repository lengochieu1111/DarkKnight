using HIEU_NL.DesignPatterns.ObjectPool.Multiple;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.Platformer.SO
{
    [CreateAssetMenu(fileName = "Player Attack Stats", menuName = "Scriptable Object / Platformer Scene / Player Attack Stats")]

    public class PlayerAttackStats : ScriptableObject
    {
        [Header("Attack Type")]
        [SerializeField] public List<PoolPrefabType> AttackList;

    }

}
