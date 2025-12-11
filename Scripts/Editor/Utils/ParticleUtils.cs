using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public static class ParticleUtils
    {

        public static ParticleSystemRenderMode GetRenderMode(Renderer renderer)
        {
            if (renderer is ParticleSystemRenderer psr)
            {
                return psr.renderMode;
            }
            return ParticleSystemRenderMode.None;
        }
    }

}