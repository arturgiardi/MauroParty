using UnityEngine;
using UnityEngine.Splines;




namespace Game.Controller.Board
{
    [RequireComponent(typeof(SplineContainer))]
    public class BoardController : MonoBehaviour
    {
        [field: SerializeField] public SplineContainer SplineContainer { get; private set; }

        void Start()
        {

        }

    }
}
