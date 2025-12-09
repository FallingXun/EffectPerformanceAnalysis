using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EffectPerformanceAnalysis
{
    public class TextureModule : IModule
    {
        private Dictionary<Texture, int> m_TextureIdDict = new Dictionary<Texture, int>();
        private List<Texture> m_TempTextureIdList = new List<Texture>();

        public int totalCount { get; private set; }
        public int totalSize { get; private set; }
        public long totalMemory { get; private set; }
        public int maxWidth { get; private set; }
        public int maxHeight { get; private set; }

        public EModule moduleType { get { return EModule.Texture; } }

        public void Init(List<Renderer> rendererList)
        {
            m_TextureIdDict.Clear();

            totalCount = 0;
            totalSize = 0;
            totalMemory = 0;
            maxWidth = 0;
            maxHeight = 0;

            foreach (var renderer in rendererList)
            {
                if (renderer == null || renderer.enabled == false)
                {
                    continue;
                }
                if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length <= 0)
                {
                    continue;
                }
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }
                    var nameIds = material.GetTexturePropertyNameIDs();
                    if (nameIds == null)
                    {
                        continue;
                    }
                    foreach (var nameId in nameIds)
                    {
                        var texture = material.GetTexture(nameId);
                        if (texture == null)
                        {
                            continue;
                        }
                        if (m_TextureIdDict.ContainsKey(texture) == false)
                        {
                            m_TextureIdDict[texture] = m_TextureIdDict.Count;
                            totalCount++;
                            totalSize += texture.width * texture.height;
                            totalMemory += Utils.GetTextureMemory(texture);
                            maxWidth = Math.Max(maxWidth, texture.width);
                            maxHeight = Math.Max(maxHeight, texture.height);
                        }
                    }
                }
            }

            Sort();
        }

        private void Sort()
        {
            m_TempTextureIdList.Clear();

            for (int i = 0; i < m_TextureIdDict.Count; i++)
            {
                m_TempTextureIdList.Add(null);
            }
            foreach (var item in m_TextureIdDict)
            {
                m_TempTextureIdList[item.Value] = item.Key;
            }

            m_TempTextureIdList.Sort((a, b) =>
            {
                var result = a.name.CompareTo(b.name);
                if (result != 0)
                {
                    return result;
                }

                result = a.GetInstanceID().CompareTo(b.GetInstanceID());
                if (result != 0)
                {
                    return result;
                }

                return 0;
            });

            for (int i = 0; i < m_TempTextureIdList.Count; i++)
            {
                m_TextureIdDict[m_TempTextureIdList[i]] = i;
            }
        }

        public int GetTextureId(Texture texture)
        {
            if (texture == null || m_TextureIdDict == null || m_TextureIdDict.ContainsKey(texture) == false)
            {
                return -1;
            }
            return m_TextureIdDict[texture];
        }
    }
}


