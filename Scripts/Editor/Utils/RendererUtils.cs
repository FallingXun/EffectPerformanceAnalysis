using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public static class RendererUtils
    {
        public static void GetAllRenderers(List<Renderer> rendererList, Transform root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer == null || renderer.enabled == false)
                {
                    continue;
                }
                if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length < 0)
                {
                    continue;
                }
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null)
                    {
                        rendererList.Add(renderer);
                        break;
                    }
                }
            }
        }

        public static int GetSortingOrder(Renderer renderer)
        {
            if (renderer != null)
            {
                return renderer.sortingOrder;
            }
            return Const.SORTING_ORDER_INVAILD;
        }

        public static long GetZDepth(Renderer renderer)
        {
            if (renderer != null)
            {
                return (long)(renderer.transform.position.z * -1000000);
            }
            return 0;
        }


        public static EComponentType GetComponentType(Renderer renderer)
        {
            if (renderer is SkinnedMeshRenderer)
            {
                return EComponentType.SkinnedMeshRenderer;
            }
            if (renderer is MeshRenderer)
            {
                return EComponentType.MeshRenderer;
            }
            if (renderer is ParticleSystemRenderer)
            {
                return EComponentType.ParticleSystemRenderer;
            }
            return EComponentType.None;
        }

        public static bool DynamicBatch(Renderer source, Renderer target)
        {
            var componentType1 = GetComponentType(source);
            var componentType2 = GetComponentType(target);
            if (componentType1 == EComponentType.None || componentType2 == EComponentType.None)
            {
                return false;
            }
            if (componentType1 == EComponentType.MeshRenderer && componentType2 == EComponentType.MeshRenderer)
            {
                return true;
            }
            if (componentType1 == EComponentType.ParticleSystemRenderer && componentType2 == EComponentType.ParticleSystemRenderer)
            {
                var particleRenderMode1 = ParticleUtils.GetRenderMode(source);
                var particleRenderMode2 = ParticleUtils.GetRenderMode(target);
                if (particleRenderMode1 == ParticleSystemRenderMode.Mesh && particleRenderMode2 == ParticleSystemRenderMode.Mesh)
                {
                    return true;
                }
                if (particleRenderMode1 < ParticleSystemRenderMode.Mesh && particleRenderMode2 < ParticleSystemRenderMode.Mesh)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool SRPBatch(Renderer source, Renderer target)
        {
            var componentType1 = GetComponentType(source);
            var componentType2 = GetComponentType(target);
            if ((componentType1 == EComponentType.SkinnedMeshRenderer || componentType1 != EComponentType.MeshRenderer) && 
                (componentType2 == EComponentType.SkinnedMeshRenderer || componentType2 != EComponentType.MeshRenderer))
            {
                return true;
            }
            return false;
        }

        public static int GetInstanceId(Renderer renderer)
        {
            if (renderer != null)
            {
                return renderer.GetInstanceID();
            }
            return 0;
        }
    }

}