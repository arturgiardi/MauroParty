using UnityEngine.Splines;


namespace Controller.Board
{
    [System.Serializable]
    public struct KnotObjectPair
    {
        public SplineKnotIndex knotIndex;
        public SplineNodeData nodeObject;
    }

}
