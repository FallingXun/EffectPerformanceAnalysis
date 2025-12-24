using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public class RootNode : IMetrics
    {
        private Transform m_Root = null;
        private List<RenderNode> m_RenderNodeList = null;

        public int sortingOrderStart { get; private set; }

        public int count
        {
            get
            {
                if (m_RenderNodeList != null)
                {
                    return m_RenderNodeList.Count;
                }
                return 0;
            }
        }

        public RenderNode this[int index]
        {
            get
            {
                if (index < count)
                {
                    return m_RenderNodeList[index];
                }
                return null;
            }
        }

        #region IMetrics
        public int sortingOrder { get; private set; }

        public int renderQueue { get; private set; }

        public int meshVertexCount { get; private set; }

        public int meshVertexAttributeCount { get; private set; }

        public int meshTriangleCount { get; private set; }

        public int renderVertexCount { get; private set; }

        public int renderTriangleCount { get; private set; }

        public int materialCount { get; private set; }

        public int passCount { get; private set; }

        public int textureCount { get; private set; }

        public int textureSize { get; private set; }

        public int textureMaxWidth { get; private set; }

        public int textureMaxHeight { get; private set; }

        public long textureMemory { get; private set; }

        public int particleMaxCount { get; private set; }
        #endregion

        public void Init(Transform root, int sortingOrderStart)
        {
            m_Root = root;
            if (m_RenderNodeList == null)
            {
                m_RenderNodeList = new List<RenderNode>();
            }
            else
            {
                foreach (var item in m_RenderNodeList)
                {
                    Pools.Release(item);
                }
                m_RenderNodeList.Clear();
            }

            var rendererList = Pools.Get<List<Renderer>>();
            var materialList = Pools.Get<List<Material>>();
            var textureList = Pools.Get<List<Texture>>();

            this.sortingOrderStart = sortingOrderStart;
            RendererUtils.GetAllRenderers(rendererList, root);
            foreach (var renderer in rendererList)
            {
                var renderNode = Pools.Get<RenderNode>();
                renderNode.Init(renderer);
                m_RenderNodeList.Add(renderNode);

                MaterialUtils.GetAllMaterial(materialList, renderer);
            }
            foreach (var material in materialList)
            {
                TextureUtils.GetAllTextures(textureList, material);
            }
            m_RenderNodeList.Sort((a, b) =>
            {
                return a.CompareTo(b);
            });

            sortingOrder = Const.SORTING_ORDER_INVAILD;
            renderQueue = 0;
            meshVertexCount = 0;
            meshVertexAttributeCount = 0;
            meshTriangleCount = 0;
            renderVertexCount = 0;
            renderTriangleCount = 0;
            passCount = 0;
            particleMaxCount = 0;
            for (int i = 0; i < count; i++)
            {
                meshVertexCount += m_RenderNodeList[i].meshVertexCount;
                meshVertexAttributeCount = Mathf.Max(meshVertexAttributeCount, m_RenderNodeList[i].meshVertexAttributeCount);
                meshTriangleCount += m_RenderNodeList[i].meshTriangleCount;
                renderVertexCount += m_RenderNodeList[i].renderVertexCount;
                renderTriangleCount += m_RenderNodeList[i].renderTriangleCount;
                particleMaxCount = Mathf.Max(particleMaxCount, m_RenderNodeList[i].particleMaxCount);
                if (i < 1)
                {
                    sortingOrder = m_RenderNodeList[i].sortingOrder;
                    renderQueue = m_RenderNodeList[i].renderQueue;
                    passCount = m_RenderNodeList[i].passCount;

                    m_RenderNodeList[i].sortingOrderRecommend = sortingOrderStart;
                }
                else
                {
                    passCount += m_RenderNodeList[i].passCount;
                    // 比较相同索引的材质是否能合批，如果能合批，则 passCount 减少 1，如果不能合批，则后续都按不能合批处理
                    // todo：暂时仅做简单的横向对比，后续可增加详细的检查
                    for (int j = 0; j < m_RenderNodeList[i].count; j++)
                    {
                        if (j >= m_RenderNodeList[i - 1].count)
                        {
                            break;
                        }
                        if (BatchUtils.Batch(m_RenderNodeList[i - 1][j], m_RenderNodeList[i][j]) == false)
                        {
                            break;
                        }
                        passCount--;
                    }

                    if (BatchUtils.Batch(m_RenderNodeList[i - 1][0], m_RenderNodeList[i][0]))
                    {
                        m_RenderNodeList[i].sortingOrderRecommend = m_RenderNodeList[i - 1].sortingOrderRecommend;
                    }
                    else
                    {
                        m_RenderNodeList[i].sortingOrderRecommend = m_RenderNodeList[i - 1].sortingOrderRecommend + 1;
                    }
                }

            }
            materialCount = materialList.Count;

            textureCount = textureList.Count;
            textureSize = 0;
            textureMaxWidth = 0;
            textureMaxHeight = 0;
            textureMemory = 0;
            foreach (var texture in textureList)
            {
                textureSize += TextureUtils.GetTextureSize(texture);
                textureMaxWidth = Mathf.Max(textureMaxWidth, texture.width);
                textureMaxHeight = Mathf.Max(textureMaxHeight, texture.height);
                textureMemory += TextureUtils.GetTextureMemory(texture);
            }


            Pools.Release(rendererList);
            Pools.Release(materialList);
            Pools.Release(textureList);
        }

        public void Update()
        {
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    m_RenderNodeList[i][0].renderer.sortingOrder = m_RenderNodeList[i].sortingOrderRecommend;
                }
                EditorUtility.SetDirty(m_Root.gameObject);
            }
        }

        public void Reset()
        {
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    m_RenderNodeList[i][0].renderer.sortingOrder = m_RenderNodeList[i].sortingOrderRecommend - sortingOrderStart;
                }
                EditorUtility.SetDirty(m_Root.gameObject);
            }
        }
    }
}