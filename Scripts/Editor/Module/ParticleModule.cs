using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EffectPerformanceAnalysis
{
    public class ParticleModule : IModule
    {

        public int maxCount { get; private set; }

        public EModule moduleType { get { return EModule.Particle; } }

        public void Init(List<Renderer> rendererList)
        {
            foreach (var renderer in rendererList)
            {
                if (renderer == null || renderer.enabled == false)
                {
                    continue;
                }
                if (renderer is ParticleSystemRenderer)
                {
                    var psr = renderer as ParticleSystemRenderer;
                    var ps = renderer.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        maxCount = Math.Max(ps.main.maxParticles, maxCount);
                    }
                }
            }
        }
    }
}


