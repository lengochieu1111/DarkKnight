using System;
using HIEU_NL.Platformer.SerializableClass;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.SO.Entity.Enemy
{
    [CreateAssetMenu(fileName = "Enemy Stats", menuName = "Scriptable Object / Platformer Scene / Enemy / Enemy Stats")]
    public class EnemyStats : EntityStats
    {

        //## IDLE
        [field: SerializeField, BoxGroup("IDLE")] public float IdleTimeMax { get; private set; } = 2f;

        //## PATROL
        [field: SerializeField, BoxGroup("PATROL")] public float PatrolSpeed { get; private set; } = 1f;
        [field: SerializeField, BoxGroup("PATROL")] public float PatrolRadius { get; private set; } = 5f;
        
        //## CHASE
        [field: SerializeField, BoxGroup("CHASE")] public float ChaseSpeed { get; private set; } = 2f;
        [field: SerializeField, BoxGroup("CHASE")] public float ChaseRadius { get; private set; } = 5f;

        //## ATTACK
        [field: SerializeField, BoxGroup("ATTACK")] public LayerMask AttackLayer { get; private set; }
        [field: SerializeField, BoxGroup("ATTACK")] public AttackData[] AttackDataArray { get; private set; }

        protected override void Reset()
        {
            base.Reset();
            
            AttackLayer = ParameterExtensions.Layers.Player;
        }
        
    }
}

