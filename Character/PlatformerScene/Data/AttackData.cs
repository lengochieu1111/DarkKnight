using System;
using HIEU_NL.Platformer.Script.ObjectPool.Multiple;
using HIEU_NL.Utilities;
using UnityEngine;

namespace HIEU_NL.Platformer.SerializableClass
{
    [Serializable]
    public class AttackData
    {
        [field: SerializeField] public ParameterExtensions.Animation.AnimationType AttackAnimType { get; private set; }
        [field: SerializeField] public PrefabType_Platformer AttackPrefabType { get; private set; }
        [field: SerializeField] public float AttackRadiusWidth { get; private set; } = 5f;
        [field: SerializeField] public float AttackRangeHeight { get; private set; } = 5f;
        [field: SerializeField] public float AttackOffsetWidth { get; private set; }
        [field: SerializeField] public float AttackOffsetHeight { get; private set; }
        
        [field: SerializeField] public float Damage { get; private set; } = 20f;
    }
}