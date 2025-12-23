using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public static class TextureUtils
    {
        private static MethodInfo m_GetStorageMemorySizeLong = null;
        private static object[] m_ObjectParams = new object[1];

        public static string GetTextureMemoryFormat(long value)
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

        public static string GetTextureSizeFormat(int size)
        {
            var sqrt = (int)Mathf.Sqrt(size);
            return string.Format("{0}x{1}", sqrt, sqrt);
        }

        public static int GetTextureSize(Texture texture)
        {
            var size = texture.width * texture.height;
            return size;
        }

        public static void GetAllTextures(List<Texture> textureList, Material material)
        {
            if (material != null)
            {
                var nameIds = material.GetTexturePropertyNameIDs();
                if (nameIds == null || nameIds.Length <= 0)
                {
                    return;
                }
                foreach (var nameId in nameIds)
                {
                    var texture = material.GetTexture(nameId);
                    if (texture == null)
                    {
                        continue;
                    }
                    if (textureList.Contains(texture))
                    {
                        continue;
                    }
                    textureList.Add(texture);
                }
            }
        }




        public static long GetAllTexturesMemory(Texture texture)
        {
            var memory = 0L;
            memory += GetTextureMemory(texture);
            return memory;
        }
    }
}
