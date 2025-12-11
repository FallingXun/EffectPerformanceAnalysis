using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public static class RendererUtils
    {

        public static int GetSortingOrder(Renderer renderer)
        {
            if (renderer != null)
            {
                return renderer.sortingOrder;
            }
            return Performance.SORTING_ORDER_INVAILD;
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