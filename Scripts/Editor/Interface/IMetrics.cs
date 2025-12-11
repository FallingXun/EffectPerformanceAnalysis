using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public interface IMetrics
    {
        int renderQueue { get; }

        int sortingOrder { get; }

        int meshVertexCount { get; }

        int meshVertexAttributeCount { get; }

        int meshTriangleCount { get; }

        int renderVertexCount { get; }

        int renderTriangleCount { get; }

        int batchCount { get; }

        int passCount { get; }

        int textureCount { get; }

        int textureSize { get; }

        int textureMaxWidth { get; }

        long textureMemory { get; }
    }

}