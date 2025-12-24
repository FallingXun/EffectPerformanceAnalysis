using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public static class MaterialUtils
    {

        public static void GetAllMaterial(List<Material> materialList,Renderer renderer)
        {
            if (renderer != null && renderer.sharedMaterials != null)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }
                    if (materialList.Contains(material))
                    {
                        continue;
                    }
                    materialList.Add(material);
                }
            }
        }


        public static int GetRenderQueue(Material material)
        {
            if (material != null)
            {
                return material.renderQueue;
            }
            return 0;
        }


        public static int GetPassCount(Material material)
        {
            if (material != null)
            {
                // todo: 阴影等 pass 需要剔除
                return material.passCount;
            }
            return 0;
        }

        public static ShaderKeywordsData GetShaderKeywords(Material material)
        {
            return new ShaderKeywordsData(material);
        }

        public static int GetInstanceId(Material material)
        {
            if (material != null)
            {
                return material.GetInstanceID();
            }
            return 0;
        }

        public static int GetMaterialIndex(Material material, Renderer renderer, int sharedMaterialIndex = 0)
        {
            if (material != null && renderer != null && renderer.sharedMaterials != null)
            {
                // todo：SRP 下合批不需要考虑材质球顺序？
                var index = 0;
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (renderer.sharedMaterials[i] == null)
                    {
                        continue;
                    }
                    if (renderer.sharedMaterials[i].renderQueue > material.renderQueue)
                    {
                        continue;
                    }
                    if (renderer.sharedMaterials[i] == material && i >= sharedMaterialIndex)
                    {
                        break;
                    }
                    index++;
                }
            }
            return 0;
        }

        public static bool Batch(Material source, Material target, EBatchType batchType)
        {
            switch (batchType)
            {
                case EBatchType.DynamicBatch:
                    {
                        return DynamicBatch(source, target);
                    }
                case EBatchType.SRPBatch:
                    {
                        return SRPBatch(source, target);
                    }
            }
            return false;
        }


        public static bool DynamicBatch(Material source, Material target)
        {
            if (source == null || target == null)
            {
                return false;
            }
            if (source != target)
            {
                return false;
            }
            if (GetPassCount(source) > 1)
            {
                return false;
            }
            return true;
        }

        public static bool SRPBatch(Material source, Material target)
        {
            if (source == null || target == null)
            {
                return false;
            }
            if (GetPassCount(source) > 1 || GetPassCount(target) > 1)
            {
                return false;
            }
            if (source.shader != target.shader)
            {
                return false;
            }
            var shaderKeywords1 = new ShaderKeywordsData(source);
            var shaderKeywords2 = new ShaderKeywordsData(target);
            if (shaderKeywords1 != shaderKeywords2)
            {
                return false;
            }

            return true;
        }
    }
}