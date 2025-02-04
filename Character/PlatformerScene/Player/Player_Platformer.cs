using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.MVC;
using HIEU_NL.Utilities;
using NaughtyAttributes;

namespace HIEU_NL.Platformer.Script.Entity.Player
{
    using DesignPatterns.ObjectPool.Multiple;
    using static ParameterExtensions.Animation;

    public class Player_Platformer : MVC_Controller<PlayerModel, PlayerView>
    {
        private PlatformerPLayerInputActions _inputActions;

        [BoxGroup("Animator")]
        [SerializeField] private Animator _animator;

        [BoxGroup("Input")]
        [SerializeField] private Vector2 _inputDirection;

        [SerializeField] private bool _jumpWasPressed;
        [SerializeField] private bool _jumpIsHeld;
        [SerializeField] private bool _jumpWasReleased;

        [SerializeField] private bool _runIsHeld;

        [SerializeField] private bool _dashWasPressed;

        [SerializeField] private bool _attackNormalWasPressed;
        [SerializeField] private bool _attackStrongWasPressed;

        #region MOVEMENT

        [BoxGroup("Move")]
        [SerializeField] private float _horizontalVelocity;


        [BoxGroup("Jump")]
        [SerializeField] private float _verticalVelocity;
        [SerializeField] private bool _isJumping;
        [SerializeField] private bool _isFastFalling;
        [SerializeField] private bool _isFalling;
        [SerializeField] private float _fastFallTime;
        [SerializeField] private float _fastFallReleaseSpeed;
        [SerializeField] private int _numberOfJumpsUsed;

        [BoxGroup("Jump Apex")]
        [SerializeField] private float _apexPoint;
        [SerializeField] private float _timePastApexThreshold;
        [SerializeField] private bool _isPastApexThreshold;

        [BoxGroup("Jump Buffer")]
        [SerializeField] private float _jumpBufferTimer;
        [SerializeField] private bool _jumpReleasedDuringBuffer;

        [BoxGroup("Jump Coyote")]
        [SerializeField] private float _coyoteTimer;

        [BoxGroup("Wall Slide")]
        [SerializeField] private bool _isWallSliding;
        [SerializeField] private bool _isWallSlideFalling;

        [BoxGroup("Wall Jump")]
        [SerializeField] private bool _useWallJumpMoveStats;
        [SerializeField] private bool _isWallJumping;
        [SerializeField] private float _wallJumpTime;
        [SerializeField] private bool _isWallJumpFastFalling;
        [SerializeField] private bool _isWallJumpFalling;
        [SerializeField] private float _wallJumpFastFallTime;
        [SerializeField] private float _wallJumpFastFallReleaseSpeed;

        [SerializeField] private float _wallJumpPostBufferTimer;

        [SerializeField] private float _wallJumpApexPoint;
        [SerializeField] private float _timePastWallJumpApexThreshold;
        [SerializeField] private bool _isPastWallJumpApexThreshold;

        [BoxGroup("Dash")]
        [SerializeField] private bool _isDashing;
        [SerializeField] private bool _isAirDashing;
        [SerializeField] private float _dashTimer;
        [SerializeField] private float _dashOnGroundTimer;
        [SerializeField] private int _numberOfDashesUsed;
        [SerializeField] private Vector2 _dashDirection;
        [SerializeField] private bool _isDashFastFalling;
        [SerializeField] private float _dashFastFallTime;
        [SerializeField] private float _dashFastFallReleaseSpeed;

        #endregion

        #region ATTACK

        [BoxGroup("Attack")]
        [SerializeField] private bool _isAttacking;
        [SerializeField] private Transform _attackPointTransform;
        [SerializeField] private int _attackIndex;

        #endregion

        [BoxGroup("ParticleSystem")]
        [SerializeField, Required("Dust Particle")] private ParticleSystem _dustParticle;


        #region UNITY CORE

