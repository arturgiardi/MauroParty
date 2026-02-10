#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Controller.Board
{
    [CustomEditor(typeof(TileController))]
    public class TileControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TileController script = (TileController)target;

            GUILayout.Space(10);
            GUILayout.Label("Editor Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Set Coin Gain"))
                SetTileBody(script, TileType.CoinGain);

            if (GUILayout.Button("Set Coin Loss"))
                SetTileBody(script, TileType.CoinLoss);

            if (GUILayout.Button("Set Star"))
                SetTileBody(script, TileType.Star);

            if (GUILayout.Button("Set Intersection"))
                SetTileBody(script, TileType.Intersection);
        }

        private void SetTileBody(TileController script, TileType type)
        {
            // Make sure we can undo this action in the editor
            Undo.RegisterFullObjectHierarchyUndo(script.gameObject, "Change Tile Body");

            switch (type)
            {
                case TileType.CoinGain: script.SetCoinGain(); break;
                case TileType.CoinLoss: script.SetCoinLoss(); break;
                case TileType.Star: script.SetStar(); break;
                case TileType.Intersection: script.SetIntersection(); break;
            }

            // Makes the scene dirty so that changes are saved
            EditorUtility.SetDirty(script);
        }

        private enum TileType
        {
            CoinGain = 0,
            CoinLoss = 1,
            Star = 2,
            Intersection = 3,
        }
    }

}
