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
        private int m_SortingOrder = Const.SORTING_ORDER_MAX;

        [Header("Render Queue 上限")]
        [SerializeField]
        private int m_RenderQueue = 4000;

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

        [Header("纹理尺寸上限（宽 x 高）")]
        [SerializeField]
        private int m_TextureSize;

        [Header("纹理最大宽度上限")]
        [SerializeField]
        private int m_TextureMaxWidth;

        [Header("纹理最大高度上限")]
        [SerializeField]
        private int m_TextureMaxHeight;

        [Header("纹理内存上限（单位：B）")]
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

    }

}