using System.Collections;
using System.Collections.Generic;
using System;

namespace EffectPerformanceAnalysis
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class DisplayAttribute : Attribute
    {
        public string name { get; private set; }

        public DisplayAttribute(string displayName)
        {
            this.name = displayName;
        }
    }
}

