using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectPerformanceAnalysis
{
    public struct ZDepthData 
    {
        public long depth;

        public ZDepthData(Renderer renderer)
        {
            if(renderer == null)
            {
                depth = 0;
            }
            else
            {
                depth = (long)(renderer.transform.position.z * 1000000);
            }
        }

        public int CompareTo(ZDepthData target)
        {
            var result = depth.CompareTo(target.depth);
            if (result != 0)
            {
                return result;
            }

            return 0;
        }

        public static bool operator ==(ZDepthData a, ZDepthData b)
        {
            return a.CompareTo(b) == 0;
        }

        public static bool operator !=(ZDepthData a, ZDepthData b)
        {
            return a.CompareTo(b) != 0;
        }

        public bool Equals(ZDepthData data)
        {
            return this == data;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ZDepthData data))
            {
                return false;
            }
            return this == data;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(depth);
        }
    }

}