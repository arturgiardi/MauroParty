using System.Collections.Generic;
using Game.Controller.CustomSpline;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Splines;

namespace Game.Controller.Board
{
    [RequireComponent(typeof(SplineNodeData))]
    public class TileController : MonoBehaviour
    {
        [field: SerializeField] private SplineNodeData Node { get; set; }
        [field: SerializeField] private BaseTileBodyController CoinGainPrefab { get; set; }
        [field: SerializeField] private BaseTileBodyController CoinLossPrefab { get; set; }
        [field: SerializeField] private BaseTileBodyController StarPrefab { get; set; }
        [field: SerializeField] private BaseTileBodyController IntersectionPrefab { get; set; }
        [field: SerializeField] private BaseTileBodyController TileBody { get; set; }

        public SplineKnotIndex Id { get; private set; }
        private List<SplineKnotIndex> _nextTiles;

        public void Setup()
        {
            Id = Node.KnotIndex;
            _nextTiles = Node.GetNextIds();
        }

        public void SetCoinGain()
        {
            ClearTileBody();
            IntantiateTile(CoinGainPrefab);
        }
        public void SetCoinLoss()
        {
            ClearTileBody();
            IntantiateTile(CoinLossPrefab);
        }

        public void SetStar()
        {
            ClearTileBody();
            IntantiateTile(StarPrefab);
        }

        public void SetIntersection()
        {
            ClearTileBody();
            IntantiateTile(IntersectionPrefab);
        }


        private void IntantiateTile(BaseTileBodyController prefab)
        {
            if (Application.isPlaying)
                TileBody = Instantiate(prefab, transform);
#if UNITY_EDITOR
            else
                TileBody = (BaseTileBodyController)PrefabUtility.InstantiatePrefab(
                    prefab,
                    transform);
#endif
        }

        private void ClearTileBody()
        {
            if (TileBody != null)
            {
                if (Application.isPlaying)
                    Destroy(TileBody.gameObject);
#if UNITY_EDITOR
                else
                    DestroyImmediate(TileBody.gameObject);
#endif
                TileBody = null;
            }
        }
    }

}
