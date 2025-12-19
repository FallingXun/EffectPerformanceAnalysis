using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct MetricsData:IMetrics
    {
        private EffectData effectData;
        private List<NodeUnitData> nodeUnitList;
        private int sortingOrderStart;

        public int rendererQueue { get; private set; }


        public MetricsData(GameObject root, int sortingOrderStart)
        {
            this.effectData = new EffectData(root);
            int index = 0;
            this.nodeUnitList = new List<NodeUnitData>();
            foreach (var renderer in effectData.RendererList)
            {
                var nodeUnit = new NodeUnitData(renderer, index++);
                nodeUnitList.Add(nodeUnit);
            }
            nodeUnitList.Sort((a,b) => {
                return a.CompareTo(b);
            });
            this.sortingOrderStart = sortingOrderStart;

            rendererQueue = 
        }

        private void SetNodeMetrics()
        {
            PerformanceNodeData
        }

       
    }

}