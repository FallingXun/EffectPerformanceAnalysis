using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Rendering;


namespace EffectPerformanceAnalysis
{
    public class Performance
    {
        public const int SORTING_ORDER_MIN = -32768;
        public const int SORTING_ORDER_MAX = 32767;
        public const int SORTING_ORDER_INVAILD = 32768;


        private static List<int> m_TempBatchList = new List<int>();


        private static List<BaseNode> m_NodeList = new List<BaseNode>();

        public static EffectData effectData { get; private set; }

        static Performance()
        {

        }


        public static PerformanceData Analyze(GameObject root, int sortingOrderStart)
        {
            if (root == null)
            {
                return new PerformanceData();
            }

            effectData = new EffectData(root);
            m_NodeList.Clear();
            foreach (var renderer in effectData.RendererList)
            {
                BaseNode node = null; 
                if (renderer is SkinnedMeshRenderer)
                {
                    node = new SkinnedMeshRendererNode();
                }
                else if (renderer is MeshRenderer)
                {
                    node = new MeshRendererNode();
                }
                else if (renderer is ParticleSystemRenderer)
                {
                    node = new ParticleSystemNode();
                }
                node.Init(renderer);
                m_NodeList.Add(node);
            }

            m_NodeList.Sort();

            int sortingOrderAdd = 0;
            m_TempBatchList.Clear();
            if (m_NodeList.Count > 0)
            {
                m_TempBatchList.Add(0);
            }
            for (int i = 0; i < m_NodeList.Count; i++)
            {
                m_NodeList[i] = m_NodeList[i].SetId(i + 1);
                bool isSameBatch = false;
                if (i > 0)
                {
                    if (m_NodeList[i].IsSameBatch(m_NodeList[i - 1]) == false)
                    {
                        sortingOrderAdd++;
                        m_TempBatchList.Add(i);
                    }
                    else
                    {
                        isSameBatch = true;
                    }
                }
                if (isSameBatch == false)
                {
                    for (int j = m_TempBatchList.Count - 2; j >= 0; j--)
                    {
                        if (m_NodeList[i].IsSameBatch(m_NodeList[m_TempBatchList[j]]) == true)
                        {
                            m_NodeList[i] = m_NodeList[i].SetRepeatId(m_NodeList[m_TempBatchList[j]].id);
                            break;
                        }
                    }
                }
                else
                {
                    m_NodeList[i] = m_NodeList[i].SetRepeatId(m_NodeList[i - 1].repeatId);
                }

                m_NodeList[i] = m_NodeList[i].SetCorrectSortingOrder(sortingOrderStart + sortingOrderAdd);
            }


            int totalBatchCount = 0;
            int totalMeshVertexCount = 0;
            int totalMeshTriangleCount = 0;
            int totalRenderVertexCount = 0;
            int totalRenderTriangleCount = 0;
            for (int i = 0; i < m_NodeList.Count; i++)
            {
                if (i <= 0 || m_NodeList[i].IsSameBatch(m_NodeList[i - 1]) == false)
                {
                    totalBatchCount += m_NodeList[i].batchCount;
                }
                totalMeshVertexCount += m_NodeList[i].meshVertexCount;
                totalMeshTriangleCount += m_NodeList[i].meshTriangleCount;
                totalRenderVertexCount += m_NodeList[i].renderVertexCount;
                totalRenderTriangleCount += m_NodeList[i].renderTriangleCount;
            }

            var data = new PerformanceData();
            data.sortingOrderStart = sortingOrderStart;
            data.materialMaxCount = materialMaxCount;
            data.particleMaxCount = particleMaxCount;
            data.totalBatchCount = totalBatchCount;
            data.totalMaterialCount = totalMaterialCount;
            data.totalMeshVertexCount = totalMeshVertexCount;
            data.totalMeshTriangleCount = totalMeshTriangleCount;
            data.totalRenderVertexCount = totalRenderVertexCount;
            data.totalRenderTriangleCount = totalRenderTriangleCount;
            data.totalTextureCount = totalTextureCount;
            data.totalTextureSize = totalTextureSize;
            data.totalTextureMemory = totalTextureMemory;
            data.nodeDataList = nodeList;

            return data;
        }

