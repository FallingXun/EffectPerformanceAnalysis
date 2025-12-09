using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EffectPerformanceAnalysis
{
    public class EffectData
    {
        private List<Renderer> m_RendererList = new List<Renderer>();
        public List<Renderer> RendererList
        {
            get
            {
                return m_RendererList;
            }
        }


        public MaterialModule materialModule { get; private set; }
        public TextureModule textureModule { get; private set; }
        public MeshModule meshModule { get; private set; }
        public ParticleModule particleModule { get; private set; }

        public EffectData(GameObject root)
        {
            m_RendererList.Clear();
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if(renderer == null || renderer.enabled == false)
                {
                    continue;
                }
                m_RendererList.Add(renderer);
            }

            materialModule = new MaterialModule();
            materialModule.Init(m_RendererList);

            textureModule = new TextureModule();
            textureModule.Init(m_RendererList);

            meshModule = new MeshModule();
            meshModule.Init(m_RendererList);

            particleModule = new ParticleModule();
            particleModule.Init(m_RendererList);

        }
    }
}
