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



        public static void GetTextures(HashSet<Texture> textureList, Material material)
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

        public static int GetTexturesCount(Material material)
        {
            var count = 0;
            var textureList = Pools.Get<HashSet<Texture>>();
            GetTextures(textureList, material);
            count = textureList.Count;
            Pools.Release(textureList);
            return count;
        }

        public static int GetTexturesSize(Material material)
        {
            var size = 0;
            var textureList = Pools.Get<HashSet<Texture>>();
            GetTextures(textureList, material);
            foreach (var texture in textureList)
            {
                size += texture.width * texture.height;
            }
            Pools.Release(textureList);
            return size;
        }

        public static int GetTexturesMaxWidth(Material material)
        {
            var maxWidth = 0;
            var textureList = Pools.Get<HashSet<Texture>>();
            GetTextures(textureList, material);
            foreach (var texture in textureList)
            {
                maxWidth = Mathf.Max(maxWidth, texture.width);
            }
            Pools.Release(textureList);
            return maxWidth;
        }

        public static int GetTexturesMaxHeight(Material material)
        {
            var maxHeight = 0;
            var textureList = Pools.Get<HashSet<Texture>>();
            GetTextures(textureList, material);
            foreach (var texture in textureList)
            {
                maxHeight = Mathf.Max(maxHeight, texture.width);
            }
            Pools.Release(textureList);
            return maxHeight;
        }

        public static long GetTexturesMemory(Material material)
        {
            var memory = 0L;
            var textureList = Pools.Get<HashSet<Texture>>();
            GetTextures(textureList, material);
            foreach (var texture in textureList)
            {
                memory += GetTextureMemory(texture);
            }
            Pools.Release(textureList);
            return memory;
        }
    }
}
