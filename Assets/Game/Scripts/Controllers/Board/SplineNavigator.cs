using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

namespace Controller.Board
{
    public class SplineNavigator : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private float moveSpeed = 5f;

        private Tween _currentMoveTween;

        public void MoveToNextKnot(Vector3 targetKnotPosition)
        {
            // Try to find a spline path from the current position 
            // to the target knot position
            if (TryGetSplinePath(transform.position, targetKnotPosition, out Spline spline, out float startT, out float endT))
            {
                // Calculate the curve distance to determine the duration
                float distance = CalculateDistanceBetweenPoints(spline, startT, endT);
                float duration = distance / moveSpeed;

                MoveAlongSpline(spline, startT, endT, duration);
            }
            else
            {
                // 2. Fallback: Straight line movement if no spline path is found
                float distance = Vector3.Distance(transform.position, targetKnotPosition);
                float duration = distance / moveSpeed;
                MoveInStraightLine(targetKnotPosition, duration);
            }
        }

        private float CalculateDistanceBetweenPoints(Spline spline,
            float startT,
            float endT)
        {
            // 1. Get the total length of the spline
            float totalLength = SplineUtility.CalculateLength(spline,
                splineContainer.transform.localToWorldMatrix);

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

            foreach (var spline in splineContainer.Splines)
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
            out float time,
            float threshold = 0.1f)
        {
            // Converts the global position to the local space of the Container
            Vector3 localPos = splineContainer.transform.InverseTransformPoint(position);

            // SplineUtility.GetNearestPoint returns the nearest position and 
            // the 't' (0 to 1)
            SplineUtility.GetNearestPoint(spline, localPos, out _, out time);

            // Verify if the point found is actually close to the knot 
            // (within the threshold)
            Vector3 pointOnSpline = (Vector3)spline.EvaluatePosition(time);
            return Vector3.Distance(pointOnSpline, localPos) < threshold;
        }

        private void MoveAlongSpline(Spline spline,
            float startT,
            float endT,
            float duration)
        {
            _currentMoveTween?.Kill();

            // Create a generic value from 0 to 1 for DoTween to animate the progress
            float progress = 0;
            _currentMoveTween =
                DOTween.To(() => progress, x => progress = x, 1f, duration)
                .OnUpdate(() =>
                {
                    // Interpolate between the start T and end T on the selected spline
                    float currentT = Mathf.Lerp(startT, endT, progress);
                    Vector3 localPos = (Vector3)spline.EvaluatePosition(currentT);
                    transform.position = splineContainer.transform.TransformPoint(localPos);

                    // Rotates the object to face the direction of 
                    // movement along the spline
                    Vector3 forward = (Vector3)spline.EvaluateTangent(currentT);
                    if (forward != Vector3.zero)
                        transform.forward = splineContainer.transform.TransformDirection(forward);
                })
                .SetEase(Ease.Linear);
        }

        private void MoveInStraightLine(Vector3 targetPos, float duration)
        {
            _currentMoveTween?.Kill();
            _currentMoveTween =
                transform.DOMove(targetPos, duration).SetEase(Ease.Linear);
        }

        private void OnDestroy()
        {
            _currentMoveTween?.Kill();
        }
    }
}
