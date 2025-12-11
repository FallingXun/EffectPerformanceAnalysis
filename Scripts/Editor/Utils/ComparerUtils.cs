using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectPerformanceAnalysis
{
    public static class ComparerUtils
    {
        public static int Compare(RenderUnitData source, RenderUnitData target)
        {
            var result = 0;

            var rendererQueue1 = MaterialUtils.GetRenderQueue(source.material);
            var rendererQueue2 = MaterialUtils.GetRenderQueue(target.material);
            result = rendererQueue1.CompareTo(rendererQueue2);
            if(result != 0)
            {
                return result;
            }

            var sortingOrder1 = RendererUtils.GetSortingOrder(source.renderer);
            var sortingOrder2 = RendererUtils.GetSortingOrder(target.renderer);
            result = sortingOrder1.CompareTo(sortingOrder2);
            if (result != 0)
            {
                return result;
            }


            var batchType1 = BatchUtils.GetBatchType(source.renderer);
            var batchType2 = BatchUtils.GetBatchType(target.renderer);
            result = batchType1.CompareTo(batchType2);
            if (result != 0)
            {
                return result;
            }

            var passCount1 = MaterialUtils.GetPassCount(source.material);
            var passCount2 = MaterialUtils.GetPassCount(target.material);
            result = passCount1.CompareTo(passCount2);
            if (result != 0)
            {
                return result;
            }

            var shaderRenderQueue1 = ShaderUtils.GetRenderQueue(source.shader);
            var shaderRenderQueue2 = ShaderUtils.GetRenderQueue(target.shader);
            result = shaderRenderQueue1.CompareTo(shaderRenderQueue2);
            if (result != 0)
            {
                return result;
            }

            var shaderPassCount1 = ShaderUtils.GetPassCount(source.shader);
            var shaderPassCount2 = ShaderUtils.GetPassCount(target.shader);
            result = shaderPassCount1.CompareTo(shaderPassCount2);
            if (result != 0)
            {
                return result;
            }

            var shaderInstanceId1 = ShaderUtils.GetInstanceId(source.shader);
            var shaderInstanceId2 = ShaderUtils.GetInstanceId(target.shader);
            result = shaderInstanceId1.CompareTo(shaderInstanceId2);
            if (result != 0)
            {
                return result;
            }

            var shaderKeywords1 = MaterialUtils.GetShaderKeywords(source.material);
            var shaderKeywords2 = MaterialUtils.GetShaderKeywords(target.material);
            result = shaderKeywords1.CompareTo(shaderKeywords2);
            if (result != 0)
            {
                return result;
            }

            var materialInstanceId1 = MaterialUtils.GetInstanceId(source.material);
            var materialInstanceId2 = MaterialUtils.GetShaderKeywords(target.material);
            result = materialInstanceId1.CompareTo(materialInstanceId2);
            if (result != 0)
            {
                return result;
            }

            var componentType1 = RendererUtils.GetComponentType(source.renderer);
            var componentType2 = RendererUtils.GetComponentType(target.renderer);
            result = componentType1.CompareTo(componentType2);
            if (result != 0)
            {
                return result;
            }

            var meshVertexCount1 = MeshUtils.GetMeshVertexCount(source.mesh);
            var meshVertexCount2 = MeshUtils.GetMeshVertexCount(target.mesh);
            result = meshVertexCount1.CompareTo(meshVertexCount2);
            if (result != 0)
            {
                return result;
            }

            var meshVertexAttributeCount1 = MeshUtils.GetMeshVertexAttributeCount(source.mesh);
            var meshVertexAttributeCount2 = MeshUtils.GetMeshVertexAttributeCount(target.mesh);
            result = meshVertexAttributeCount1.CompareTo(meshVertexAttributeCount2);
            if (result != 0)
            {
                return result;
            }

            var meshVertexAttributeCombination1 = MeshUtils.GetMeshVertexAttributeCombination(source.mesh);
            var meshVertexAttributeCombination2 = MeshUtils.GetMeshVertexAttributeCombination(target.mesh);
            result = meshVertexAttributeCombination1.CompareTo(meshVertexAttributeCombination2);
            if (result != 0)
            {
                return result;
            }

            var meshTriangleCount1 = MeshUtils.GetMeshTriangleCount(source.mesh);
            var meshTriangleCount2 = MeshUtils.GetMeshTriangleCount(target.mesh);
            result = meshTriangleCount1.CompareTo(meshTriangleCount2);
            if (result != 0)
            {
                return result;
            }

            var meshInstanceId1 = MeshUtils.GetInstanceId(source.mesh);
            var meshInstanceId2 = MeshUtils.GetInstanceId(target.mesh);
            result = meshInstanceId1.CompareTo(meshInstanceId2);
            if (result != 0)
            {
                return result;
            }

            var particleSystemRenderMode1 = ParticleUtils.GetRenderMode(source.renderer);
            var particleSystemRenderMode2 = ParticleUtils.GetRenderMode(target.renderer);
            result = particleSystemRenderMode1.CompareTo(particleSystemRenderMode2);
            if (result != 0)
            {
                return result;
            }


            var rendererIndex1 = source.rendererIndex;
            var rendererIndex2 = target.rendererIndex;
            result = rendererIndex1.CompareTo(rendererIndex2);
            if (result != 0)
            {
                return result;
            }

            return 0;
        }
       

        public static int Compare(NodeUnitData source, NodeUnitData target)
        {
            var result = 0;

            var renderUnit1 = source.renderUnitList != null && source.renderUnitList.Count > 0 ? source.renderUnitList[0]:new RenderUnitData();
            var renderUnit2 = target.renderUnitList != null && target.renderUnitList.Count > 0 ? target.renderUnitList[0]:new RenderUnitData();
            result = renderUnit1.CompareTo(renderUnit2);
            if(result!= 0)
            {
                return result;
            }

            return 0;
        }
    }
    }

}