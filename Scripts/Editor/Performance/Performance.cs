using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Rendering;


namespace EffectPerformanceAnalysis
{
    public class Performance
    {
        public const int SORTING_ORDER_MIN = -32768;
        public const int SORTING_ORDER_MAX = 32767;
        public const int SORTING_ORDER_INVAILD = 32768;


        private static List<NodeUnitData> m_NodeUnitList = new List<NodeUnitData>();


        static Performance()
        {

        }


        public static MetricsData Analyze(GameObject root, int sortingOrderStart)
        {
            if (root == null)
            {
                return new MetricsData();
            }




            int sortingOrderAdd = 0;
            m_TempBatchList.Clear();
            if (m_NodeUnitList.Count > 0)
            {
                m_TempBatchList.Add(0);
            }
            for (int i = 0; i < m_NodeUnitList.Count; i++)
            {
                m_NodeUnitList[i] = m_NodeUnitList[i].SetId(i + 1);
                bool isSameBatch = false;
                if (i > 0)
                {
                    if (m_NodeUnitList[i].IsSameBatch(m_NodeUnitList[i - 1]) == false)
                    {
                        sortingOrderAdd++;
                        m_TempBatchList.Add(i);
                    }
                    else
                    {
                        isSameBatch = true;
                    }
                }
                if (isSameBatch == false)
                {
                    for (int j = m_TempBatchList.Count - 2; j >= 0; j--)
                    {
                        if (m_NodeUnitList[i].IsSameBatch(m_NodeUnitList[m_TempBatchList[j]]) == true)
                        {
                            m_NodeUnitList[i] = m_NodeUnitList[i].SetRepeatId(m_NodeUnitList[m_TempBatchList[j]].id);
                            break;
                        }
                    }
                }
                else
                {
                    m_NodeUnitList[i] = m_NodeUnitList[i].SetRepeatId(m_NodeUnitList[i - 1].repeatId);
                }

                m_NodeUnitList[i] = m_NodeUnitList[i].SetCorrectSortingOrder(sortingOrderStart + sortingOrderAdd);
            }


            int totalBatchCount = 0;
            int totalMeshVertexCount = 0;
            int totalMeshTriangleCount = 0;
            int totalRenderVertexCount = 0;
            int totalRenderTriangleCount = 0;
            for (int i = 0; i < m_NodeUnitList.Count; i++)
            {
                if (i <= 0 || m_NodeUnitList[i].IsSameBatch(m_NodeUnitList[i - 1]) == false)
                {
                    totalBatchCount += m_NodeUnitList[i].batchCount;
                }
                totalMeshVertexCount += m_NodeUnitList[i].meshVertexCount;
                totalMeshTriangleCount += m_NodeUnitList[i].meshTriangleCount;
                totalRenderVertexCount += m_NodeUnitList[i].renderVertexCount;
                totalRenderTriangleCount += m_NodeUnitList[i].renderTriangleCount;
            }

            var data = new MetricsData();
            data.sortingOrderStart = sortingOrderStart;
            data.materialMaxCount = materialMaxCount;
            data.particleMaxCount = particleMaxCount;
            data.totalBatchCount = totalBatchCount;
            data.totalMaterialCount = totalMaterialCount;
            data.totalMeshVertexCount = totalMeshVertexCount;
            data.totalMeshTriangleCount = totalMeshTriangleCount;
            data.totalRenderVertexCount = totalRenderVertexCount;
            data.totalRenderTriangleCount = totalRenderTriangleCount;
            data.totalTextureCount = totalTextureCount;
            data.totalTextureSize = totalTextureSize;
            data.totalTextureMemory = totalTextureMemory;
            data.nodeDataList = nodeList;

            return data;
        }
        
    }
}
