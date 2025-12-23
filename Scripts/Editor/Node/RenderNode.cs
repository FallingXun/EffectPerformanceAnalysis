using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectPerformanceAnalysis
{
    public class RenderNode : IMetrics
    {
        private List<RenderUnitData> m_RenderUnitList = null;

        public int count
        {
            get
            {
                if (m_RenderUnitList != null)
                {
                    return m_RenderUnitList.Count;
                }
                return 0;
            }
        }

        public RenderUnitData this[int index]
        {
            get
            {
                if (index < count)
                {
                    return m_RenderUnitList[index];
                }
                return default;
            }
        }

        public int sortingOrderRecommend { get; set; }

        #region IMetrics

        public int renderQueue { get; private set; }

        public int sortingOrder { get; private set; }

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

        public void Init(Renderer renderer)
        {
            if (m_RenderUnitList == null)
            {
                m_RenderUnitList = new List<RenderUnitData>();
            }
            else
            {
                m_RenderUnitList.Clear();
            }
            if (renderer != null && renderer.sharedMaterials != null)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    var material = renderer.sharedMaterials[i];
                    if (material == null)
                    {
                        continue;
                    }
                    var node = new RenderUnitData(material, renderer, i);
                    m_RenderUnitList.Add(node);
                }
                m_RenderUnitList.Sort((a, b) =>
                {
                    return ComparerUtils.Compare(a, b);
                });
            }

            if (m_RenderUnitList.Count > 0)
            {
                var materialList = Pools.Get<List<Material>>();
                var textureList = Pools.Get<List<Texture>>();
                MaterialUtils.GetAllMaterial(materialList, renderer);
                foreach (var material in materialList)
                {
                    TextureUtils.GetAllTextures(textureList, material);
                }

                sortingOrder = RendererUtils.GetSortingOrder(m_RenderUnitList[0].renderer);
                renderQueue = MaterialUtils.GetRenderQueue(m_RenderUnitList[0].material);
                meshVertexCount = MeshUtils.GetMeshVertexCount(m_RenderUnitList[0].mesh);
                meshVertexAttributeCount = MeshUtils.GetMeshVertexAttributeCount(m_RenderUnitList[0].mesh);
                meshTriangleCount = MeshUtils.GetMeshTriangleCount(m_RenderUnitList[0].mesh);
                renderVertexCount = MeshUtils.GetRenderVertexCount(m_RenderUnitList[0].mesh, MaterialUtils.GetPassCount(m_RenderUnitList[0].material), m_RenderUnitList[0].renderer);
                renderTriangleCount = MeshUtils.GetRenderTraingleCount(m_RenderUnitList[0].mesh, MaterialUtils.GetPassCount(m_RenderUnitList[0].material), m_RenderUnitList[0].renderer);
                passCount = MaterialUtils.GetPassCount(m_RenderUnitList[0].material);
                for (int i = 1; i < m_RenderUnitList.Count; i++)
                {
                    renderVertexCount += MeshUtils.GetRenderVertexCount(m_RenderUnitList[i].mesh, MaterialUtils.GetPassCount(m_RenderUnitList[i].material), m_RenderUnitList[i].renderer);
                    renderTriangleCount += MeshUtils.GetRenderTraingleCount(m_RenderUnitList[i].mesh, MaterialUtils.GetPassCount(m_RenderUnitList[i].material), m_RenderUnitList[i].renderer);
                    if (BatchUtils.Batch(m_RenderUnitList[i - 1], m_RenderUnitList[i]) == false)
                    {
                        passCount += MaterialUtils.GetPassCount(m_RenderUnitList[i].material);
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

                particleMaxCount = ParticleUtils.GetMaxCount(m_RenderUnitList[0].renderer);

                Pools.Release(materialList);
                Pools.Release(textureList);
            }
        }

        public int CompareTo(RenderNode target)
        {
            return ComparerUtils.Compare(this, target);
        }
    }
}