using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct RenderUnit
    {
        public Material material;
        public Mesh mesh;
        public ENodeType nodeType;
        public Renderer renderer;

        public EBatchType batchType
        {
            get
            {
                switch (nodeType)
                {
                    case ENodeType.SkinnedMeshRenderer:
                        {
                            if (Utils.UsingSRP())
                            {
                                return EBatchType.SBPBatch;
                            }
                            return EBatchType.None;
                        }
                    case ENodeType.MeshRenderer:
                        {
                            if (Utils.UsingSRP())
                            {
                                return EBatchType.SBPBatch;
                            }
                            return EBatchType.DynamicBatch;
                        }
                    case ENodeType.ParticleSystem:
                        {
                            return EBatchType.DynamicBatch;
                        }
                    default:
                        {
                            return EBatchType.None;
                        }
                }


            }
        }

        public int CompareTo(RenderUnit target)
        {
            // 对比 RenderQueue
            if (material == null)
            {
                return -1;
            }
            if (target.material == null)
            {
                return 1;
            }
            var result = material.renderQueue.CompareTo(target.material.renderQueue);
            if (result != 0)
            {
                return result;
            }

            // 同 RenderQueue，对比 SortingOrder
            if (renderer == null)
            {
                return -1;
            }
            if (target.renderer == null)
            {
                return 1;
            }
            result = renderer.sortingOrder.CompareTo(target.renderer.sortingOrder);
            if (result != 0)
            {
                return result;
            }

            // 同 RenderQueue，同 SortingOrder，对比组件类型（同一种组件类型使用相同合批方式）
            if (Utils.UsingSRP())
            {
                // SBP 仅区分粒子和非粒子
                if (nodeType == ENodeType.ParticleSystem || target.nodeType == ENodeType.ParticleSystem)
                {
                    result = nodeType.CompareTo(target.nodeType);
                    if (result != 0)
                    {
                        return result;
                    }
                }
            }
            else
            {
                result = nodeType.CompareTo(target.nodeType);
                if (result != 0)
                {
                    return result;
                }
            }

            // 同 RenderQueue，同 SortingOrder，同组件类型，对比材质
            switch (batchType)
            {
                case EBatchType.SBPBatch:
                    {
                        var shader1 = Performance.effectData.materialModule.GetShaderId(material.shader);
                        var shader2 = Performance.effectData.materialModule.GetShaderId(target.material.shader);
                        if (shader1 < 0)
                        {
                            return -1;
                        }
                        if (shader2 < 0)
                        {
                            return 1;
                        }
                        result = shader1.CompareTo(shader2);
                        if (result != 0)
                        {
                            return result;
                        }

                        var group1 = Performance.effectData.materialModule.GetShaderKeywordsGroupId(material);
                        var group2 = Performance.effectData.materialModule.GetShaderKeywordsGroupId(target.material);
                        if (group1 < 0)
                        {
                            return -1;
                        }
                        if (group2 < 0)
                        {
                            return 1;
                        }
                        result = group1.CompareTo(group2);
                        if (result != 0)
                        {
                            return result;
                        }
                    }
                    break;
                case EBatchType.DynamicBatch:
                    {
                        var material1 = Performance.effectData.materialModule.GetMaterialId(material);
                        var material2 = Performance.effectData.materialModule.GetMaterialId(target.material);
                        if (material1 < 0)
                        {
                            return -1;
                        }
                        if (material2 < 0)
                        {
                            return 1;
                        }
                        result = material1.CompareTo(material2);
                        if (result != 0)
                        {
                            return result;
                        }
                    }
                    break;
                case EBatchType.None:
                    {
                        break;
                    }

            }

            // 同 RenderQueue，同 SortingOrder，同组件类型，同材质，对比网格
            var mesh1 = Performance.effectData.meshModule.GetMeshId(mesh);
            var mesh2 = Performance.effectData.meshModule.GetMeshId(target.mesh);
            if (nodeType == ENodeType.ParticleSystem)
            {
                var psr1 = renderer as ParticleSystemRenderer;
                var psr2 = target.renderer as ParticleSystemRenderer;
                if (psr1 == null)
                {
                    return -1;
                }
                if (psr2 == null)
                {
                    return 1;
                }
                // 粒子特效的Mesh模式和其他模式不能合批
                if (psr1.renderMode == ParticleSystemRenderMode.Mesh && psr2.renderMode == ParticleSystemRenderMode.Mesh)
                {
                    if (mesh1 < 0)
                    {
                        return -1;
                    }
                    if (mesh2 < 0)
                    {
                        return 1;
                    }
                    result = mesh1.CompareTo(mesh2);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                result = psr1.renderMode.CompareTo(psr2.renderMode);
                if (result != 0)
                {
                    return result;
                }
            }
            else
            {
                if (mesh1 < 0)
                {
                    return -1;
                }
                if (mesh2 < 0)
                {
                    return 1;
                }
                result = mesh1.CompareTo(mesh2);
                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }

        public bool CanBatch(RenderUnit target)
        {
            if (material == null || target.material == null)
            {
                return false;
            }

            // 对比组件类型
            if (Utils.UsingSRP())
            {
                if (nodeType == ENodeType.ParticleSystem || target.nodeType == ENodeType.ParticleSystem)
                {
                    if (nodeType != target.nodeType)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (nodeType != target.nodeType)
                {
                    return false;
                }
            }

            // 同组件类型，对比材质
            switch (batchType)
            {
                case EBatchType.SBPBatch:
                    {
                        var shader1 = Performance.effectData.materialModule.GetShaderId(material.shader);
                        var shader2 = Performance.effectData.materialModule.GetShaderId(target.material.shader);
                        if (shader1 != shader2 || shader1 < 0 || shader2 < 0)
                        {
                            return false;
                        }

                        var group1 = Performance.effectData.materialModule.GetShaderKeywordsGroupId(material);
                        var group2 = Performance.effectData.materialModule.GetShaderKeywordsGroupId(target.material);
                        if (group1 != group2 || group1 < 0 || group2 < 0)
                        {
                            return false;
                        }
                    }
                    break;
                case EBatchType.DynamicBatch:
                    {
                        var material1 = Performance.effectData.materialModule.GetMaterialId(material);
                        var material2 = Performance.effectData.materialModule.GetMaterialId(target.material);
                        if (material1 != material2 || material1 < 0 || material2 < 0)
                        {
                            return false;
                        }
                    }
                    break;
                case EBatchType.None:
                    {
                        return false;
                    }

            }

            // 同组件类型，同材质，对比网格
            if (nodeType == ENodeType.ParticleSystem)
            {
                var psr1 = renderer as ParticleSystemRenderer;
                var psr2 = target.renderer as ParticleSystemRenderer;
                if (psr1 == null || psr2 == null)
                {
                    return false;
                }
                if (psr1.renderMode == ParticleSystemRenderMode.Mesh || psr2.renderMode == ParticleSystemRenderMode.Mesh)
                {
                    if (psr1.renderMode != psr2.renderMode)
                    {
                        return false;
                    }
                    if (mesh == null || target.mesh == null)
                    {
                        return false;
                    }
                    if (mesh.vertexCount > 300 || mesh.vertexCount * mesh.vertexAttributeCount > 900)
                    {
                        return false;
                    }
                    if (target.mesh.vertexCount > 300 || target.mesh.vertexCount * target.mesh.vertexAttributeCount > 900)
                    {
                        return false;
                    }
                }
            }
            else
            {
                switch (batchType)
                {
                    case EBatchType.DynamicBatch:
                        {
                            if (mesh == null || target.mesh == null)
                            {
                                return false;
                            }
                            if (mesh.vertexCount > 300 || mesh.vertexCount * mesh.vertexAttributeCount > 900)
                            {
                                return false;
                            }
                            if (target.mesh.vertexCount > 300 || target.mesh.vertexCount * target.mesh.vertexAttributeCount > 900)
                            {
                                return false;
                            }
                        }
                        break;
                    case EBatchType.None:
                        {
                            return false;
                        }
                }
            }

            return true;
        }
    }
}