using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine.Rendering;

namespace EffectPerformanceAnalysis
{
    public class Utils
    {

        private const string PACKAGE_NAME = "Packages/com.xun.effectperformanceanalysis";



        public static string GetPackageRootPath()
        {
            var path = Path.GetFullPath(PACKAGE_NAME);
            return path;
        }

        public static bool UsingSRP()
        {
            return GraphicsSettings.currentRenderPipeline != null;
        }


        public static ulong GetShaderKeywordsGroup(Material material, Dictionary<string, int> shaderKeywordIdDict)
        {
            ulong group = 0;
            if (material == null || material.shaderKeywords == null || material.shaderKeywords.Length <= 0 || shaderKeywordIdDict == null)
            {
                return group;
            }
            foreach (var keyword in material.shaderKeywords)
            {
                if (string.IsNullOrWhiteSpace(keyword) == false)
                {
                    shaderKeywordIdDict.TryGetValue(keyword, out int id);
                    group |= (ulong)1 << id;
                }
            }
            return group;
        }

    }
}
