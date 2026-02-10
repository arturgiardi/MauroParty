using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif 
using UnityEngine;
using UnityEngine.Splines;


namespace Game.Controller.CustomSpline
{
    [ExecuteInEditMode]
    public class SplineKnotObjectInstantiator : MonoBehaviour, ISerializationCallbackReceiver
    {
        [field: SerializeField] private SplineContainer SplineContainer { get; set; }
        [field: SerializeField] private SplineNodeData NodeDataPrefab { get; set; }

        [SerializeField, HideInInspector]
        private List<KnotObjectPair> serializedPairs = new List<KnotObjectPair>();

        // Dictionary used only for performance, not serialized directly
        private Dictionary<SplineKnotIndex, SplineNodeData> instantiatedObjects = new();
        private bool isUpdating = false;


        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // Sincronizes the dictionary to the serialized list before serialization
            serializedPairs.Clear();
            foreach (var nodeDictionary in instantiatedObjects)
            {
                if (nodeDictionary.Value != null)
                    serializedPairs.Add(new KnotObjectPair
                    {
                        knotIndex = nodeDictionary.Key,
                        nodeObject = nodeDictionary.Value
                    });
            }
#endif
        }

        public void OnAfterDeserialize() { }

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (SplineContainer == null)
                return;
            CreateInstantiatedObjects();

            Spline.Changed += OnSplineChanged;
            RefreshNodes();
        }

        private void CreateInstantiatedObjects()
        {
            //Create dictionary from serialized list
            instantiatedObjects.Clear();
            foreach (var pair in serializedPairs)
            {
                if (pair.nodeObject != null)
                    instantiatedObjects[pair.knotIndex] = pair.nodeObject;
            }
        }

        private void OnDisable() => Spline.Changed -= OnSplineChanged;

        private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification)
        {
            if (isUpdating || this == null)
                return;

            // KnotModified is called during a change of position/rotation/scale of a knot
            // Don't affect the structure of the spline
            if (modification == SplineModification.KnotModified)
            {
                SyncPositions();
            }
            else //Insert, remove or change in the number of knots
            {
                RefreshNodes();
            }
        }

        private void RefreshNodes()
        {
            if (SplineContainer == null || NodeDataPrefab == null)
                return;

            isUpdating = true;

            HashSet<SplineKnotIndex> currentKnots = new();

            //1. Identify valid Knots (respecting Links/Weld)
            for (int s = 0; s < SplineContainer.Splines.Count; s++)
            {
                for (int k = 0; k < SplineContainer.Splines[s].Count; k++)
                {
                    var index = new SplineKnotIndex(s, k);

                    // Responsible for ensuring that a knot that is shared between
                    // multiple splines returns the same index
                    var effectiveIndex = GetMasterKnotIndex(index);
                    currentKnots.Add(effectiveIndex);
                }
            }

            // 2. Remove orphaned objects (Knots that no longer exist)
            var keysToRemove =
                instantiatedObjects.Keys.Where(k => !currentKnots.Contains(k)).ToList();
            foreach (var key in keysToRemove)
            {
                if (instantiatedObjects[key] != null)
                    DestroyImmediate(instantiatedObjects[key].gameObject);
                instantiatedObjects.Remove(key);
            }

            // 3. Create new objects only for Knots that don't have one yet
            foreach (var index in currentKnots)
            {
                if (!instantiatedObjects.ContainsKey(index))
                {
                    SpawnObject(index);
                }
            }

            SyncPositions();
            UpdateAllNeighbors();

            isUpdating = false;
        }

        private SplineKnotIndex GetMasterKnotIndex(SplineKnotIndex index)
        {
            var links = SplineContainer.KnotLinkCollection.GetKnotLinks(index);
            if (links.Any())
            {
                return links.OrderBy(n => n.Spline).ThenBy(n => n.Knot).First();
            }
            return index;
        }

        private void SpawnObject(SplineKnotIndex index)
        {
            if (Application.isPlaying)
                return;

            Vector3 worldPos = SplineContainer.transform.TransformPoint(
                SplineContainer[index.Spline][index.Knot].Position);

            var node = (SplineNodeData)PrefabUtility.InstantiatePrefab(NodeDataPrefab, transform);
            node.transform.position = worldPos;
            node.transform.rotation = Quaternion.identity;
            node.gameObject.name = $"Node_S{index.Spline}_K{index.Knot}";

            node.KnotIndex = index;
            node.Container = SplineContainer;

            instantiatedObjects[index] = node;
        }

        private void SyncPositions()
        {
            foreach (var obj in instantiatedObjects)
            {
                if (obj.Value == null)
                    continue;

                // Verify if the index is still valid before accessing
                if (obj.Key.Spline < SplineContainer.Splines.Count &&
                    obj.Key.Knot < SplineContainer[obj.Key.Spline].Count)
                {
                    var knot = SplineContainer[obj.Key.Spline][obj.Key.Knot];
                    obj.Value.transform.position =
                        SplineContainer.transform.TransformPoint(knot.Position);
                }
            }
        }

        private void UpdateAllNeighbors()
        {
            foreach (var obj in instantiatedObjects)
            {
                if (obj.Value != null)
                {
                    var data = obj.Value.GetComponent<SplineNodeData>();
                    if (data != null)
                        data.UpdateNeighbors(instantiatedObjects);
                }
            }
        }
    }
#endif
}
