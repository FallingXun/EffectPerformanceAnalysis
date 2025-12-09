using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct RenderCommand
    {
        public int sortingOrder;
        public Material material;
        public int materialId;
        public long shaderKeywordGroupId;
        public int meshVertexCount;
        public int meshVertexAttributeCount;

        public int CompareTo(RenderCommand targetCommand)
        {
            var result = 0;

            if (materialId < 0)
            {
                return -1;
            }
            if (targetCommand.materialId < 0)
            {
                return 1;
            }

            result = material.renderQueue.CompareTo(targetCommand.material.renderQueue);
            if(result != 0)
            {
                return result;
            }

            result = sortingOrder.CompareTo(targetCommand.sortingOrder);
            if (result != 0)
            {
                return result;
            }

            result = materialId.CompareTo(targetCommand.materialId);

        }

        public bool DynamicBatching(RenderCommand targetCommand)
        {
            if (materialId < 0 || targetCommand.materialId < 0)
            {
                return false;
            }
            if (material != targetCommand.material)
            {
                return false;
            }
            if (material.passCount > 1)
            {
                return false;
            }
            if (meshVertexCount <= 0 || targetCommand.meshVertexCount <= 0)
            {
                return false;
            }
            if (meshVertexCount > 300 || targetCommand.meshVertexCount > 300)
            {
                return false;
            }
            if (meshVertexAttributeCount > 900 || targetCommand.meshVertexAttributeCount > 900)
            {
                return false;
            }
            return true;
        }

        public bool SRPBatch(RenderCommand targetCommand)
        {
            if (materialId < 0 || targetCommand.materialId < 0)
            {
                return false;
            }
            if (material.passCount > 1 || targetCommand.material.passCount > 1)
            {
                return false;
            }
            if (material.shader != targetCommand.material.shader)
            {
                return false;
            }
            if (shaderKeywordGroupId != targetCommand.shaderKeywordGroupId || shaderKeywordGroupId < 0 || targetCommand.shaderKeywordGroupId < 0)
            {
                return false;
            }
            return true;
        }
    }
}