        protected override void Awake()
        {
            base.Awake();

            //##
            _inputActions = new PlatformerPLayerInputActions();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            //##
            _inputActions.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            //##
            _inputActions.Disable();
        }

        protected virtual void Update()
        {
            UpdatePlayerInput();

            CountTimer();

            AttackChesck();

            JumpChecks();
            LandChecks();
            WallJumpCheck();

            WallSlideCheck();
            DashCheck();

            //##

            if (!isGrounded || !(_inputDirection.x == 0))
            {
                PlayEffect_Dust();
            }

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            //##
            Jump();
            Fall();
            WallSlide();
            WallJump();
            Dash();

            if (isGrounded)
            {
                Move(model.MovementStats.GroundAcceleration, model.MovementStats.GroundDeceleration, _inputDirection);
            }
            else
            {
                // wall jumping
                if (_useWallJumpMoveStats)
                {
                    Move(model.MovementStats.WallJumpMoveAcceleration, model.MovementStats.WallJumpMoveDeceleration, _inputDirection);
                }

                // airborne
                else
                {
                    Move(model.MovementStats.AirAcceleration, model.MovementStats.AirDeceleration, _inputDirection);
                }
            }

            ApplyVelocity();
        }

        #endregion

        #region SETUP/RESET COMPONENT/VALUE

        protected override void ResetValues()
        {
            base.ResetValues();

            ResetAttack();

            _verticalVelocity = Physics2D.gravity.y;
        }

        protected override void SetupComponents()
        {
            base.SetupComponents();

            if (rigidbody_ == null)
            {
                rigidbody_ = GetComponent<Rigidbody2D>();
            }

            if (capsuleCollider == null)
            {
                capsuleCollider = GetComponent<CapsuleCollider2D>();
            }

            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider2D>();
            }

        }

        #endregion

        #region UPDATE VALUE

        private void UpdatePlayerInput()
        {
            _inputDirection = _inputActions.Player.Move.ReadValue<Vector2>();

            _jumpWasPressed = _inputActions.Player.Jump.WasPressedThisFrame();
            _jumpIsHeld = _inputActions.Player.Jump.IsPressed();
            _jumpWasReleased = _inputActions.Player.Jump.WasReleasedThisFrame();

            _runIsHeld = _inputActions.Player.Run.IsPressed();

            _dashWasPressed = _inputActions.Player.Dash.WasPressedThisFrame();

            _attackNormalWasPressed = _inputActions.Player.AttackNormal.WasPressedThisFrame();
            _attackStrongWasPressed = _inputActions.Player.AttackStrong.WasPressedThisFrame();
        }

        #endregion

        #region Movement

