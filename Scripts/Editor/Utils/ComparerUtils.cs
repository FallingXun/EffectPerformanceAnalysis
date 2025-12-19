using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EffectPerformanceAnalysis
{
    public static class ComparerUtils
    {

        #region RenderNode Comparer

        private static Dictionary<ECompareType, Func<RenderNode, RenderNode, int>> m_RenderNodeComparerDict = new Dictionary<ECompareType, Func<RenderNode, RenderNode, int>>()
        {
            {ECompareType.SortingOrder, SortingOrderComparer},
            {ECompareType.RenderQueue, RenderQueueComparer},
            {ECompareType.ZDepth, ZDepthComparer},
            {ECompareType.MaterialIndex, MaterialIndexComparer},
            {ECompareType.BatchType, BatchTypeComparer},
            {ECompareType.PassCount, PassCountComparer},
            {ECompareType.ShaderRenderQueue, ShaderRenderQueueComparer},
            {ECompareType.ShaderPassCount, ShaderPassCountComparer},
            {ECompareType.ShaderInstanceId, ShaderInstanceIdComparer},
            {ECompareType.ShaderKeywords, ShaderKeywordsComparer},
            {ECompareType.MaterialInstanceId, MaterialInstanceIdComparer},
            {ECompareType.ComponentType, ComponentTypeComparer},
            {ECompareType.MeshVertexCount, MeshVertexCountComparer},
            {ECompareType.MeshVertexAttributeCount, MeshVertexAttributeCountComparer},
            {ECompareType.MeshVertexAttributeCombination, MeshVertexAttributeCombinationComparer},
            {ECompareType.MeshTriangleCount, MeshTriangleCountComparer},
            {ECompareType.MeshInstanceId, MeshInstanceIdComparer},
            {ECompareType.ParticleSystemRenderMode, ParticleSystemRenderModeComparer},
        };

        private static int SortingOrderComparer(RenderNode source, RenderNode target)
        {
            var sortingOrder1 = RendererUtils.GetSortingOrder(source.renderer);
            var sortingOrder2 = RendererUtils.GetSortingOrder(target.renderer);
            return sortingOrder1.CompareTo(sortingOrder2);
        }

        private static int RenderQueueComparer(RenderNode source, RenderNode target)
        {
            var renderQueue1 = MaterialUtils.GetRenderQueue(source.material);
            var renderQueue2 = MaterialUtils.GetRenderQueue(target.material);
            return renderQueue1.CompareTo(renderQueue2);
        }

        private static int ZDepthComparer(RenderNode source, RenderNode target)
        {
            var zDepth1 = RendererUtils.GetZDepth(source.renderer);
            var zDepth2 = RendererUtils.GetZDepth(target.renderer);
            return zDepth1.CompareTo(zDepth2);
        }

        private static int MaterialIndexComparer(RenderNode source, RenderNode target)
        {
            var materialIndex1 = MaterialUtils.GetMaterialIndex(source.material, source.renderer, source.sharedMaterialIndex);
            var materialIndex2 = MaterialUtils.GetMaterialIndex(target.material, target.renderer, target.sharedMaterialIndex);
            return materialIndex1.CompareTo(materialIndex2);
        }

        private static int BatchTypeComparer(RenderNode source, RenderNode target)
        {

            var batchType1 = BatchUtils.GetBatchType(source.renderer);
            var batchType2 = BatchUtils.GetBatchType(target.renderer);
            return batchType1.CompareTo(batchType2);
        }

        private static int PassCountComparer(RenderNode source, RenderNode target)
        {
            var passCount1 = MaterialUtils.GetPassCount(source.material);
            var passCount2 = MaterialUtils.GetPassCount(target.material);
            return passCount1.CompareTo(passCount2);
        }

        private static int ShaderRenderQueueComparer(RenderNode source, RenderNode target)
        {
            var shaderRenderQueue1 = ShaderUtils.GetRenderQueue(source.shader);
            var shaderRenderQueue2 = ShaderUtils.GetRenderQueue(target.shader);
            return shaderRenderQueue1.CompareTo(shaderRenderQueue2);
        }

        private static int ShaderPassCountComparer(RenderNode source, RenderNode target)
        {

            var shaderPassCount1 = ShaderUtils.GetPassCount(source.shader);
            var shaderPassCount2 = ShaderUtils.GetPassCount(target.shader);
            return shaderPassCount1.CompareTo(shaderPassCount2);
        }

        private static int ShaderInstanceIdComparer(RenderNode source, RenderNode target)
        {
            var shaderInstanceId1 = ShaderUtils.GetInstanceId(source.shader);
            var shaderInstanceId2 = ShaderUtils.GetInstanceId(target.shader);
            return shaderInstanceId1.CompareTo(shaderInstanceId2);
        }

        private static int ShaderKeywordsComparer(RenderNode source, RenderNode target)
        {
            var shaderKeywords1 = MaterialUtils.GetShaderKeywords(source.material);
            var shaderKeywords2 = MaterialUtils.GetShaderKeywords(target.material);
            return shaderKeywords1.CompareTo(shaderKeywords2);
        }

        private static int MaterialInstanceIdComparer(RenderNode source, RenderNode target)
        {
            var materialInstanceId1 = MaterialUtils.GetInstanceId(source.material);
            var materialInstanceId2 = MaterialUtils.GetShaderKeywords(target.material);
            return materialInstanceId1.CompareTo(materialInstanceId2);
        }

        private static int ComponentTypeComparer(RenderNode source, RenderNode target)
        {
            var componentType1 = RendererUtils.GetComponentType(source.renderer);
            var componentType2 = RendererUtils.GetComponentType(target.renderer);
            return componentType1.CompareTo(componentType2);
        }

        private static int MeshVertexCountComparer(RenderNode source, RenderNode target)
        {
            var meshVertexCount1 = MeshUtils.GetMeshVertexCount(source.mesh);
            var meshVertexCount2 = MeshUtils.GetMeshVertexCount(target.mesh);
            return meshVertexCount1.CompareTo(meshVertexCount2);
        }

        private static int MeshVertexAttributeCountComparer(RenderNode source, RenderNode target)
        {
            var meshVertexAttributeCount1 = MeshUtils.GetMeshVertexAttributeCount(source.mesh);
            var meshVertexAttributeCount2 = MeshUtils.GetMeshVertexAttributeCount(target.mesh);
            return meshVertexAttributeCount1.CompareTo(meshVertexAttributeCount2);
        }

        private static int MeshVertexAttributeCombinationComparer(RenderNode source, RenderNode target)
        {
            var meshVertexAttributeCombination1 = MeshUtils.GetMeshVertexAttributeCombination(source.mesh);
            var meshVertexAttributeCombination2 = MeshUtils.GetMeshVertexAttributeCombination(target.mesh);
            return meshVertexAttributeCombination1.CompareTo(meshVertexAttributeCombination2);
        }

        private static int MeshTriangleCountComparer(RenderNode source, RenderNode target)
        {
            var meshTriangleCount1 = MeshUtils.GetMeshTriangleCount(source.mesh);
            var meshTriangleCount2 = MeshUtils.GetMeshTriangleCount(target.mesh);
            return meshTriangleCount1.CompareTo(meshTriangleCount2);
        }

        private static int MeshInstanceIdComparer(RenderNode source, RenderNode target)
        {
            var meshInstanceId1 = MeshUtils.GetInstanceId(source.mesh);
            var meshInstanceId2 = MeshUtils.GetInstanceId(target.mesh);
            return meshInstanceId1.CompareTo(meshInstanceId2);
        }

        private static int ParticleSystemRenderModeComparer(RenderNode source, RenderNode target)
        {
            var particleSystemRenderMode1 = ParticleUtils.GetRenderMode(source.renderer);
            var particleSystemRenderMode2 = ParticleUtils.GetRenderMode(target.renderer);
            return particleSystemRenderMode1.CompareTo(particleSystemRenderMode2);
        }

        #endregion


        public static int Compare(RenderNode source, RenderNode target)
        {
            var result = 0;
            foreach (ECompareType compareType in Enum.GetValues(typeof(ECompareType)))
            {
                if (m_RenderNodeComparerDict.TryGetValue(compareType, out Func<RenderNode, RenderNode, int> comparer))
                {
                    if (comparer != null)
                    {
                        result = comparer(source, target);
                        if (result != 0)
                        {
                            return result;
                        }
                    }
                }
            }
            return 0;
        }


        public static int Compare(NodeUnitData source, NodeUnitData target)
        {
            var result = 0;

            var count1 = source.renderNodeList != null ? source.renderList.Count : 0;
            var count2 = target.renderNodeList != null ? target.renderList.Count : 0;
            var count = Mathf.Min(count1,count2);
            for (int i = 0; i < count; i++)
            {
                result = Compare(source.renderNodeList[i], target.renderNodeList[i]);
                if (result != 0)
                {
                    return result;
                }
            }

            result = count1.CompareTo(count2);
            if (result != 0)
            {
                return result;
            }

            return 0;
        }
    }
}

