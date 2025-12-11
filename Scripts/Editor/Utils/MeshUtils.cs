using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace EffectPerformanceAnalysis
{
    public static class MeshUtils
    {
        private static Mesh m_ParticleBillboardMesh = null;


        public static Mesh GetMesh(Renderer renderer)
        {
            if (renderer is SkinnedMeshRenderer smr)
            {
                return smr.sharedMesh;
            }
            if (renderer is MeshRenderer mr)
            {
                var mf = renderer.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    return mf.sharedMesh;
                }
            }
            if (renderer is ParticleSystemRenderer psr)
            {
                switch (psr.renderMode)
                {
                    case ParticleSystemRenderMode.Billboard:
                    case ParticleSystemRenderMode.HorizontalBillboard:
                    case ParticleSystemRenderMode.Stretch:
                    case ParticleSystemRenderMode.VerticalBillboard:
                        {
                            if (m_ParticleBillboardMesh == null)
                            {
                                // 粒子特效的 Billboard 模式网格自动生成，使用自定义网格替代
                                m_ParticleBillboardMesh = new Mesh();
                                m_ParticleBillboardMesh.vertices = new Vector3[]{
                                    Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero
                                };
                                m_ParticleBillboardMesh.triangles = new int[]{
                                    0,1,2,0,2,3
                                };
                            }
                            return m_ParticleBillboardMesh;
                        }
                }
            }
            return null;
        }

        public static int GetMeshVertexCount(Mesh mesh)
        {
            if (mesh != null)
            {
                return mesh.vertexCount;
            }
            return 0;
        }

        public static int GetMeshVertexAttributeCount(Mesh mesh)
        {
            if (mesh != null)
            {
                return mesh.vertexCount * mesh.vertexAttributeCount;
            }
            return 0;
        }

        public static int GetMeshTriangleCount(Mesh mesh)
        {
            if (mesh != null)
            {
                return mesh.triangles.Length / 3;
            }
            return 0;
        }

        public static int GetRenderVertexCount(Mesh mesh, int passCount, Renderer renderer)
        {
            if (mesh != null)
            {
                if (renderer is ParticleSystemRenderer)
                {
                    ParticleSystem ps = renderer.GetComponent<ParticleSystem>();
                    if (ps != null && mesh != null)
                    {
                        // 粒子特效的网格数量按最大数量计算
                        // todo: 计算 Trails 和多网格
                        return passCount * mesh.vertexCount * ps.main.maxParticles;
                    }
                }
                else
                {
                    return passCount * mesh.vertexCount;
                }
            }
            return 0;
        }

        public static int GetRenderTraingleCount(Mesh mesh, int passCount, Renderer renderer)
        {
            if (mesh != null)
            {
                if (renderer is ParticleSystemRenderer)
                {
                    ParticleSystem ps = renderer.GetComponent<ParticleSystem>();
                    if (ps != null && mesh != null)
                    {
                        // 粒子特效的网格数量按最大数量计算
                        // todo: 计算 Trails 和多网格
                        return passCount * GetMeshTriangleCount(mesh) * ps.main.maxParticles;
                    }
                }
                else
                {
                    return passCount * GetMeshTriangleCount(mesh);
                }
            }
            return 0;
        }

        public static int GetMeshVertexAttributeCombination(Mesh mesh)
        {
            if (mesh != null)
            {
                var combination = 0;
                foreach (VertexAttribute item in Enum.GetValues(typeof(VertexAttribute)))
                {
                    combination |= 1 << (int)item;
                }
                return combination;
            }
            return 0;
        }

        public static int GetInstanceId(Mesh mesh)
        {
            if (mesh != null)
            {
                return mesh.GetInstanceID();
            }
            return 0;
        }

        public static bool DynamicBatch(Mesh source, Mesh target)
        {
            if (source == null || target == null)
            {
                return false;
            }
            var vertexCount1 = GetMeshVertexCount(source);
            var vertexCount2 = GetMeshVertexCount(source);
            if (vertexCount1 > 300 || vertexCount2 > 300)
            {
                return false;
            }
            var vertexAttributeCount1 = GetMeshVertexAttributeCount(source);
            var vertexAttributeCount2 = GetMeshVertexAttributeCount(target);
            if (vertexAttributeCount1 > 900 || vertexAttributeCount2 > 900)
            {
                return false;
            }
            return true;
        }

        public static bool SRPBatch(Mesh source, Mesh target)
        {
            if (source == null || target == null)
            {
                return false;
            }
            return true;
        }
    }
}