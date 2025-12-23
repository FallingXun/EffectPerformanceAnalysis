using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public interface IMetrics
    {
        int sortingOrder { get; }

        int renderQueue { get; }

        int meshVertexCount { get; }

        int meshVertexAttributeCount { get; }

        int meshTriangleCount { get; }

        int renderVertexCount { get; }

        int renderTriangleCount { get; }

        int passCount { get; }

        int textureCount { get; }

        int textureSize { get; }

        int textureMaxWidth { get; }

        int textureMaxHeight { get; }

        long textureMemory { get; }

        int particleMaxCount { get; }
    }

}