using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
#endif


namespace Controller.Board
{
    public class SplineNavigator : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private float moveSpeed = 5f;

        private Tween _currentMoveTween;

        public void MoveToNextKnot(Vector3 targetKnotPosition)
        {
            // 1. Tenta encontrar a spline que conecta a posição atual ao destino
            if (TryGetSplinePath(transform.position, targetKnotPosition, out Spline spline, out float startT, out float endT))
            {
                // Calcula a distância real na curva para determinar a duração
                float distance = CalculateDistanceBetweenPoints(spline, startT, endT);
                float duration = distance / moveSpeed;

                MoveAlongSpline(spline, startT, endT, duration);
            }
            else
            {
                float distance = Vector3.Distance(transform.position, targetKnotPosition);
                float duration = distance / moveSpeed;
                // 2. Fallback: Movimento em linha reta caso não haja conexão por spline
                MoveInStraightLine(targetKnotPosition, duration);
            }
        }

        private float CalculateDistanceBetweenPoints(Spline spline,
            float startT,
            float endT)
        {
            // 1. Pegamos o comprimento TOTAL da spline usando a assinatura que você 
            // confirmou
            float totalLength = SplineUtility.CalculateLength(spline, 
                splineContainer.transform.localToWorldMatrix);
            
            // 2. Calculamos a distância aproximada do trecho
            // Em splines, a distância não é perfeitamente linear ao T, 
            // mas para knots adjacentes, a proporção (delta T * total) é uma excelente 
            // aproximação.
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
                // Verifica se esta spline contém knots próximos à posição atual e à de destino
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
            // Converte a posição global para o espaço local do Container
            Vector3 localPos = splineContainer.transform.InverseTransformPoint(position);

            // SplineUtility.GetNearestPoint retorna a posição mais próxima e o 't' (0 a 1)
            SplineUtility.GetNearestPoint(spline, localPos, out _, out time);

            // Verifica se o ponto encontrado está realmente perto do knot 
            // (dentro do threshold)
            Vector3 pointOnSpline = (Vector3)spline.EvaluatePosition(time);
            return Vector3.Distance(pointOnSpline, localPos) < threshold;
        }

        private void MoveAlongSpline(Spline spline,
            float startT,
            float endT,
            float duration)
        {
            _currentMoveTween?.Kill();

            // Criamos um valor genérico de 0 a 1 para o DoTween animar o progresso
            float progress = 0;
            _currentMoveTween =
                DOTween.To(() => progress, x => progress = x, 1f, duration)
                .OnUpdate(() =>
                {
                    // Interpola entre o T inicial e o T final na spline selecionada
                    float currentT = Mathf.Lerp(startT, endT, progress);
                    Vector3 localPos = (Vector3)spline.EvaluatePosition(currentT);
                    transform.position = splineContainer.transform.TransformPoint(localPos);

                    // Opcional: Rotacionar para a direção do caminho
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
