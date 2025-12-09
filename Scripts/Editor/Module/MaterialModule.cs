using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EffectPerformanceAnalysis
{
    public class MaterialModule : IModule
    {
        private Dictionary<Material, int> m_MaterialIdDict = new Dictionary<Material, int>();
        private Dictionary<Shader, int> m_ShaderIdDict = new Dictionary<Shader, int>();
        private Dictionary<string, int> m_ShaderKeywordIdDict = new Dictionary<string, int>();

        private static List<Material> m_TempMaterialIdList = new List<Material>();
        private static List<Shader> m_TempShaderIdList = new List<Shader>();
        private static List<string> m_TempShaderKeywordIdList = new List<string>();

        public int maxCount { get; private set; }

        public int totalCount { get; private set; }

        public EModule moduleType { get { return EModule.Material; } }

        public void Init(List<Renderer> rendererList)
        {
            m_MaterialIdDict.Clear();
            m_ShaderIdDict.Clear();
            m_ShaderKeywordIdDict.Clear();

            m_TempMaterialIdList.Clear();
            m_TempShaderIdList.Clear();
            m_TempShaderKeywordIdList.Clear();

            maxCount = 0;
            totalCount = 0;

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
                var count = 0;
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }
                    count++;
                    if (m_MaterialIdDict.ContainsKey(material))
                    {
                        continue;
                    }

                    m_MaterialIdDict[material] = m_MaterialIdDict.Count;
                    totalCount++;

                    if (m_ShaderIdDict.ContainsKey(material.shader) == false)
                    {
                        m_ShaderIdDict[material.shader] = m_ShaderIdDict.Count;
                    }
                    if (material.shaderKeywords != null)
                    {
                        foreach (var keyword in material.shaderKeywords)
                        {
                            if (string.IsNullOrWhiteSpace(keyword) == false)
                            {
                                continue;
                            }
                            if (m_ShaderKeywordIdDict.ContainsKey(keyword) == false)
                            {
                                m_ShaderKeywordIdDict[keyword] = m_ShaderKeywordIdDict.Count;
                            }
                        }
                    }
                }
                maxCount = Math.Max(maxCount, count);
            }

            SortShader();
            SortShaderKeyword();
            SortMaterial();
        }


        private void SortShader()
        {
            m_TempShaderIdList.Clear();

            for (int i = 0; i < m_ShaderIdDict.Count; i++)
            {
                m_TempShaderIdList.Add(null);
            }
            foreach (var item in m_ShaderIdDict)
            {
                m_TempShaderIdList[item.Value] = item.Key;
            }
            m_TempShaderIdList.Sort((a, b) =>
            {
                var result = 0;
                result = a.renderQueue.CompareTo(b.renderQueue);
                if (result != 0)
                {
                    return result;
                }

                result = a.name.CompareTo(b.name);
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
            for (int i = 0; i < m_TempShaderIdList.Count; i++)
            {
                m_ShaderIdDict[m_TempShaderIdList[i]] = i;
            }
        }

        private void SortShaderKeyword()
        {
            m_TempShaderKeywordIdList.Clear();
            for (int i = 0; i < m_ShaderKeywordIdDict.Count; i++)
            {
                m_TempShaderKeywordIdList.Add(null);
            }
            foreach (var item in m_ShaderKeywordIdDict)
            {
                m_TempShaderKeywordIdList[item.Value] = item.Key;
            }
            m_TempShaderKeywordIdList.Sort((a, b) =>
            {
                var result = 0;
                result = a.CompareTo(b);
                if (result != 0)
                {
                    return result;
                }

                return 0;
            });
            for (int i = 0; i < m_TempShaderKeywordIdList.Count; i++)
            {
                m_ShaderKeywordIdDict[m_TempShaderKeywordIdList[i]] = i;
            }
        }


        private void SortMaterial()
        {
            m_TempMaterialIdList.Clear();
            for (int i = 0; i < m_MaterialIdDict.Count; i++)
            {
                m_TempMaterialIdList.Add(null);
            }
            foreach (var item in m_MaterialIdDict)
            {
                m_TempMaterialIdList[item.Value] = item.Key;
            }
            m_TempMaterialIdList.Sort((a, b) =>
            {
                var result = 0;

                result = a.renderQueue.CompareTo(b.renderQueue);
                if (result != 0)
                {
                    return result;
                }

                result = a.passCount.CompareTo(b.passCount);
                if (result != 0)
                {
                    return result;
                }

                var shader1 = GetShaderId(a.shader);
                var shader2 = GetShaderId(b.shader);
                result = shader1.CompareTo(shader2);
                if (result != 0)
                {
                    return result;
                }

                var group1 = GetShaderKeywordsGroupId(a);
                var group2 = GetShaderKeywordsGroupId(b);
                result = group1.CompareTo(group2);
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

            for (int i = 0; i < m_TempMaterialIdList.Count; i++)
            {
                m_MaterialIdDict[m_TempMaterialIdList[i]] = i;
            }
        }

        public int GetShaderId(Shader shader)
        {
            if (shader == null || m_ShaderIdDict == null || m_ShaderIdDict.ContainsKey(shader) == false)
            {
                return -1;
            }
            return m_ShaderIdDict[shader];
        }

        public long GetShaderKeywordsGroupId(Material material)
        {
            if (material == null || material.shaderKeywords == null || material.shaderKeywords.Length <= 0 || m_ShaderKeywordIdDict == null)
            {
                return -1;
            }
            long group = 0;
            foreach (var keyword in material.shaderKeywords)
            {
                if (string.IsNullOrWhiteSpace(keyword) == false)
                {
                    int id = -1;
                    m_ShaderKeywordIdDict.TryGetValue(keyword, out id);
                    if (id < 0 || id >= 62)
                    {
                        return -1;
                    }
                    group |= (long)1 << id;
                }
            }
            return group;
        }


        public int GetMaterialId(Material material)
        {
            if (material == null || m_MaterialIdDict == null || m_MaterialIdDict.ContainsKey(material) == false)
            {
                return int.MaxValue;
            }
            return m_MaterialIdDict[material];
        }




        public bool DynamicBatching(Material sMaterial, Material tMaterial, Mesh sMesh, Mesh tMesh)
        {
            if (sMaterial == null || tMaterial == null || sMesh == null || tMesh == null)
            {
                return false;
            }
            if (sMaterial != tMaterial)
            {
                return false;
            }
            if (sMaterial.passCount > 1)
            {
                return false;
            }
            if (sMesh.vertexCount > 300 || tMesh.vertexCount > 300)
            {
                return false;
            }
            if (sMesh.vertexCount * sMesh.vertexAttributeCount > 900 || sMesh.vertexCount * sMesh.vertexAttributeCount > 900)
            {
                return false;
            }
            return true;
        }

        public bool SRPBatch(Material sMaterial, Material tMaterial)
        {
            if (sMaterial == null || tMaterial == null)
            {
                return false;
            }
            if(sMaterial.passCount > 1 || tMaterial.passCount > 1)
            {
                return false;
            }
            if (sMaterial.shader != tMaterial.shader)
            {
                return false;
            }
            var sGroup = GetShaderKeywordsGroupId(sMaterial);
            var tGroup = GetShaderKeywordsGroupId(tMaterial);
            if (sGroup != tGroup || sGroup < 0 || tGroup < 0)
            {
                return false;
            }
            return true;
        }
    }
}


