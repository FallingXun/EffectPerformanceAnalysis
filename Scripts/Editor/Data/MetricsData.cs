using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct MetricsData
    {
        public string value;
        public string name;

        public MetricsData(string value, string name)
        {
            this.value = value;
            this.name = name;
        }
    }

}