        private void Move(float acceleration, float deceleration, Vector2 moveInput)
        {
            if (!_isDashing)
            {
                bool isMoving = Mathf.Abs(moveInput.x) >= model.MovementStats.MoveThreshold;

                if (isMoving)
                {
                    float targetVelocity = 0;

                    if (_runIsHeld)
                    {
                        targetVelocity = moveInput.x * model.MovementStats.MaxRunSpeed;
                    }
                    else
                    {
                        targetVelocity = moveInput.x * model.MovementStats.MaxWalkSpeed;
                    }

                    _horizontalVelocity = Mathf.Lerp(_horizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
                    
                    //##
                    HandleFlip();
                    
                }
                else
                {
                    _horizontalVelocity = Mathf.Lerp(_horizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
                }

                _animator.SetBool("IsMoving", moveInput.x != 0);

            }
        }

        private void HandleFlip()
        {
            int yAxisRotateLeft = isBeginFlipLeft ? Y_AXIS_0 : Y_AXIS_180;
            int yAxisRotateRight = isBeginFlipLeft ? Y_AXIS_180 : Y_AXIS_0;
            
            if (_horizontalVelocity > 0 && isFlippingLeft && Mathf.Approximately(MyTransform.eulerAngles.y, yAxisRotateLeft))
            {
                MyTransform.rotation = Quaternion.Euler(0, yAxisRotateRight, 0);
                isFlippingLeft = false;
            }
            else if (_horizontalVelocity < 0 && !isFlippingLeft && Mathf.Approximately(MyTransform.eulerAngles.y, yAxisRotateRight))
            {
                MyTransform.rotation = Quaternion.Euler(0, yAxisRotateLeft, 0);
                isFlippingLeft = true;
            }
        }

        #endregion

        #region Jump

        private void ResetJumpValues()
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;
        }

        private void JumpChecks()
        {
            if (_jumpWasPressed)
            {
                if (_isWallSlideFalling && _wallJumpPostBufferTimer >= 0f)
                {
                    return;
                }

                else if (_isWallSliding || (isTouchingWall && !isGrounded))
                {
                    return;
                }

                _jumpBufferTimer = model.MovementStats.JumpBufferTime;
                _jumpReleasedDuringBuffer = false;
            }

            if (_jumpWasReleased)
            {
                if (_jumpBufferTimer > 0f)
                {
                    _jumpReleasedDuringBuffer = true;
                }

                if (_isJumping && _verticalVelocity > 0f)
                {
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                        _isFastFalling = true;
                        _fastFallTime = model.MovementStats.TimeForUpwardsCancel;
                        _verticalVelocity = 0f;
                    }
                    else
                    {
                        _isFastFalling = true;
                        _fastFallReleaseSpeed = _verticalVelocity;
                    }
                }
            }

            // initiate jump with jump buffering and coyote time
            if (_jumpBufferTimer > 0f && !_isJumping && (isGrounded || _coyoteTimer > 0f))
            {
                InitiateJump(1);

                if (_jumpReleasedDuringBuffer)
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = _verticalVelocity;
                }
            }

            // double jump
            else if (_jumpBufferTimer > 0f && (_isJumping || _isWallJumping || _isWallSlideFalling || _isAirDashing || _isDashFastFalling) && !isTouchingWall && _numberOfJumpsUsed < model.MovementStats.NumberOfJumpsAllowed)
            {
                _isFastFalling = false;
                InitiateJump(1);

                if (_isDashFastFalling)
                {
                    _isDashFastFalling = false;
                }
            }

