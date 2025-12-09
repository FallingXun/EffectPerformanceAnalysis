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

        private static MethodInfo m_GetStorageMemorySizeLong = null;
        private static object[] m_ObjectParams = new object[1];


        public static string GetPackageRootPath()
        {
            var path = Path.GetFullPath(PACKAGE_NAME);
            return path;
        }

        public static bool UsingSRP()
        {
            return GraphicsSettings.currentRenderPipeline != null;
        }


        public static string GetTextureSizeFormat(long value)
        {
            string result = "";
            if (value > 1024 * 1024 * 1024)
            {
                result = (value / (1024 * 1024 * 1024)).ToString("F2") + "GB";
            }
            else if (value > 1024 * 1024)
            {
                result = (value / (1024 * 1024)).ToString("F2") + "MB";
            }
            else if (value > 1024)
            {
                result = (value / (1024)).ToString("F2") + "KB";
            }
            else
            {
                result = value.ToString("F2") + "B";
            }
            return result;
        }

        public static long GetTextureMemory(Texture texture)
        {
            long memory = 0;
            if (m_GetStorageMemorySizeLong == null)
            {
                Type type = typeof(TextureImporter).Assembly.GetType("UnityEditor.TextureUtil");
                m_GetStorageMemorySizeLong = type.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Public);
                //m_GetRuntimeMemorySizeLong = type.GetMethod("GetRuntimeMemorySizeLong", BindingFlags.Static | BindingFlags.Public);
            }
            if (m_GetStorageMemorySizeLong != null)
            {
                m_ObjectParams[0] = texture;
                memory = (long)m_GetStorageMemorySizeLong.Invoke(null, m_ObjectParams);
            }
            return memory;
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