        private static void SortShader()
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
                if (a.renderQueue != b.renderQueue)
                {
                    return a.renderQueue.CompareTo(b.renderQueue);
                }
                if (a.name != b.name)
                {
                    return a.name.CompareTo(b.name);
                }
                return a.GetInstanceID().CompareTo(b.GetInstanceID());
            });
            for (int i = 0; i < m_TempShaderIdList.Count; i++)
            {
                m_ShaderIdDict[m_TempShaderIdList[i]] = i;
            }
        }

        private static void SortShaderKeyword()
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
                return a.CompareTo(b);
            });
            for (int i = 0; i < m_TempShaderKeywordIdList.Count; i++)
            {
                m_ShaderKeywordIdDict[m_TempShaderKeywordIdList[i]] = i;
            }
        }


        private static void SortShaderKeywordGroup()
        {
            foreach (var material in m_MaterialIdDict.Keys)
            {
                var group = Utils.GetShaderKeywordsGroup(material, m_ShaderKeywordIdDict);
                if (m_ShaderKeywordGroupIdDict.ContainsKey(group) == false)
                {
                    m_ShaderKeywordGroupIdDict[group] = m_ShaderKeywordGroupIdDict.Count;
                }
            }

            m_TempShaderKeywordGroupIdList.Clear();
            for (int i = 0; i < m_ShaderKeywordGroupIdDict.Count; i++)
            {
                m_TempShaderKeywordGroupIdList.Add(0);
            }
            foreach (var item in m_ShaderKeywordGroupIdDict)
            {
                m_TempShaderKeywordGroupIdList[item.Value] = item.Key;
            }
            m_TempShaderKeywordGroupIdList.Sort((a, b) =>
            {
                return a.CompareTo(b);
            });
            for (int i = 0; i < m_TempShaderKeywordGroupIdList.Count; i++)
            {
                m_ShaderKeywordGroupIdDict[m_TempShaderKeywordGroupIdList[i]] = i;
            }
        }

        private static void SortMaterial()
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
                var shader1 = m_ShaderIdDict[a.shader];
                var shader2 = m_ShaderIdDict[b.shader];
                if (shader1 != shader2)
                {
                    return shader1.CompareTo(shader2);
                }
                var group1 = Utils.GetShaderKeywordsGroup(a, m_ShaderKeywordIdDict);
                var group2 = Utils.GetShaderKeywordsGroup(b, m_ShaderKeywordIdDict);
                if (group1 != group2)
                {
                    return group1.CompareTo(group2);
                }
                if (a.renderQueue != b.renderQueue)
                {
                    return a.renderQueue.CompareTo(b.renderQueue);
                }
                return a.GetInstanceID().CompareTo(b.GetInstanceID());
            });
            for (int i = 0; i < m_TempMaterialIdList.Count; i++)
            {
                m_MaterialIdDict[m_TempMaterialIdList[i]] = i;
            }
        }

        private static void SortTexture()
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
                if (a.name != b.name)
                {
                    return a.name.CompareTo(b.name);
                }
                return a.GetInstanceID().CompareTo(b.GetInstanceID());
            });
            for (int i = 0; i < m_TempTextureIdList.Count; i++)
            {
                m_TextureIdDict[m_TempTextureIdList[i]] = i;
            }
        }

        private static void SortMesh()
        {
            m_TempMeshIdList.Clear();
            for (int i = 0; i < m_MeshIdDict.Count; i++)
            {
                m_TempMeshIdList.Add(null);
            }
            foreach (var item in m_MeshIdDict)
            {
                m_TempMeshIdList[item.Value] = item.Key;
            }
            m_TempMeshIdList.Sort((a, b) =>
            {
                ulong id1 = 0;
                ulong id2 = 0;
                foreach (VertexAttribute item in Enum.GetValues(typeof(VertexAttribute)))
                {
                    id1 = (id1 << 1) + (ulong)(a.HasVertexAttribute(item) ? 1 : 0);
                    id2 = (id2 << 1) + (ulong)(b.HasVertexAttribute(item) ? 1 : 0);
                }
                if (id1 != id2)
                {
                    return id1.CompareTo(id2);
                }
                if (a.vertexCount != b.vertexCount)
                {
                    return a.vertexCount.CompareTo(b.vertexCount);
                }
                if (a.triangles.Length != b.triangles.Length)
                {
                    return a.triangles.Length.CompareTo(b.triangles.Length);
                }
                return a.GetInstanceID().CompareTo(b.GetInstanceID());
            });
            for (int i = 0; i < m_TempMeshIdList.Count; i++)
            {
                m_MeshIdDict[m_TempMeshIdList[i]] = i;
            }
        }

    }
}
