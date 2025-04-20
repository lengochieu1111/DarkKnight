using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIEU_NL.Utilities.Move
{
    public class CurveMove
    {
        public event Action OnFinished;

        private Transform _transform;

        private Vector2 _startPosition;
        private Vector2 _endPosition;

        private float _arcHeight;

        private float _moveTimeMax;
        private float _moveTimeElapsed;

        private bool _rotateTowardsMoveDirection;
        private float _rotationSpeed;
        
        private int arcResolution = 30;

        private List<Vector2> _arcPoints = new List<Vector2>();

        public float MoveTimeMax => _moveTimeMax;

        public CurveMove(Transform transform, Vector2 startPosition, Vector2 endPosition, float arcHeight,
            float moveSpeed, bool rotateTowardsMoveDirection = false, float rotationSpeed = 10f)
        {
            _transform = transform;
            _startPosition = startPosition;
            _endPosition = endPosition;
            _arcHeight = arcHeight;

            _rotateTowardsMoveDirection = rotateTowardsMoveDirection;
            _rotationSpeed = rotationSpeed;

            CalculateArcPath();
                
            _moveTimeElapsed = 0f;
            _moveTimeMax = CalculateArcLength() / moveSpeed;

        }


        //#


        /*public void Moving()
        {

            if (_moveTimeElapsed < _moveTimeMax)
            {
                float t = _moveTimeElapsed / _moveTimeMax;

                Vector2 arcPosition = CalculateArcPoint(_startPosition, _endPosition, _arcHeight, t);
                _transform.position = arcPosition;

                if (_rotateTowardsMoveDirection && t > 0)
                {
                    Vector2 tangent = CalculateArcTangent(_startPosition, _endPosition, _arcHeight, t);
                    RotateTowardsDirection(tangent);
                }

                _moveTimeElapsed += Time.deltaTime;
            }
            else
            {
                _transform.position = _endPosition;
                OnFinished?.Invoke();
            }

        }

        private Vector2 CalculateArcPoint(Vector2 start, Vector2 end, float height, float t)
        {
            Vector2 midPoint = Vector2.Lerp(start, end, 0.5f);

            Vector2 upDirection = Vector2.up;

            Vector2 moveDirection = (end - start).normalized;

            Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);

            if (Vector2.Dot(perpendicular, upDirection) < 0)
            {
                perpendicular = -perpendicular;
            }

            Vector2 arcMidPoint = midPoint + perpendicular * height;

            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * start + 2f * oneMinusT * t * arcMidPoint + t * t * end;
        }

        private Vector2 CalculateArcTangent(Vector2 start, Vector2 end, float height, float t)
        {
            Vector2 midPoint = Vector2.Lerp(start, end, 0.5f);

            Vector2 moveDirection = (end - start).normalized;

            Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);

            if (Vector2.Dot(perpendicular, Vector2.up) < 0)
            {
                perpendicular = -perpendicular;
            }

            Vector2 arcMidPoint = midPoint + perpendicular * height;

            Vector2 tangent = 2 * (1 - t) * (arcMidPoint - start) + 2 * t * (end - arcMidPoint);
            return tangent.normalized;
        }

        private void RotateTowardsDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
                return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }*/



        public void Moving()
        {
            MoveInArcWithConstantSpeed();
        }
        

        private void CalculateArcPath()
        {
            _arcPoints.Clear();

            int numPoints = Mathf.Max(arcResolution, 50); 

            _arcPoints.Add(_startPosition);

            for (int i = 1; i <= numPoints; i++)
            {
                float t = i / (float)numPoints;
                Vector2 point = CalculateArcPoint(_startPosition, _endPosition, _arcHeight, t);
                _arcPoints.Add(point);
            }
        }

        
        // Tính toán chiều dài đường cong dựa trên các điểm đã tính
        private float CalculateArcLength()
        {
            float length = 0f;
            for (int i = 1; i < _arcPoints.Count; i++)
            {
                length += Vector2.Distance(_arcPoints[i - 1], _arcPoints[i]);
            }

            return length;
        }

        
        // Di chuyển đối tượng theo đường cong với tốc độ cố định
        private void MoveInArcWithConstantSpeed()
        {
            if (_moveTimeElapsed < _moveTimeMax)
            {
                float t = _moveTimeElapsed / _moveTimeMax;

                Vector2 arcPosition = CalculateArcPoint(_startPosition, _endPosition, _arcHeight, t);
                _transform.position = arcPosition;

                if (_rotateTowardsMoveDirection && t > 0)
                {
                    Vector2 tangent = CalculateArcTangent(_startPosition, _endPosition, _arcHeight, t);
                    RotateTowardsDirection(tangent);
                }

                _moveTimeElapsed += Time.deltaTime;
            }
            else
            {
                _transform.position = _endPosition;
                OnFinished?.Invoke();
            }

        }

        
        // Tính toán vị trí trên đường cong dựa trên tham số t (0-1)
        private Vector2 CalculateArcPoint(Vector2 start, Vector2 end, float height, float t)
        {
            Vector2 midPoint = Vector2.Lerp(start, end, 0.5f);

            Vector2 moveDirection = (end - start).normalized;

            Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);

            if (Vector2.Dot(perpendicular, Vector2.up) < 0)
            {
                perpendicular = -perpendicular;
            }

            Vector2 arcMidPoint = midPoint + perpendicular * height;

            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * start + 2f * oneMinusT * t * arcMidPoint + t * t * end;
        }

        
        // Tính vector tiếp tuyến tại một điểm trên đường cong
        private Vector2 CalculateArcTangent(Vector2 start, Vector2 end, float height, float t)
        {
            Vector2 midPoint = Vector2.Lerp(start, end, 0.5f);

            Vector2 moveDirection = (end - start).normalized;

            Vector2 perpendicular = new Vector2(-moveDirection.y, moveDirection.x);

            if (Vector2.Dot(perpendicular, Vector2.up) < 0)
            {
                perpendicular = -perpendicular;
            }

            Vector2 arcMidPoint = midPoint + perpendicular * height;

            Vector2 tangent = 2 * (1 - t) * (arcMidPoint - start) + 2 * t * (end - arcMidPoint);
            return tangent.normalized;
        }

        
        private void RotateTowardsDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
                return;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            _transform.rotation =
                Quaternion.Slerp(_transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        public float GetProgressPercentage() => _moveTimeElapsed / _moveTimeMax;

    }
}