using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

namespace Controller.Board
{
    public class SplineMovementController : MonoBehaviour
    {
        [field: SerializeField] private float MoveSpeed { get; set; } = 5f;
        [field: SerializeField] private float KnotThreshold { get; set; } = 0.1f;
        [field: SerializeField] private float RotationSpeed { get; set; } = 10f;
        private SplineContainer _splineContainer;
        private Tween _moveTween;
        private Tween _rotationTween;

        public void Setup(SplineContainer container)
        {
            _splineContainer = container;
        }

        public void MoveToKnot(Vector3 targetKnotPosition)
        {
            // Try to find a spline path from the current position 
            // to the target knot position
            if (TryGetSplinePath(transform.position,
                targetKnotPosition,
                out Spline spline,
                out float startT,
                out float endT))
            {
                if (endT < startT)
                    endT += 1f;

                // Calculate the curve distance to determine the duration
                float distance = CalculateDistanceBetweenPoints(spline, startT, endT);
                float duration = distance / MoveSpeed;

                MoveAlongSpline(spline, startT, endT, duration);
            }
            else
            {
                // 2. Fallback: Straight line movement if no spline path is found
                float distance = Vector3.Distance(transform.position, targetKnotPosition);
                float duration = distance / MoveSpeed;
                MoveInStraightLine(targetKnotPosition, duration);
            }
        }

        private float CalculateDistanceBetweenPoints(Spline spline,
            float startT,
            float endT)
        {
            // 1. Get the total length of the spline
            float totalLength = SplineUtility.CalculateLength(spline,
                _splineContainer.transform.localToWorldMatrix);

            //2. Calculate the approximate distance between the two points on the spline
            // In splines, the distance is not perfectly linear with T, 
            // but for adjacent knots, the proportion (delta T * total) is an 
            // excellent approximation.
            return Mathf.Abs(endT - startT) * totalLength;
        }

        private bool TryGetSplinePath(Vector3 currentPos,
            Vector3 targetPos,
            out Spline foundSpline,
            out float startT,
            out float endT)
        {
            foundSpline = null;
            startT = 0f;
            endT = 0f;

            foreach (var spline in _splineContainer.Splines)
            {
                // Verify if this spline contains knots close to the current position 
                // and the target position
                bool hasStart = TryGetTimeAtPosition(spline, currentPos, out float tStart);
                bool hasEnd = TryGetTimeAtPosition(spline, targetPos, out float tEnd);

                if (hasStart && hasEnd)
                {
                    foundSpline = spline;
                    startT = tStart;
                    endT = tEnd;
                    return true;
                }
            }
            return false;
        }

        private bool TryGetTimeAtPosition(Spline spline,
            Vector3 position,
            out float time)
        {
            // Converts the global position to the local space of the Container
            Vector3 localPos = _splineContainer.transform.InverseTransformPoint(position);

            // SplineUtility.GetNearestPoint returns the nearest position and 
            // the 't' (0 to 1)
            SplineUtility.GetNearestPoint(spline, localPos, out _, out time);

            // Verify if the point found is actually close to the knot 
            // (within the threshold)
            Vector3 pointOnSpline = (Vector3)spline.EvaluatePosition(time);
            return Vector3.Distance(pointOnSpline, localPos) < KnotThreshold;
        }

        private void MoveAlongSpline(Spline spline,
            float startT,
            float endT,
            float duration)
        {
            _moveTween?.Kill();

            // Create a generic value from 0 to 1 for DoTween to animate the progress
            float progress = 0;
            _moveTween =
                DOTween.To(() => progress, x => progress = x, 1f, duration)
                .OnUpdate(() =>
                {
                    // Interpolate between the start T and end T on the selected spline
                    float currentT = Mathf.Lerp(startT, endT, progress);

                    float normalizedT = currentT % 1f;
                    if (normalizedT < 0)
                        normalizedT += 1f;

                    Vector3 localPos = (Vector3)spline.EvaluatePosition(normalizedT);
                    transform.position =
                        _splineContainer.transform.TransformPoint(localPos);

                    // Rotates the object to face the direction of 
                    // movement along the spline
                    Vector3 tangent = (Vector3)spline.EvaluateTangent(normalizedT);
                    if (tangent != Vector3.zero)
                    {
                        Vector3 worldDirection = 
                            _splineContainer.transform.TransformDirection(tangent);
                        Quaternion targetRotation = Quaternion.LookRotation(worldDirection);

                        // Slerp makes the rotation smooth
                        transform.rotation = Quaternion.Slerp(transform.rotation, 
                            targetRotation, Time.deltaTime * RotationSpeed);
                    }
                })
                .SetEase(Ease.Linear);
        }

        private void MoveInStraightLine(Vector3 targetPos, float duration)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            //Rotate to face the target
            if (direction != Vector3.zero)
            {
                float lookDuration = Mathf.Min(.5f, duration);
                _rotationTween = transform.DOLookAt(targetPos, lookDuration);
            }
            _moveTween?.Kill();
            _moveTween =
                transform.DOMove(targetPos, duration).SetEase(Ease.Linear);
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
            _rotationTween?.Kill();
        }
    }
}
