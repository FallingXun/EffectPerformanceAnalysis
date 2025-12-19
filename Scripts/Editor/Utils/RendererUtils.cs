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
                rendererList.Add(renderer);
            }
        }

        public static int GetSortingOrder(Renderer renderer)
        {
            if (renderer != null)
            {
                return renderer.sortingOrder;
            }
            return Performance.SORTING_ORDER_INVAILD;
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