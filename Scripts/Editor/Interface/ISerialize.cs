using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public interface ISerialize
    {
        void Clear();

        void Serialize();

        void Desrialize();
    }

}
