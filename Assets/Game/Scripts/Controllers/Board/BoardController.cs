using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
#endif


namespace Controller.Board
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
