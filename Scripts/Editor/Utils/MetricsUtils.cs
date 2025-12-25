using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EffectPerformanceAnalysis
{
    public static class MetricsUtils
    {
        public static bool IsQualified(IMetrics metrics, IMetrics metricsLimit)
        {
            foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
            {
                if (IsQualified(metricsType, metrics, metricsLimit) == false)
                {
                    return false;
                }
            }
            return true;
        }


        public static bool IsQualified(EMetrics metricsType, IMetrics metrics, IMetrics metricsLimit)
        {
            if(metrics == null || metricsLimit == null)
            {
                return true;
            }
            switch (metricsType)
            {
                case EMetrics.OrderInLayer:
                    {
                        return metrics.sortingOrder <= metricsLimit.sortingOrder;
                    }
                case EMetrics.RenderQueue:
                    {
                        return metrics.renderQueue <= metricsLimit.renderQueue;
                    }
                case EMetrics.MeshVertexCount:
                    {
                        return metrics.meshVertexCount <= metricsLimit.meshVertexCount;
                    }
                case EMetrics.MeshVertexAttributes:
                    {
                        return metrics.meshVertexAttributeCount <= metricsLimit.meshVertexAttributeCount;
                    }
                case EMetrics.MeshTriangleCount:
                    {
                        return metrics.meshTriangleCount <= metricsLimit.meshTriangleCount;
                    }
                case EMetrics.RenderVertexCount:
                    {
                        return metrics.renderVertexCount <= metricsLimit.renderVertexCount;
                    }
                case EMetrics.RenderTriangleCount:
                    {
                        return metrics.renderTriangleCount <= metricsLimit.renderTriangleCount;
                    }
                case EMetrics.MaterialCount:
                    {
                        return metrics.materialCount <= metricsLimit.materialCount;
                    }
                case EMetrics.PassCount:
                    {
                        return metrics.passCount <= metricsLimit.passCount;
                    }
                case EMetrics.TextureCount:
                    {
                        return metrics.textureCount <= metricsLimit.textureCount;
                    }
                case EMetrics.TextureSize:
                    {
                        return metrics.textureSize <= metricsLimit.textureSize;
                    }
                case EMetrics.TextureMaxWidth:
                    {
                        return metrics.textureMaxWidth <= metricsLimit.textureMaxWidth;
                    }
                case EMetrics.TextureMaxHeight:
                    {
                        return metrics.textureMaxHeight <= metricsLimit.textureMaxHeight;
                    }
                case EMetrics.TextureMemory:
                    {
                        return metrics.textureMemory <= metricsLimit.textureMemory;
                    }
                case EMetrics.ParticleMaxCount:
                    {
                        return metrics.particleMaxCount <= metricsLimit.particleMaxCount;
                    }
                default:
                    {
                        Debug.LogErrorFormat("未实现 {0} 类型的上限检测！", metricsType.ToString());
                    }
                    break;
            }
            return false;
        }

        public static string GetMetricsValueFormat(EMetrics metricsType, IMetrics metrics)
        {
            string value = "";
            switch (metricsType)
            {
                case EMetrics.OrderInLayer:
                    {
                        if (metrics is RenderNode renderNode)
                        {
                            value = string.Format("{0}[{1}]", renderNode.sortingOrder, renderNode.sortingOrderRecommend);
                        }
                        else
                        {
                            value = metrics.sortingOrder.ToString();
                        }
                    }
                    break;
                case EMetrics.RenderQueue:
                    {
                        value = metrics.renderQueue.ToString();
                    }
                    break;
                case EMetrics.MeshVertexCount:
                    {
                        value = metrics.meshVertexCount.ToString();
                    }
                    break;
                case EMetrics.MeshVertexAttributes:
                    {
                        value = metrics.meshVertexAttributeCount.ToString();
                    }
                    break;
                case EMetrics.MeshTriangleCount:
                    {
                        value = metrics.meshTriangleCount.ToString();
                    }
                    break;
                case EMetrics.RenderVertexCount:
                    {
                        value = metrics.renderVertexCount.ToString();
                    }
                    break;
                case EMetrics.RenderTriangleCount:
                    {
                        value = metrics.renderTriangleCount.ToString();
                    }
                    break;
                case EMetrics.MaterialCount:
                    {
                        value = metrics.materialCount.ToString();
                    }
                    break;
                case EMetrics.PassCount:
                    {
                        value = metrics.passCount.ToString();
                    }
                    break;
                case EMetrics.TextureCount:
                    {
                        value = metrics.textureCount.ToString();
                    }
                    break;
                case EMetrics.TextureSize:
                    {
                        value = TextureUtils.GetTextureSizeFormat(metrics.textureSize);
                    }
                    break;
                case EMetrics.TextureMaxWidth:
                    {
                        value = metrics.textureMaxWidth.ToString();
                    }
                    break;
                case EMetrics.TextureMaxHeight:
                    {
                        value = metrics.textureMaxHeight.ToString();
                    }
                    break;
                case EMetrics.TextureMemory:
                    {
                        value = TextureUtils.GetTextureMemoryFormat(metrics.textureMemory);
                    }
                    break;
                case EMetrics.ParticleMaxCount:
                    {
                        value = metrics.particleMaxCount.ToString();
                    }
                    break;
                default:
                    {
                        Debug.LogErrorFormat("未实现 {0} 类型的格式输出！", metricsType.ToString());
                    }
                    break;
            }
            return value;
        }
    }

}