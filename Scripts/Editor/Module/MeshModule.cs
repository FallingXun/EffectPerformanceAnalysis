using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;

namespace EffectPerformanceAnalysis
{
    public class MeshModule : IModule
    {
        private Dictionary<Mesh, int> m_MeshIdDict = new Dictionary<Mesh, int>();
        private List<Mesh> m_TempMeshIdList = new List<Mesh>();

        public EModule moduleType { get { return EModule.Mesh; } }


        public void Init(List<Renderer> rendererList)
        {
            m_MeshIdDict.Clear();
            m_TempMeshIdList.Clear();

            foreach (var renderer in rendererList)
            {
                if (renderer == null || renderer.enabled == false)
                {
                    continue;
                }
                if (renderer is SkinnedMeshRenderer)
                {
                    var smr = renderer as SkinnedMeshRenderer;
                    if (smr.sharedMesh != null)
                    {
                        if (m_MeshIdDict.ContainsKey(smr.sharedMesh) == false)
                        {
                            m_MeshIdDict[smr.sharedMesh] = m_MeshIdDict.Count;
                        }
                    }
                }
                else if (renderer is MeshRenderer)
                {
                    var mf = renderer.GetComponent<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null)
                    {
                        if (m_MeshIdDict.ContainsKey(mf.sharedMesh) == false)
                        {
                            m_MeshIdDict[mf.sharedMesh] = m_MeshIdDict.Count;
                        }
                    }
                }
                else if (renderer is ParticleSystemRenderer)
                {
                    var psr = renderer as ParticleSystemRenderer;
                    if (psr.renderMode == ParticleSystemRenderMode.Mesh && psr.mesh != null)
                    {
                        if (m_MeshIdDict.ContainsKey(psr.mesh) == false)
                        {
                            m_MeshIdDict[psr.mesh] = m_MeshIdDict.Count;
                        }
                    }
                }
            }

            Sort();
        }

        private void Sort()
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
                var result = 0;
                ulong id1 = 0;
                ulong id2 = 0;
                foreach (VertexAttribute item in Enum.GetValues(typeof(VertexAttribute)))
                {
                    id1 = (id1 << 1) + (ulong)(a.HasVertexAttribute(item) ? 1 : 0);
                    id2 = (id2 << 1) + (ulong)(b.HasVertexAttribute(item) ? 1 : 0);
                }
                result = id1.CompareTo(id2);
                if (result != 0)
                {
                    return result;
                }

                result = a.vertexCount.CompareTo(b.vertexCount);
                if (result != 0)
                {
                    return result;
                }

                result = a.triangles.Length.CompareTo(b.triangles.Length);
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
            for (int i = 0; i < m_TempMeshIdList.Count; i++)
            {
                m_MeshIdDict[m_TempMeshIdList[i]] = i;
            }
        }

        public int GetMeshId(Mesh mesh)
        {
            if (mesh == null || m_MeshIdDict == null || m_MeshIdDict.ContainsKey(mesh) == false)
            {
                return -1;
            }
            return m_MeshIdDict[mesh];
        }
    }
}


