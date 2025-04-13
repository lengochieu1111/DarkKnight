using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using HIEU_NL.SO.Weapon;
using UnityEngine;

namespace HIEU_NL.Platformer.SO.Entity.Player
{
    [CreateAssetMenu(fileName = "Player Attack Stats", menuName = "Scriptable Object / Platformer Scene / Player / Attack Stats")]

    public class PlayerAttackStats : ScriptableObject
    {
        [Header("Attack Type")]
        [SerializeField] public WeaponDataListSO WeaponDataListSO;

    }

}
