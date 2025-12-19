using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public class NodeUnitData
    {
        private List<RenderNode> renderNodeList = new List<RenderNode>();

        public int renderQueue { get; private set; }

        public int sortingOrder { get; private set; }

        public int meshVertexCount { get; private set; }

        public int meshVertexAttributeCount { get; private set; }

        public int meshTriangleCount { get; private set; }

        public int renderVertexCount { get; private set; }

        public int renderTriangleCount { get; private set; }

        public int batchCount { get; private set; }

        public int passCount { get; private set; }

        public int textureCount { get; private set; }

        public int textureSize { get; private set; }

        public int textureMaxWidth { get; private set; }

        public long textureMemory { get; private set; }

        public void Init(Renderer renderer)
        {
            renderNodeList.Clear();
            if (renderer != null && renderer.sharedMaterials != null)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    var material = renderer.sharedMaterials[i];
                    if (material == null)
                    {
                        continue;
                    }
                    var node = new RenderNode(material, renderer, i);
                    renderNodeList.Add(node);
                }
                renderNodeList.Sort((a, b) =>
                {
                    return ComparerUtils.Compare(a, b);
                });
            }

            var materialList = Pools.Get<List<Material>>();
            MaterialUtils.GetAllMaterial(materialList, renderer);
            var textureList = Pools.Get<List<Texture>>();
            foreach (var material in materialList)
            {
                TextureUtils.GetAllTextures(textureList, material);
            }

            sortingOrder = RendererUtils.GetSortingOrder(renderNodeList[0].renderer);
            renderQueue = MaterialUtils.GetRenderQueue(renderNodeList[0].material);
            meshVertexCount = MeshUtils.GetMeshVertexCount(renderNodeList[0].mesh);
            meshVertexAttributeCount = MeshUtils.GetMeshVertexAttributeCombination(renderNodeList[0].mesh);
            meshTriangleCount = MeshUtils.GetMeshTriangleCount(renderNodeList[0].mesh);
            renderVertexCount = MeshUtils.GetRenderTraingleCount(renderNodeList[0].mesh, MaterialUtils.GetPassCount(renderNodeList[0].material), renderNodeList[0].renderer);
            for (int i = 1; i < renderNodeList.Count; i++)
            {
                if (BatchUtils.Batch(renderNodeList[i - 1], renderNodeList[i]))
                {

                }
            }
        }

        public int CompareTo(NodeUnitData target)
        {
            return ComparerUtils.Compare(this, target);
        }
    }
}