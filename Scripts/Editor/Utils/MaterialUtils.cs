using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public static class MaterialUtils
    {
        public static void GetMaterials(ICollection<Material> materials, Renderer renderer)
        {
            if (renderer != null && renderer.sharedMaterials!=null)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if(material == null)
                    {
                        continue;
                    }
                    materials.Add(material);
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