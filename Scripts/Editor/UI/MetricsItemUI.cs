using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public class MetricsItemUI
    {
        public string title;
        public float width;
        public bool show;
        public EPerformanceMetrics performanceMetrics;

        public MetricsItemUI(EPerformanceMetrics performanceMetrics, string title, float width, bool show)
        {
            this.performanceMetrics = performanceMetrics;
            this.title = title;
            this.width = width;
            this.show = show;
        }
    }

}
