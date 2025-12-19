using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public class RootNode :IMetrics
    {
        private List<NodeUnitData> m_NodeUnitList = new List<NodeUnitData>();

        #region IMetrics

        public int renderQueue
        {
            get
            {
                if (m_NodeUnitList.Count > 0)
                {
                    return m_NodeUnitList[0].renderQueue;
                }
                return 0;
            }
        }


        public int sortingOrder
        {
            get
            {
                if (m_NodeUnitList.Count > 0)
                {
                    return m_NodeUnitList[0].sortingOrder;
                }
                return 0;
            }
        }

        public int meshVertexCount
        {
            get
            {
                
            }
        }

        public int meshVertexAttributeCount => throw new System.NotImplementedException();

        public int meshTriangleCount => throw new System.NotImplementedException();

        public int renderVertexCount => throw new System.NotImplementedException();

        public int renderTriangleCount => throw new System.NotImplementedException();

        public int batchCount => throw new System.NotImplementedException();

        public int passCount => throw new System.NotImplementedException();

        public int textureCount => throw new System.NotImplementedException();

        public int textureSize => throw new System.NotImplementedException();

        public int textureMaxWidth => throw new System.NotImplementedException();

        public long textureMemory => throw new System.NotImplementedException();

        #endregion

        public void Init(Transform root)
        {
            foreach (var item in m_NodeUnitList)
            {
                Pools.Release(item);
            }
            m_NodeUnitList.Clear();
            var list = Pools.Get<List<Renderer>>();
            RendererUtils.GetAllRenderers(list, root);
            foreach (var renderer in list)
            {
                var nodeUnit = Pools.Get<NodeUnitData>();
                nodeUnit.Init(renderer);
                m_NodeUnitList.Add(nodeUnit);
            }
            Pools.Release(list);
            m_NodeUnitList.Sort((a, b) =>
            {
                return a.CompareTo(b);
            });
        }


    }
}