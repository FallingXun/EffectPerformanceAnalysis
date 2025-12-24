using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectPerformanceAnalysis
{
    public static class BatchUtils 
    {
        public static EBatchType GetBatchType(Renderer renderer)
        {
            var componentType = RendererUtils.GetComponentType(renderer);
            switch (componentType)
            {
                case EComponentType.SkinnedMeshRenderer:
                    {
                        if (Utils.UsingSRP())
                        {
                            return EBatchType.SRPBatch;
                        }
                        return EBatchType.None;
                    }
                case EComponentType.MeshRenderer:
                    {
                        if (Utils.UsingSRP())
                        {
                            return EBatchType.SRPBatch;
                        }
                        return EBatchType.DynamicBatch;
                    }
                case EComponentType.ParticleSystemRenderer:
                    {
                        return EBatchType.DynamicBatch;
                    }
                default:
                    {
                        return EBatchType.None;
                    }
            }
        }


        public static bool Batch(RenderUnitData source, RenderUnitData target)
        {
            var batchType1 = GetBatchType(source.renderer);
            var batchType2 = GetBatchType(target.renderer);
            if (batchType1 != batchType2)
            {
                return false;
            }
            switch (batchType1)
            {
                case EBatchType.DynamicBatch:
                    {
                        if(RendererUtils.DynamicBatch(source.renderer,target.renderer) == false)
                        {
                            return false;
                        }   
                        if (MaterialUtils.DynamicBatch(source.material, target.material) == false)
                        {
                            return false;
                        }
                        if (MeshUtils.DynamicBatch(source.mesh, target.mesh) == false)
                        {
                            return false;
                        }
                        return true;
                    }
                case EBatchType.SRPBatch:
                    {
                        if (RendererUtils.SRPBatch(source.renderer, target.renderer) == false)
                        {
                            return false;
                        }
                        if (MaterialUtils.SRPBatch(source.material, target.material) == false)
                        {
                            return false;
                        }
                        if (MeshUtils.SRPBatch(source.mesh, target.mesh) == false)
                        {
                            return false;
                        }
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }

}