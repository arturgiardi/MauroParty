using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
#endif


namespace Controller.Board
{
    public class BoardMovementController : MonoBehaviour
    {
        public TileController Tile { get; set; }
        private BoardController _boardController { get; set; }
        private float _movementSpeed = 2f;
        private Tween currentTween;
        private SplineContainer SplineContainer => _boardController.SplineContainer;

        // Guardamos em qual spline o personagem está atualmente
        private int _currentSplineIndex = 0;
        private float _currentT = 0f;
        public void Setup(BoardController boardController)
        {
            _boardController = boardController;
        }

        public void MoveToTile(TileController tile)
        {
            currentTween?.Kill();
            if (Tile == null)
                DefaultMovement(tile);
            else
                MoveAlongBoard(tile);
        }

        private void DefaultMovement(TileController tile)
        {
            var distance = Vector3.Distance(transform.position, tile.transform.position);
            float duration = distance / _movementSpeed;

            currentTween = transform.DOMove(tile.transform.position, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Tile = tile;
                });
        }

        private void MoveAlongBoard(TileController tile)
        {
            // 1. Encontrar o 't' do destino na spline atual
            // Tentamos primeiro encontrar a posição do tile dentro da spline que o jogador já está seguindo
            if (TryGetTOnSpline(tile.transform.position, _currentSplineIndex, out float targetT))
            {
                AnimateMovement(_currentSplineIndex, _currentT, targetT);
            }
            else
            {
                // 2. Se o tile não existir na spline atual, procuramos em outras
                // Isso acontece em bifurcações ou trocas de caminho
                if (SplineUtility.GetNearestPoint(SplineContainer, tile.transform.position, out float3 _, out float newT, out int newSplineIndex))
                {
                    _currentSplineIndex = newSplineIndex;
                    AnimateMovement(_currentSplineIndex, _currentT, newT);
                }
            }
        }

        private bool TryGetTOnSpline(Vector3 position, int splineIndex, out float t)
        {
            var spline = SplineContainer[splineIndex];
            // Converte a posição do mundo para o espaço local da spline
            float3 localPos = SplineContainer.transform.InverseTransformPoint(position);
            return SplineUtility.GetNearestPoint(spline, localPos, out float3 _, out t);
        }

        private void AnimateMovement(int splineIndex, float startT, float endT)
        {
            float distance = SplineContainer[splineIndex].GetLength() * Mathf.Abs(endT - startT);
            float duration = distance / _movementSpeed;

            currentTween = DOTween.To(() => startT, x =>
            {
                _currentT = x;
                transform.position = SplineContainer.EvaluatePosition(splineIndex, x);

                float3 tangent = SplineContainer.EvaluateTangent(splineIndex, x);
                if (!tangent.Equals(float3.zero))
                    transform.rotation = Quaternion.LookRotation(tangent);

            }, endT, duration)
            .SetEase(Ease.Linear);
        }

        private void OnDestroy()
        {
            currentTween?.Kill();
        }


    }
}
