using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.Platformer.SO.Entity.Player
{

    [CreateAssetMenu(fileName = "Player Movement Stats", menuName = "Scriptable Object / Platformer Scene / Player / Movement Stats")]

    public class PlayerMovementStats : ScriptableObject
    {
        [Header("Walk")]
        [Range(0f, 1f)] public float MoveThreshold = 0.25f;
        [Range(1f, 100f)] public float MaxWalkSpeed = 12.5f;
        [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
        [Range(0.25f, 50f)] public float GroundDeceleration = 20f;
        [Range(0.25f, 50f)] public float AirAcceleration = 5f;
        [Range(0.25f, 50f)] public float AirDeceleration = 5f;
        [Range(0.25f, 50f)] public float WallJumpMoveAcceleration = 5f;
        [Range(0.25f, 50f)] public float WallJumpMoveDeceleration = 5f;

        [Header("Run")]
        [Range(1f, 100f)] public float MaxRunSpeed = 20f;

        [Header("Ground / Collision Chesks")]
        public LayerMask GroundLayer;
        public float GroundDetectionRayLength = 0.02f;
        public float HeadDetectionRayLength = 0.02f;
        [Range(0.1f, 1f)] public float HeadWidth = 0.75f;
        public float WallDetectionRayLength = 0.125f;
        [Range(0.01f, 2f)] public float WallDetectionRayHeightMultiplier = 0.9f;

        [Header("Reset Jump Options")]
        public bool ResetJumpsOnWallSlide = true;

        [Header("Jump")]
        public float JumpHeight = 6.5f;
        public float JumpGravityCoefficient = 0.9f;
        public float GravityJumpMultiplier = 1.5f;
        [Range(0.1f, 1.1f)] public float JumpHeightCompensitionFactor = 1.054f;
        public float TimeTillJumpApex = 0.35f;
        [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
        public float MaxFallSpeed = 26f;
        [Range(1, 5)] public int NumberOfJumpsAllowed = 2;

        [Header("Jump Cut")]
        [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

        [Header("Jump Apex")]
        [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
        [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;

        [Header("Jump Buffer")]
        [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

        [Header("Jump Coyote")]
        [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;

        [Header("Wall Slide")]
        [Min(0.01f)] public float WallSlideSpeed = 5f;
        [Range(0.25f, 50f)] public float WallSlideDecelerationSpeed = 50f;

        [Header("Wall Jump")]
        public Vector2 WallJumpDirection = new Vector2(-20f, 6.5f);
        [Range(0f, 1f)] public float WallJumpPostBufferTime = 0.125f;
        [Range(0.01f, 5f)] public float WallJumpGravityOnReleaseMultiplier = 1f;

        [Header("Dash")]
        [Range(0f, 1f)] public float DashTime = 0.11f;
        [Range(1f, 200f)] public float DashSpeed = 40f;
        [Range(0f, 1f)] public float TimeBtwDashesOnGround = 0.225f;
        public bool ResetDashOnWallSlide = true;
        [Range(1, 5)] public int NumberOfDashes = 2;
        [Range(0f, 0.5f)] public float DashDiagonallyBias = 0.4f;
        [Range(10, 40)] public int DashEnergyReduct = 20;

        [Header("Dash Cancle Time")]
        [Range(0.01f, 5f)] public float DashGravityOnReleaseMultiplier = 1f;
        [Range(0.02f, 0.3f)] public float DashTimeForUpwardsCancle = 0.027f;

        [Header("Debug")]
        public bool DebugShowIsGroundedBox;
        public bool DebugShowHeadBumpBox;
        public bool DebugShowWallHitBox;

        [Header("Jump Visualization Tool")]
        public bool ShowWalkJumpArc = true;
        public bool ShowRunJumpArc = false;
        public bool StopOnCollision = true;
        public bool DrawRight = true;

        [Range(5, 100)] public int ArcResolution = 20;
        [Range(0, 500)] public int VisualizationSteps = 90;

        public readonly Vector2[] DashDirections = new Vector2[]
        {
        new Vector2(0, 0), //# nothing
        new Vector2(1, 0), //# right
        new Vector2(-1, 0), //# left
        new Vector2(0, 1), //# up
        new Vector2(0, -1), //# down
        new Vector2(1, 1).normalized, //# top right
        new Vector2(-1, 1).normalized, //# top left
        new Vector2(1, -1).normalized, //# bottom right
        new Vector2(-1, -1).normalized, //# bottom left
        };

        // Jump
        [field: SerializeField] public float JumpGravity { get; private set; }
        [field: SerializeField] public float InitialJumpVelocity { get; private set; }
        [field: SerializeField] public float AdjustedJumpHeight { get; private set; }

        // Wall Jump
        [field: SerializeField] public float WallJumpGravity { get; private set; }
        [field: SerializeField] public float InitialWallJumpVelocity { get; private set; }
        [field: SerializeField] public float AdjustedWallJumpHeight { get; private set; }

        // UNITY

        private void OnValidate()
        {
            CalculateValues();
        }

        private void OnEnable()
        {
            CalculateValues();
        }

        // OTHERS

        private void CalculateValues()
        {
            //# Jump
            AdjustedJumpHeight = JumpHeight * JumpHeightCompensitionFactor;
            JumpGravity = -(JumpGravityCoefficient * AdjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, JumpGravityCoefficient);
            InitialJumpVelocity = Mathf.Abs(JumpGravity) * TimeTillJumpApex * GravityJumpMultiplier;

            //# Wall Jump
            AdjustedWallJumpHeight = WallJumpDirection.y * JumpHeightCompensitionFactor;
            WallJumpGravity = -(JumpGravityCoefficient * AdjustedWallJumpHeight) / Mathf.Pow(TimeTillJumpApex, JumpGravityCoefficient);
            InitialWallJumpVelocity = Mathf.Abs(WallJumpGravity) * TimeTillJumpApex * GravityJumpMultiplier;
        }

    }

}
