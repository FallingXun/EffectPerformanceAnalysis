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

        public static int GetMaxCount(Renderer renderer)
        {
            if (renderer is ParticleSystemRenderer psr)
            {
                var ps = renderer.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    return ps.main.maxParticles;
                }
            }
            return 0;
        }
    }

}