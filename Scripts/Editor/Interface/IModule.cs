using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public interface IModule
    {
        EModule moduleType { get; }

        void Init(List<Renderer> renderers);
    }

}
