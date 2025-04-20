using System.Collections;
using System.Threading;
using HIEU_NL.Utilities;
using HIEU_NL.Utilities.Move;
using UnityEngine;
using static HIEU_NL.Utilities.ParameterExtensions.Animation;

namespace HIEU_NL.Platformer.Script.Entity.Enemy.Viking
{
    public class Viking_State
    {
        /// <summary>
        /// IDLE STATE
        /// </summary>
        public class IdleState : BaseEnemyState.IdleState
        {
            private Coroutine _idleCoroutine;
    
            public IdleState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Idle, transitionAnimDuration);
    
                _idleCoroutine = owner.StartCoroutine(Idle_Coroutine());
    
            }
    
            public override void OnExit()
            {
                if (_idleCoroutine != null)
                {
                    owner.StopCoroutine(_idleCoroutine);
                    _idleCoroutine = null;
                }
                
                base.OnExit();
            }
            
            //#
            
            private IEnumerator Idle_Coroutine()
            {
                yield return new WaitForSeconds(owner.Stats.IdleTimeMax);
                owner.Finish_IdleState();
            }
    
        }
        
        /// <summary>
        /// PATROL STATE
        /// </summary>
        public class PatrolState : BaseEnemyState.PatrolState
        {
            private Vector2 _patrolDirection;
            private float _patrolSpeed;
    
            public PatrolState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
    
                if (HasReachToDestination())
                {
                    owner.SetIsFlippingLeft(!owner.IsFlippingLeft);
                }
    
                _patrolSpeed = 0f;
                _patrolDirection = owner.IsFlippingLeft ? Vector2.left : Vector2.right;
    
                animator.CrossFadeInFixedTime(ANIM_HASH_Run, transitionAnimDuration);
    
            }
    
            public override void Update()
            {
                if (HasReachToDestination())
                {
                    HandleReachToDestionation();
                }
                else
                {
                    _patrolSpeed = Mathf.Lerp(_patrolSpeed, owner.Stats.PatrolSpeed, owner.Stats.PatrolSpeed * Time.deltaTime);
                    owner.MyTransform.position += (Vector3)_patrolDirection * (_patrolSpeed * Time.deltaTime);
                }
    
            }
    
            //# 
    
            private void HandleReachToDestionation()
            {
                owner.Finish_PatrolState();
            }
    
            private bool HasReachToDestination()
            {
                return owner.IsFlippingLeft && owner.MyTransform.position.x <= owner.PatrolPositionLeft.x
                    || !owner.IsFlippingLeft && owner.MyTransform.position.x >= owner.PatrolPositionRight.x
                    || owner.IsTouchingWall;
            }
    
        }
        
        /// <summary>
        /// CHASE STATE
        /// </summary>
        public class ChaseState : BaseEnemyState.ChaseState
        {
            private Vector2 _chaseDirection;
            private float _chaseSpeed;
            
            private CancellationTokenSource _cancellationTokenSource;
    
            public ChaseState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
                
                _chaseSpeed = 0f;
                
                animator.CrossFadeInFixedTime(ANIM_HASH_Run, transitionAnimDuration);
            }
    
            public override void Update()
            {
                if (owner.PlayerInChaseRange())
                {
                    if (LookingTowardThePlayer())
                    {
                        owner.SetIsFlippingLeft(!owner.IsFlippingLeft);
                    }

                    _chaseSpeed = Mathf.Lerp(_chaseSpeed, owner.Stats.ChaseSpeed, owner.Stats.ChaseSpeed * Time.deltaTime);
                    _chaseDirection = owner.TargetTransform.position.x < owner.MyTransform.position.x ? Vector2.left : Vector2.right;

                    owner.MyTransform.position += (Vector3)_chaseDirection * (_chaseSpeed * Time.deltaTime);
                }
                else
                {
                    owner.Finish_ChaseState();
                }
            }

            public override void OnExit()
            {
                _cancellationTokenSource?.Cancel();
                // _cancellationTokenSource?.Dispose();
                
                base.OnExit();
            }

            private bool LookingTowardThePlayer()
            {
                return owner.IsFlippingLeft && owner.MyTransform.position.x <= owner.TargetTransform.position.x
                    || !owner.IsFlippingLeft && owner.MyTransform.position.x >= owner.TargetTransform.position.x;
            }
    
        }
        
        /// <summary>
        /// ATTACK STATE
        /// </summary>
        public class AttackState : BaseEnemyState.AttackState
        {
            private Viking _viking;
            private CurveMove _curveMove;
            private int _attackAnimHash;
            private int _attackIndex;
            private Coroutine _attackCoroutine;
            private bool _isPlayedJumpEndAnimation;
            private bool _isPlayedAttackAnimation;
    
            public AttackState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
    
            public override void OnEnter()
            {
                base.OnEnter();
                
                _viking = owner as Viking;
                _isPlayedAttackAnimation = false;
                _isPlayedJumpEndAnimation = false;
                
                if (owner.AttackIndex == 3 || owner.AttackIndex == 4)
                {
                    _attackIndex = Random.Range(3,5);
                }
                else
                {
                    _attackIndex = owner.AttackIndex;
                }
    
                if (_attackIndex == 3)
                {
                    animator.CrossFadeInFixedTime(ANIM_HASH_JumpStart, transitionAnimDuration);

                    Vector2 startPosition = owner.MyTransform.position;
                    float widthOffset = owner.IsFlippingLeft
                        ? Random.Range(_viking.WidthTargetOffset.x, _viking.WidthTargetOffset.y)
                        : Random.Range(_viking.WidthTargetOffset.x, _viking.WidthTargetOffset.y) * -1;
                    Vector2 endPosition = owner.TargetTransform.position
                        .With(y: owner.MyTransform.position.y)
                        .Add(x: widthOffset);
                    
                    _curveMove = new CurveMove(owner.MyTransform, startPosition,
                        endPosition, _viking.JumpHeight, _viking.JumpSpeed);
                    
                    _curveMove.OnFinished += CurveMove_OnFinished;
                }
                else
                {
                    PlayAttackAnimation();
                }
    
            }

            private void CurveMove_OnFinished()
            {
                _curveMove.OnFinished -= CurveMove_OnFinished; 
                _curveMove = null;
            }

            public override void Update()
            {
                base.Update();

                if (_curveMove != null && _attackIndex == 3)
                {
                    _curveMove?.Moving();

                    if (!_isPlayedJumpEndAnimation && _curveMove.GetProgressPercentage() > 0.5f)
                    {
                        _isPlayedJumpEndAnimation = true;
                        animator.CrossFadeInFixedTime(ANIM_HASH_JumpEnd, transitionAnimDuration);
                    }
                    else if (!_isPlayedAttackAnimation && _curveMove.GetProgressPercentage() > 0.8f)
                    {
                        _isPlayedAttackAnimation = true;
                        PlayAttackAnimation();
                    }
                    
                }
                
            }

            public override void OnExit()
            {
                if (_attackCoroutine != null)
                {
                    owner.StopCoroutine(_attackCoroutine);
                    _attackCoroutine = null;
                }

                base.OnExit();
            }
            
            //#

            private void PlayAttackAnimation()
            {
                _attackAnimHash = ALL_ANIM_HASH[owner.Stats.AttackDataArray[_attackIndex].AttackAnimType];
                animator.CrossFadeInFixedTime(_attackAnimHash, transitionAnimDuration);
                    
                _attackCoroutine = owner.StartCoroutine(Attack_Coroutine());
            }
            
            private IEnumerator Attack_Coroutine()
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(_attackAnimHash));
                float attackTime = animator.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(attackTime);
                owner.Finish_AttackState();
            }
            
    
        }
        
        /// <summary>
        /// PAIN STATE
        /// </summary>
        public class PainState : BaseEnemyState.PainState
        {
            private Coroutine _painCoroutine;
            private int _animHash;

            public PainState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                base.OnEnter();
                
                blendAnimCoefficient = 0.8f;

                _animHash = ANIM_HASH_Pain;
                animator.CrossFadeInFixedTime(_animHash, transitionAnimDuration);
    
                _painCoroutine = owner.StartCoroutine(Pain_Coroutine());
    
            }
    
            public override void OnExit()
            {
                owner.StopCoroutine(_painCoroutine);
                _painCoroutine = null;
                owner.Finish_PainState();
            }
            
            //#
            
            private IEnumerator Pain_Coroutine()
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(_animHash));
                float painTime = animator.GetCurrentAnimatorStateInfo(0).length * blendAnimCoefficient;
                yield return new WaitForSeconds(painTime);
                owner.Finish_PainState();
            }
            
        }
        
        /// <summary>
        /// DIE STATE
        /// </summary>
        public class DeadState : BaseEnemyState.DeadState 
        {
            private Coroutine _deadCoroutine;
            
            public DeadState(BaseEnemy owner, Animator animator) : base(owner, animator) { }
            
            public override void OnEnter()
            {
                owner.Begin_DeadState();

                animator.CrossFadeInFixedTime(ANIM_HASH_Dead, transitionAnimDuration);

                _deadCoroutine = owner.StartCoroutine(Dead_Coroutine());
                
            }
        
            public override void OnExit()
            {
                owner.StopCoroutine(_deadCoroutine);
                _deadCoroutine = null;
                
                base.OnExit();
            }
            
            //#
            private IEnumerator Dead_Coroutine()
            {
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(ANIM_HASH_Dead));
                float painTime = animator.GetCurrentAnimatorStateInfo(0).length * blendAnimCoefficient;
                yield return new WaitForSeconds(painTime);
                
                owner.Deactivate();
            }

        }
        
        /* using UnityEngine;
        using System.Collections;

        public class ForwardArcMovement2D : MonoBehaviour
        {
            [Header("Movement Settings")]
            [Range(0.1f, 20f)]
            public float moveDistance = 5f;     // Khoảng cách di chuyển
            [Range(0.1f, 10f)]
            public float arcHeight = 2f;        // Chiều cao của vòng cung
            [Range(0.1f, 10f)]
            public float movementDuration = 1f; // Thời gian di chuyển

            [Header("Rotation Settings")]
            public bool rotateTowardsMoveDirection = true;
            public float rotationSpeed = 10f;

            [Header("Debug")]
            public bool visualizeArc = true;
            public int arcResolution = 30;
            public Color arcColor = Color.yellow;

            private Vector2 _startPosition;
            private Vector2 _endPosition;
            private bool _isMoving = false;
            private Vector2 _movementDirection;

            // Bắt đầu di chuyển theo vòng cung theo hướng forward hiện tại
            public void StartArcMovementInForwardDirection()
            {
                if (!_isMoving)
                {
                    StopAllCoroutines();

                    // Lấy vị trí hiện tại làm điểm bắt đầu
                    _startPosition = transform.position;

                    // Lấy hướng forward của đối tượng
                    _movementDirection = transform.right; // Trong Unity 2D, trục x thường là forward

                    // Tính toán điểm đích dựa trên hướng forward và khoảng cách di chuyển
                    _endPosition = _startPosition + (_movementDirection * moveDistance);

                    StartCoroutine(MoveInArc());
                }
            }

            // Di chuyển đối tượng theo đường cong
            private IEnumerator MoveInArc()
            {
                _isMoving = true;
                float elapsedTime = 0f;

                while (elapsedTime < movementDuration)
                {
                    float t = elapsedTime / movementDuration;

                    // Tính toán vị trí trên đường cong
                    Vector2 arcPosition = CalculateArcPoint(_startPosition, _endPosition, arcHeight, t);
                    transform.position = arcPosition;

                    // Quay đối tượng theo hướng di chuyển nếu được bật
                    if (rotateTowardsMoveDirection && t > 0)
                    {
                        // Tính vector hướng di chuyển tại thời điểm hiện tại trên đường cong
                        Vector2 tangent = CalculateArcTangent(_startPosition, _endPosition, arcHeight, t);
                        RotateTowardsDirection(tangent);
                    }

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Đảm bảo đối tượng đến đúng vị trí cuối
                transform.position = _endPosition;
                _isMoving = false;
            }

            // Tính toán vị trí trên đường cong dựa trên tham số t (0-1)
            public Vector2 CalculateArcPoint(Vector2 start, Vector2 end, float height, float t)
            {
                // Điểm giữa của đường thẳng từ start đến end
                Vector2 midPoint = Vector2.Lerp(start, end, 0.5f);

                // Tính toán vector hướng lên (trong không gian 2D)
                Vector2 direction = (end - start).normalized;
                Vector2 perpendicular = new Vector2(-direction.y, direction.x); // Vector vuông góc với hướng di chuyển

                // Điểm cao nhất của vòng cung
                Vector2 arcMidPoint = midPoint + perpendicular * height;

                // Sử dụng Quadratic Bezier Curve
                float oneMinusT = 1f - t;
                return oneMinusT * oneMinusT * start + 2f * oneMinusT * t * arcMidPoint + t * t * end;
            }

            // Tính vector tiếp tuyến tại một điểm trên đường cong
            public Vector2 CalculateArcTangent(Vector2 start, Vector2 end, float height, float t)
            {
                // Điểm giữa của đường thẳng từ start đến end
                Vector2 midPoint = Vector2.Lerp(start, end, 0.5f);

                // Tính toán vector hướng lên (trong không gian 2D)
                Vector2 direction = (end - start).normalized;
                Vector2 perpendicular = new Vector2(-direction.y, direction.x);

                // Điểm cao nhất của vòng cung
                Vector2 arcMidPoint = midPoint + perpendicular * height;

                // Đạo hàm của đường cong Bezier bậc 2
                // P'(t) = 2(1-t)(P1-P0) + 2t(P2-P1)
                Vector2 tangent = 2 * (1 - t) * (arcMidPoint - start) + 2 * t * (end - arcMidPoint);
                return tangent.normalized;
            }

            // Quay đối tượng theo hướng di chuyển
            private void RotateTowardsDirection(Vector2 direction)
            {
                if (direction.sqrMagnitude < 0.001f)
                    return;

                // Tính góc quay dựa trên vector hướng
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Quay đối tượng đến góc này
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Cho phép kích hoạt di chuyển từ bên ngoài
            public void Jump()
            {
                StartArcMovementInForwardDirection();
            }

            // Vẽ đường cong trong Scene View để dễ dàng điều chỉnh
            private void OnDrawGizmos()
            {
                if (!visualizeArc || !isActiveAndEnabled)
                    return;

                Vector2 start = transform.position;
                Vector2 direction = transform.right; // Trong Unity 2D, trục x thường là forward
                Vector2 end = start + (direction * moveDistance);

                Gizmos.color = arcColor;

                // Vẽ đường thẳng nối điểm đầu và cuối
                Gizmos.DrawLine(start, end);

                // Vẽ đường cong bằng cách nối các điểm
                Vector2 previousPoint = start;

                for (int i = 1; i <= arcResolution; i++)
                {
                    float t = i / (float)arcResolution;
                    Vector2 point = CalculateArcPoint(start, end, arcHeight, t);
                    Gizmos.DrawLine(previousPoint, point);
                    previousPoint = point;

                    // Vẽ vector hướng di chuyển tại một số điểm
                    if (i % 5 == 0)
                    {
                        Vector2 tangent = CalculateArcTangent(start, end, arcHeight, t);
                        Gizmos.DrawRay(point, tangent * 0.5f);
                    }
                }
            }
        }*/
     
    }
    
}


