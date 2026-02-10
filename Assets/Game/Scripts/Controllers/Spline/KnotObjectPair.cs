using UnityEngine.Splines;


namespace Game.Controller.CustomSpline
{
    [System.Serializable]
    public struct KnotObjectPair
    {
        public SplineKnotIndex knotIndex;
        public SplineNodeData nodeObject;
    }

}
