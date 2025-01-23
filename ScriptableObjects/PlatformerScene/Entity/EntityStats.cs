using System;
using HIEU_NL.Utilities;
using NaughtyAttributes;
using UnityEngine;

namespace HIEU_NL.Platformer.SO.Entity
{
    public abstract class EntityStats : ScriptableObject
    {
        //# COLLISION CHECK
        [field: SerializeField, BoxGroup("COLLISION CHECK")] public LayerMask TerrainLayer{ get; private set; }
        [field: SerializeField, BoxGroup("COLLISION CHECK")] public float HeadWidth { get; private set; } = 0.75f;
        [field: SerializeField, BoxGroup("COLLISION CHECK")] public float HeadDetectionRayLength { get; private set; } = 0.02f;
        [field: SerializeField, BoxGroup("COLLISION CHECK")] public float GroundDetectionRayLength { get; private set; } = 0.02f;
        [field: SerializeField, BoxGroup("COLLISION CHECK")] public float WallDetectionRayLength { get; private set; } = 0.125f;
        [field: SerializeField, BoxGroup("COLLISION CHECK")] public float WallDetectionRayHeightMultiplier { get; private set; } = 0.9f;
        [field: SerializeField, BoxGroup("COLLISION CHECK")] public float GravityCoefficient { get; private set; } = 0.5f;

        //# HEALTH
        [field: SerializeField, BoxGroup("HEALTH")] public int MaxHealth { get; private set; } = 100;
        
        //# DEBUG
        public bool DebugShowIsGroundedBox;
        public bool DebugShowHeadBumpBox;
        public bool DebugShowWallHitBox;

        protected virtual void Reset()
        {
            TerrainLayer = ParameterExtensions.Layers.Terrain;
        }

    }
}

