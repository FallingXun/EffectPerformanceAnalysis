using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectPerformanceAnalysis
{
    public struct ShaderKeywordsData
    {
        public int count;
        public string[] shaderKeywords;

        public ShaderKeywordsData(Material material)
        {
            if (material != null)
            {
                shaderKeywords = material.shaderKeywords;
            }
            else
            {
                shaderKeywords = null;
            }
            count = shaderKeywords != null ? shaderKeywords.Length : 0;
        }
       
        public int CompareTo(ShaderKeywordsData target)
        {
            var result = count.CompareTo(target.count);
            if(result != 0)
            {
                return result;
            }

            var list1 = Pools.Get<List<string>>();
            var list2 = Pools.Get<List<string>>();
            list1.AddRange(shaderKeywords);
            list2.AddRange(target.shaderKeywords);
            list1.Sort();
            list2.Sort();
            for (int i = 0; i < count; i++)
            {
                result = list1[i].CompareTo(list2[i]);
                if (result != 0)
                {
                    break;
                }
            }
            Pools.Release(list1);
            Pools.Release(list2);
            if (result != 0)
            {
                return result;
            }
            return 0;
        }

        public static bool operator == (ShaderKeywordsData a, ShaderKeywordsData b)
        {
            return a.CompareTo(b) == 0;
        }

        public static bool operator !=(ShaderKeywordsData a, ShaderKeywordsData b)
        {
            return a.CompareTo(b) != 0;
        }

        public bool Equals(ShaderKeywordsData data)
        {
            return this == data;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ShaderKeywordsData data))
            {
                return false;
            }
            return this == data;
        }

        public override int GetHashCode()
        {
#if UNITY_2022_3_OR_NEWER
            return HashCode.Combine(count, shaderKeywords);
#else
            return shaderKeywords.GetHashCode();
#endif
        }
    }
}