            // air jump after coyote time lapsed
            else if (_jumpBufferTimer > 0f && _isFalling && !_isWallSlideFalling && _numberOfJumpsUsed < model.MovementStats.NumberOfJumpsAllowed - 1)
            {
                InitiateJump(2);
                _isFastFalling = false;
            }
        }

        private void InitiateJump(int numberOfJumpsUsed)
        {
            //## PLAY ANIMATION
            PlayAnimation_JumpStart();

            //##
            if (!_isJumping)
            {
                _isJumping = true;
            }

            ResetAttack();
            ResetWallJumpValues();

            _jumpBufferTimer = 0f;
            _numberOfJumpsUsed += numberOfJumpsUsed;
            _verticalVelocity = model.MovementStats.InitialJumpVelocity;
        }

        private void Jump()
        {
            // apply gravity while jumping
            if (_isJumping)
            {
                // check for head bump
                if (bumpedHead)
                {
                    _isFastFalling = true;
                }

                // gravity on ascending
                if (_verticalVelocity >= 0f)
                {
                    // apex controls
                    _apexPoint = Mathf.InverseLerp(model.MovementStats.InitialJumpVelocity, 0f, _verticalVelocity);

                    if (_apexPoint > model.MovementStats.ApexThreshold)
                    {
                        if (!_isPastApexThreshold)
                        {
                            _isPastApexThreshold = true;
                            _timePastApexThreshold = 0f;
                        }

                        if (_isPastApexThreshold)
                        {
                            _timePastApexThreshold += Time.fixedDeltaTime;

                            if (_timePastApexThreshold < model.MovementStats.ApexHangTime)
                            {
                                _verticalVelocity = 0f;
                            }
                            else
                            {
                                _verticalVelocity = -0.01f;
                            }
                        }

                    }

                    // gravity on ascending but not past apex threshold
                    else if (!_isFastFalling)
                    {
                        _verticalVelocity += model.MovementStats.JumpGravity * Time.fixedDeltaTime;

                        if (_isPastApexThreshold)
                        {
                            _isPastApexThreshold = false;
                        }
                    }

                }

                // gravity on descending
                else if (!_isFastFalling)
                {
                    _verticalVelocity += model.MovementStats.JumpGravity * model.MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }

                else if (_verticalVelocity < 0f)
                {
                    if (!_isFalling)
                    {
                        _isFalling = true;
                    }
                }

            }

            // jump cut
            if (_isFastFalling)
            {
                if (_fastFallTime >= model.MovementStats.TimeForUpwardsCancel)
                {
                    _verticalVelocity += model.MovementStats.JumpGravity * model.MovementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
                else if (_fastFallTime < model.MovementStats.TimeForUpwardsCancel)
                {
                    _verticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / model.MovementStats.TimeForUpwardsCancel));
                }

                _fastFallTime += Time.fixedDeltaTime;
            }
        }

        #endregion

        #region Land /Fall

        private void LandChecks()
        {
            // land
            if ((_isJumping || _isFalling || _isWallJumpFalling || _isWallJumping || _isWallSlideFalling || _isWallSliding || _isDashFastFalling) && isGrounded && _verticalVelocity <= 0f)
            {
                //## PLAY ANIMATION
                PlayAnimation_Landing();

                //##
                ResetJumpValues();
                StopWallSlide();
                ResetWallJumpValues();
                ResetDashes();
                ResetAttack();

                _numberOfJumpsUsed = 0;

                _verticalVelocity = Physics2D.gravity.y;

                if (_isDashFastFalling && isGrounded)
                {
                    ResetDashValues();
                    return;
                }

                ResetDashValues();

            }
        }

        private void Fall()
        {
            // normal gravity while falling
            if (!isGrounded && !_isJumping && !_isWallSliding && !_isWallJumping && !_isDashing && _isDashFastFalling)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }

                _verticalVelocity += model.MovementStats.JumpGravity * Time.fixedDeltaTime;
            }
        }

        #endregion

        #region Wall Slide

        private void WallSlideCheck()
        {
            if (isTouchingWall && !isGrounded && !_isDashing)
            {
                if (_verticalVelocity < 0f && !_isWallSliding)
                {
                    ResetJumpValues();
                    ResetWallJumpValues();
                    ResetDashValues();

                    if (model.MovementStats.ResetDashOnWallSlide)
                    {
                        ResetDashes();
                    }

                    _isWallSlideFalling = false;
                    _isWallSliding = true;

                    if (model.MovementStats.ResetJumpsOnWallSlide)
                    {
                        _numberOfJumpsUsed = 0;
                    }

                }
            }

            else if (_isWallSliding && !isTouchingWall && !isGrounded && !_isWallSlideFalling)
            {
                _isWallSlideFalling = true;
                StopWallSlide();
            }

            else
            {
                StopWallSlide();
            }
        }

        private void StopWallSlide()
        {
            if (_isWallSliding)
            {
                _numberOfJumpsUsed++;

                _isWallSliding = false;
            }
        }

        private void WallSlide()
        {
            if (_isWallSliding)
            {
                _verticalVelocity = Mathf.Lerp(_verticalVelocity, -model.MovementStats.WallSlideSpeed, model.MovementStats.WallSlideDecelerationSpeed * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Wall Jump

        private void WallJumpCheck()
        {
            if (ShouldApplyPostWallJumpBuffer())
            {
                _wallJumpPostBufferTimer = model.MovementStats.WallJumpPostBufferTime;
            }

            // wall jump fast falling
            if (_jumpWasReleased && !_isWallSliding && !isTouchingWall && _isWallJumping)
            {
                if (_verticalVelocity > 0f)
                {
                    if (_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = false;
                        _isWallJumpFastFalling = true;
                        _wallJumpFastFallTime = model.MovementStats.TimeForUpwardsCancel;

                        _verticalVelocity = 0f;
                    }
                    else
                    {
                        _isWallJumpFastFalling = true;
                        _wallJumpFastFallReleaseSpeed = _verticalVelocity;
                    }
                }
            }

            // actual jump with post wall jump buffer time
            if (_jumpWasPressed && _wallJumpPostBufferTimer > 0f)
            {
                InitiateWallJump();
            }
        }

        private void InitiateWallJump()
        {
            if (!_isWallJumping)
            {
                _isWallJumping = true;
                _useWallJumpMoveStats = true;
            }

            StopWallSlide();
            ResetJumpValues();
            _wallJumpTime = 0f;

            _verticalVelocity = model.MovementStats.InitialWallJumpVelocity;

            int dirMultiplier = 0;
            Vector2 hitPoint = lastWallHit.collider.ClosestPoint(capsuleCollider.bounds.center);

            if (hitPoint.x > MyTransform.position.x)
            {
                dirMultiplier = -1;
            }
            else
            {
                dirMultiplier = 1;
            }

            _horizontalVelocity = Mathf.Abs(model.MovementStats.WallJumpDirection.x) * dirMultiplier;
        }

        private void WallJump()
        {
            // apply wall jump gravity
            if (_isWallJumping)
            {
                // time to take over movement controls while wall jumping
                _wallJumpTime += Time.fixedDeltaTime;
                if (_wallJumpTime >= model.MovementStats.TimeTillJumpApex)
                {
                    _useWallJumpMoveStats = false;
                }

                // hit head
                if (bumpedHead)
                {
                    _isWallJumpFastFalling = true;
                    _useWallJumpMoveStats = false;
                }

                // gravity in ascending
                if (_verticalVelocity >= 0f)
                {
                    // apex controls
                    _wallJumpApexPoint = Mathf.InverseLerp(model.MovementStats.WallJumpDirection.y, 0f, _verticalVelocity);

                    if (_wallJumpApexPoint > model.MovementStats.ApexThreshold)
                    {
                        if (!_isPastWallJumpApexThreshold)
                        {
                            _isPastWallJumpApexThreshold = true;
                            _timePastWallJumpApexThreshold = 0f;
                        }

                        if (_isPastWallJumpApexThreshold)
                        {
                            _timePastWallJumpApexThreshold += Time.fixedDeltaTime;
                            if (_timePastWallJumpApexThreshold < model.MovementStats.ApexHangTime)
                            {
                                _verticalVelocity = 0f;
                            }
                            else
                            {
                                _verticalVelocity = -0.01f;
                            }
                        }
                    }

                    // gravity in ascending but not past apex threshold
                    else if (!_isWallJumpFastFalling)
                    {
                        _verticalVelocity += model.MovementStats.WallJumpGravity * Time.fixedDeltaTime;

                        if (_isPastWallJumpApexThreshold)
                        {
                            _isPastWallJumpApexThreshold = false;
                        }
                    }
                }

                // gravity on desending
                else if (!_isWallJumpFastFalling)
                {
                    _verticalVelocity += model.MovementStats.WallJumpGravity * Time.fixedDeltaTime;
                }

                else if (_verticalVelocity < 0f)
                {
                    if (!_isWallJumpFalling)
                    {
                        _isWallJumpFalling = true;
                    }
                }
            }

            // handle wall jump cut time
            if (_isWallJumpFastFalling)
            {
                if (_wallJumpFastFallTime >= model.MovementStats.TimeForUpwardsCancel)
                {
                    _verticalVelocity += model.MovementStats.WallJumpGravity * model.MovementStats.WallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
                else if (_wallJumpFastFallTime < model.MovementStats.TimeForUpwardsCancel)
                {
                    _verticalVelocity = Mathf.Lerp(_wallJumpFastFallReleaseSpeed, 0f, (_wallJumpFastFallTime / model.MovementStats.TimeForUpwardsCancel));
                }

                _wallJumpFastFallTime += Time.fixedDeltaTime;
            }
        }

        private bool ShouldApplyPostWallJumpBuffer()
        {
            if (!isGrounded && (isTouchingWall || _isWallSliding))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ResetWallJumpValues()
        {
            _isWallSlideFalling = false;
            _useWallJumpMoveStats = false;
            _isWallJumping = false;
            _isWallJumpFastFalling = false;
            _isWallJumpFalling = false;
            _isPastWallJumpApexThreshold = false;

            _wallJumpFastFallTime = 0f;
            _wallJumpTime = 0f;

        }

        #endregion

        #region Dash

        private void ResetDashValues()
        {
            _isDashFastFalling = false;
            _dashOnGroundTimer = -0.01f;
        }

        private void ResetDashes()
        {
            _numberOfDashesUsed = 0;
        }

        private void DashCheck()
        {
            if (_dashWasPressed)
            {
                // ground dash
                if (isGrounded && _dashOnGroundTimer < 0 && !_isDashing)
                {
                    InitiateDash();
                }

                // air dash
                else if (!isGrounded && !_isDashing && _numberOfDashesUsed < model.MovementStats.NumberOfDashes)
                {
                    _isAirDashing = true;
                    InitiateDash();

                    // you left a wallslide but dashed within the wall jump post buffertimer
                    if (_wallJumpPostBufferTimer > 0f)
                    {
                        _numberOfJumpsUsed--;
                        if (_numberOfJumpsUsed < 0)
                        {
                            _numberOfJumpsUsed = 0;
                        }
                    }
                }
            }
        }

        private void InitiateDash()
        {
            _dashDirection = _inputDirection;

            Vector2 closeDirection = Vector2.zero;
            float minDistance = Vector2.Distance(_dashDirection, model.MovementStats.DashDirections[0]);

            for (int i = 0; i < model.MovementStats.DashDirections.Length; i++)
            {
                // skip if me hit it bang on
                if (_dashDirection == model.MovementStats.DashDirections[i])
                {
                    closeDirection = _dashDirection;
                    break;
                }

                float distance = Vector2.Distance(_dashDirection, model.MovementStats.DashDirections[i]);

                // check if this is a diagonal direction and apply bias
                bool isDiagonal = (Mathf.Abs(model.MovementStats.DashDirections[i].x) == 1) && (Mathf.Abs(model.MovementStats.DashDirections[i].y) == 1);
                if (isDiagonal)
                {
                    distance -= model.MovementStats.DashDiagonallyBias;
                }

                else if (distance < minDistance)
                {
                    minDistance = distance;
                    closeDirection = model.MovementStats.DashDirections[i];
                }
            }

            // handle direction with NO input
            if (closeDirection == Vector2.zero)
            {
                if (isFlippingLeft)
                {
                    closeDirection = Vector2.left;
                }
                else
                {
                    closeDirection = Vector2.right;
                }
            }

            _dashDirection = closeDirection;
            _numberOfDashesUsed++;
            _isDashing = true;
            _dashTimer = 0f;
            _dashOnGroundTimer = model.MovementStats.TimeBtwDashesOnGround;

            ResetJumpValues();
            ResetWallJumpValues();
            StopWallSlide();

        }

        private void Dash()
        {
            if (_isDashing)
            {
                // stop the dash after the time
                _dashTimer += Time.fixedDeltaTime;
                if (_dashTimer >= model.MovementStats.DashTime)
                {
                    if (isGrounded)
                    {
                        ResetDashes();
                    }

                    _isAirDashing = false;
                    _isDashing = false;

                    if (!_isJumping && !_isWallJumping)
                    {
                        _dashFastFallTime = 0f;
                        _dashFastFallReleaseSpeed = _verticalVelocity;

                        if (!isGrounded)
                        {
                            _isDashFastFalling = true;
                        }
                    }

                    return;
                }

                _horizontalVelocity = model.MovementStats.DashSpeed * _dashDirection.x;

                if (_dashDirection.y != 0f || _isAirDashing)
                {
                    _verticalVelocity = model.MovementStats.DashSpeed * _dashDirection.y;
                }
            }

            // handle dash cut time
            else if (_isDashFastFalling)
            {
                if (_verticalVelocity > 0f)
                {
                    if (_dashFastFallTime < model.MovementStats.DashTimeForUpwardsCancle)
                    {
                        _verticalVelocity = Mathf.Lerp(_dashFastFallReleaseSpeed, 0f, (_dashFastFallTime / model.MovementStats.DashTimeForUpwardsCancle));
                    }
                    else if (_dashFastFallTime >= model.MovementStats.DashTimeForUpwardsCancle)
                    {
                        _verticalVelocity += model.MovementStats.JumpGravity * model.MovementStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                    }

                    _dashFastFallTime += Time.fixedDeltaTime;
                }

                else
                {
                    _verticalVelocity += model.MovementStats.JumpGravity * model.MovementStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
            }
        }

        #endregion

        /*#region Collision Checks

        private void CollisionChesks()
        {
            IsGrounded();
            BumpHead();
            IsTouchingWall();
        }

        private void IsGrounded()
        {
            Vector2 boxCastOrigin = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
            Vector2 boxCastSize = new Vector2(boxCollider.bounds.size.x, model.MovementStats.GroundDetectionRayLength);

            _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, model.MovementStats.GroundDetectionRayLength, model.MovementStats.GroundLayer);
            isGrounded = _groundHit.collider != null;

            if (model.MovementStats.DebugShowIsGroundedBox)
            {
                Color rayColor = Color.red;
                if (isGrounded)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Debug.DrawRay(new Vector2((boxCastOrigin.x - boxCastSize.x / 2), boxCastOrigin.y), Vector2.down * model.MovementStats.GroundDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCastOrigin.x + boxCastSize.x / 2), boxCastOrigin.y), Vector2.down * model.MovementStats.GroundDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCastOrigin.x - boxCastSize.x / 2), boxCastOrigin.y - model.MovementStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);

            }
        }

        private void BumpHead()
        {
            Vector2 boxCastOrigin = new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y);
            Vector2 boxCastSize = new Vector2(boxCollider.bounds.size.x * model.MovementStats.HeadWidth, model.MovementStats.HeadDetectionRayLength);

            _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, model.MovementStats.HeadDetectionRayLength, model.MovementStats.GroundLayer);
            bumpedHead = _headHit.collider != null;

            if (model.MovementStats.DebugShowHeadBumpBox)
            {
                Color rayColor = Color.red;
                if (bumpedHead)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                float headWidth = model.MovementStats.HeadWidth;

                Debug.DrawRay(new Vector2((boxCastOrigin.x - boxCastSize.x / 2 * headWidth), boxCastOrigin.y), Vector2.up * model.MovementStats.HeadDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCastOrigin.x + (boxCastSize.x / 2) * headWidth), boxCastOrigin.y), Vector2.up * model.MovementStats.HeadDetectionRayLength, rayColor);
                Debug.DrawRay(new Vector2((boxCastOrigin.x - boxCastSize.x / 2 * headWidth), boxCastOrigin.y + model.MovementStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);

            }
        }

        private void IsTouchingWall()
        {
            float originEndPoint = 0f;
            if (isFlippingLeft)
            {
                originEndPoint = capsuleCollider.bounds.min.x;
            }
            else
            {
                originEndPoint = capsuleCollider.bounds.max.x;
            }

            float adjustedHeight = capsuleCollider.size.y * model.MovementStats.WallDetectionRayHeightMultiplier;

            Vector2 boxCastOrigin = new Vector2(originEndPoint, capsuleCollider.bounds.center.y);
            Vector2 boxCastSize = new Vector2(model.MovementStats.WallDetectionRayLength, adjustedHeight);

            _wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, MyTransform.right, model.MovementStats.WallDetectionRayLength, model.MovementStats.GroundLayer);

            if (_wallHit.collider != null)
            {
                lastWallHit = _wallHit;
                isTouchingWall = true;
            }
            else
            {
                isTouchingWall = false;
            }

            if (model.MovementStats.DebugShowWallHitBox)
            {
                Color rayColor = Color.red;
                if (isGrounded)
                    rayColor = Color.green;
                else
                    rayColor = Color.red;

                Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
                Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
                Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
                Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

                Debug.DrawRay(boxBottomLeft, boxBottomRight, rayColor);
                Debug.DrawRay(boxBottomRight, boxTopRight, rayColor);
                Debug.DrawRay(boxTopRight, boxTopLeft, rayColor);
                Debug.DrawRay(boxTopLeft, boxBottomLeft, rayColor);

            }
        }

        #endregion*/

        #region Timer

        private void CountTimer()
        {
            // jump buffer
            _jumpBufferTimer -= Time.deltaTime;

            // jump coyote time
            if (!isGrounded)
            {
                _coyoteTimer -= Time.deltaTime;
            }
            else
            {
                _coyoteTimer = model.MovementStats.JumpCoyoteTime;
            }

            // wall jump biffer timer
            if (!ShouldApplyPostWallJumpBuffer())
            {
                _wallJumpPostBufferTimer -= Time.deltaTime;
            }

            // dash timer
            if (isGrounded)
            {
                _dashOnGroundTimer -= Time.deltaTime;
            }
        }

        #endregion

        #region Apply Velocity

        private void ApplyVelocity()
        {
            // clam fall speed
            if (!_isDashing)
            {
                _verticalVelocity = Mathf.Clamp(_verticalVelocity, -model.MovementStats.MaxFallSpeed, 50f);
            }

            else
            {
                _verticalVelocity = Mathf.Clamp(_verticalVelocity, -50, 50f);
            }

            rigidbody_.velocity = new Vector2(_horizontalVelocity, _verticalVelocity);
        }

        #endregion

        #region Attack

        private void ResetAttack()
        {
            _attackIndex = 0;
            _isAttacking = false;
        }

        private void AttackChesck()
        {
            if (_attackNormalWasPressed || _attackNormalWasPressed)
            {
                if (!_isAttacking)
                {
                    InitiateAttack();

                }
            }

        }

        private void InitiateAttack()
        {
            _isAttacking = true;

            //## PLAY AMIMATION
            PlayAnimation_Attack();
        }

        public void SpawnSlash()
        {
            Vector2 attackDirection = (Vector2)(_attackPointTransform.position - MyTransform.position);

            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            Quaternion attackRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            if (isFlippingLeft)
            {
                attackRotation = Quaternion.Euler(-180, attackRotation.eulerAngles.y, attackRotation.eulerAngles.z);
            }
            
            //## 
            PoolPrefab slashPrefab = MultipleObjectPool.Instance.GetPoolObject(model.AttackStats.AttackList[_attackIndex], rotation: attackRotation, parent: _attackPointTransform);
            slashPrefab?.Activate();
            
        }

        public void EndAttack()
        {
            _attackIndex = (_attackIndex + 1) % model.AttackStats.AttackList.Count;
            _isAttacking = false;
        }

        #endregion

        // ANIMATION

        #region ANIMATION

        private void PlayAnimation_JumpStart()
        {
            PlayerAnimation(ANIM_HASH_JumpStart);
        }

        private void PlayAnimation_Landing()
        {
            PlayerAnimation(ANIM_HASH_Landing);
        }

        private void PlayAnimation_Attack()
        {
            PlayerAnimation(ANIM_HASH_Attack, 0.15f);
        }

        //# BASE
        private void PlayerAnimation(int animationHash, float crossFadeTime = 0.2f)
        {
            _animator.CrossFadeInFixedTime(animationHash, crossFadeTime);
        }

        #endregion

        #region EFFECT

        private void PlayEffect_Dust()
        {
            _dustParticle.Play();
        }

        #endregion


    }

}
