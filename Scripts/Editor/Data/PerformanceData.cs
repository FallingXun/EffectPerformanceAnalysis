using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct PerformanceData
    {
        public int sortingOrderStart;
        public int materialMaxCount;
        public int particleMaxCount;
        public int totalBatchCount;
        public int totalMaterialCount;
        public int totalMeshVertexCount;
        public int totalMeshTriangleCount;
        public int totalRenderVertexCount;
        public int totalRenderTriangleCount;
        public int totalTextureCount;
        public int totalTextureSize;
        public int totalTextureMaxWidth;
        public int totalTextureMaxHeight;
        public long totalTextureMemory;
        public List<PerformanceNodeData> nodeDataList;
    }

}