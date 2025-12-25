using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EffectPerformanceAnalysis
{
    public class MetricsLimitConfigAsset : ScriptableObject, IMetrics
    {
        [Header("Order In Layer 上限")]
        [SerializeField]
        private int m_SortingOrder;

        [Header("Render Queue 上限")]
        [SerializeField]
        private int m_RenderQueue;

        [Header("网格顶点数量上限")]
        [SerializeField]
        private int m_MeshVertexCount;

        [Header("网格顶点属性数量上限")]
        [SerializeField]
        private int m_MeshVertexAttributeCount;

        [Header("网格三角面数量上限")]
        [SerializeField]
        private int m_MeshTriangleCount;

        [Header("渲染顶点数量上限")]
        [SerializeField]
        private int m_RenderVertexCount;

        [Header("渲染三角面数量上限")]
        [SerializeField]
        private int m_RenderTriangleCount;

        [Header("材质球数量上限")]
        [SerializeField]
        private int m_MaterialCount;

        [Header("Pass 数量上限")]
        [SerializeField]
        private int m_PassCount;

        [Header("纹理数量上限")]
        [SerializeField]
        private int m_TextureCount;

        [Header("纹理尺寸（宽 x 高）上限")]
        [SerializeField]
        private int m_TextureSize;

        [Header("纹理最大宽度上限")]
        [SerializeField]
        private int m_TextureMaxWidth;

        [Header("纹理最大高度上限")]
        [SerializeField]
        private int m_TextureMaxHeight;

        [Header("纹理内存上限")]
        [SerializeField]
        private int m_TextureMemory;

        [Header("粒子最大数量上限")]
        [SerializeField]
        private int m_ParticleMaxCount;

        #region IMetrics

        public int sortingOrder => m_SortingOrder;

        public int renderQueue => m_RenderQueue;

        public int meshVertexCount => m_MeshVertexCount;

        public int meshVertexAttributeCount => m_MeshVertexAttributeCount;

        public int meshTriangleCount => m_MeshTriangleCount;

        public int renderVertexCount => m_RenderVertexCount;

        public int renderTriangleCount => m_RenderTriangleCount;

        public int materialCount => m_MaterialCount;

        public int passCount => m_PassCount;

        public int textureCount => m_TextureCount;

        public int textureSize => m_TextureSize;

        public int textureMaxWidth => m_TextureMaxWidth;

        public int textureMaxHeight => m_TextureMaxHeight;

        public long textureMemory => m_TextureMemory;

        public int particleMaxCount => m_ParticleMaxCount;

        #endregion

        public bool IsVaild(IMetrics data)
        {
            foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
            {
                if(IsVaild(data,metricsType) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsVaild(IMetrics data, EMetrics metricsType)
        {
            switch (metricsType)
            {
                case EMetrics.OrderInLayer:
                    {
                        return data.sortingOrder <= sortingOrder;
                    }
                case EMetrics.RenderQueue:
                    {
                        return data.renderQueue <= renderQueue;
                    }
                case EMetrics.MeshVertexCount:
                    {
                        return data.meshVertexCount <= meshVertexCount;
                    }
                case EMetrics.MeshVertexAttributes:
                    {
                        return data.meshVertexAttributeCount <= meshVertexAttributeCount;
                    }
                case EMetrics.MeshTriangleCount:
                    {
                        return data.meshTriangleCount <= meshTriangleCount;
                    }
                case EMetrics.RenderVertexCount:
                    {
                        return data.renderVertexCount <= renderVertexCount;
                    }
                case EMetrics.RenderTriangleCount:
                    {
                        return data.renderTriangleCount <= renderTriangleCount;
                    }
                case EMetrics.MaterialCount:
                    {
                        return data.materialCount <= materialCount;
                    }
                case EMetrics.PassCount:
                    {
                        return data.passCount <= passCount;
                    }
                case EMetrics.TextureCount:
                    {
                        return data.textureCount <= textureCount;
                    }
                case EMetrics.TextureSize:
                    {
                        return data.textureSize <= textureSize;
                    }
                case EMetrics.TextureMaxWidth:
                    {
                        return data.textureMaxWidth <= textureMaxWidth;
                    }
                case EMetrics.TextureMaxHeight:
                    {
                        return data.textureMaxHeight <= textureMaxHeight;
                    }
                case EMetrics.TextureMemory:
                    {
                        return data.textureMemory <= textureMemory;
                    }
                case EMetrics.ParticleMaxCount:
                    {
                        return data.particleMaxCount <= particleMaxCount;
                    }
                default:
                    {
                        Debug.LogErrorFormat("未实现 {0} 类型的上限检测！", metricsType.ToString());
                    }
                    break;
            }
            return false;
        }
    }

}