using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;


namespace Game.Controller.CustomSpline
{
    public class SplineNodeData : MonoBehaviour
    {
        [field: SerializeField, HideInInspector]
        public SplineKnotIndex KnotIndex { get; set; }
        [field: SerializeField, HideInInspector]
        public SplineContainer Container { get; set; }

        [field: SerializeField]
        private List<SplineNodeData> PreviousNodes { get; set; } = new();

        [field: SerializeField]
        private List<SplineNodeData> NextNodes { get; set; } = new();

        public void UpdateNeighbors(Dictionary<SplineKnotIndex, SplineNodeData> lookup)
        {
            NextNodes.Clear();
            PreviousNodes.Clear();

            if (Container == null)
                return;

            // 1. Pegamos todos os nós que estão "soldados" ou linkados a este
            var linkedKnots = Container.KnotLinkCollection.GetKnotLinks(KnotIndex);

            // Se não houver links, tratamos apenas o nó atual como uma lista de um item
            var allRelatedKnots = new List<SplineKnotIndex>();

            foreach (var link in linkedKnots)
                allRelatedKnots.Add(link);

            if (linkedKnots.Count == 0) // Nó sem links, adiciona ele mesmo
                allRelatedKnots.Add(KnotIndex);

            // 2. Para cada nó linkado (de cada spline diferente), buscamos os vizinhos
            foreach (var knot in allRelatedKnots)
            {
                var spline = Container.Splines[knot.Spline];
                int nodeCount = spline.Count;

                // --- Próximos Nós ---
                int nextIdx = knot.Knot + 1;
                if (nextIdx < nodeCount)
                    AddToLink(new SplineKnotIndex(knot.Spline, nextIdx), NextNodes, lookup);
                else if (spline.Closed)
                    AddToLink(new SplineKnotIndex(knot.Spline, 0), NextNodes, lookup);

                // --- Nós Anteriores ---
                int prevIdx = knot.Knot - 1;
                if (prevIdx >= 0)
                    AddToLink(new SplineKnotIndex(knot.Spline, prevIdx), PreviousNodes, lookup);
                else if (spline.Closed)
                    AddToLink(new SplineKnotIndex(knot.Spline, nodeCount - 1), PreviousNodes, lookup);
            }
        }

        internal List<SplineKnotIndex> GetNextIds() 
            => NextNodes.Select(n => n.KnotIndex).ToList();
             
        private void AddToLink(SplineKnotIndex targetIdx,
            List<SplineNodeData> list,
            Dictionary<SplineKnotIndex, SplineNodeData> lookup)
        {
            // Como usamos o "MasterKnot" no dicionário do Instantiator, 
            // precisamos garantir que estamos buscando a chave correta
            var masterIndex = GetMasterKnotIndex(targetIdx);

            if (lookup.TryGetValue(masterIndex, out SplineNodeData neighbor))
            {
                if (!list.Contains(neighbor)) // Evita duplicatas se a spline for muito pequena
                    list.Add(neighbor);
            }
        }

        // Função auxiliar para achar o ID único usado no dicionário
        private SplineKnotIndex GetMasterKnotIndex(SplineKnotIndex index)
        {
            var links = Container.KnotLinkCollection.GetKnotLinks(index);
            var list = new List<SplineKnotIndex>();

            foreach (var l in links)
                list.Add(l);

            if (list.Count > 0)
                return Enumerable.OrderBy(list, n => n.Spline).ThenBy(n => n.Knot).First();

            return index;
        }
    }
}
