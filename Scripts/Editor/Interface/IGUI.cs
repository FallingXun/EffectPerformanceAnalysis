using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public interface IGUI
    {
        void Init(GameObject go);

        void OnInspectorGUI();
    }

}
