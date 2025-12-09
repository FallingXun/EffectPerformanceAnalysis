using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public class ParticleSystemNode : BaseNode
    {

        public override ENodeType nodeType
        {
            get
            {
                return ENodeType.ParticleSystem;
            }
        }

        public override bool canBatch
        {
            get
            {
                if (mainMaterial == null)
                {
                    return false;
                }

                if (meshVertexCount > 300)
                {
                    return false;
                }

                if (meshVertexAttributeCount > 900)
                {
                    return false;
                }

                foreach (var material in m_MaterialList)
                {
                    if (material != null && material.passCount <= 1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        public override int meshVertexCount
        {
            get
            {
                ParticleSystemRenderer psr = renderer as ParticleSystemRenderer;
                ParticleSystem ps = renderer.GetComponent<ParticleSystem>();
                if (psr != null && ps != null)
                {
                    var count = 0;
                    switch (psr.renderMode)
                    {
                        case ParticleSystemRenderMode.Billboard:
                        case ParticleSystemRenderMode.HorizontalBillboard:
                        case ParticleSystemRenderMode.VerticalBillboard:
                        case ParticleSystemRenderMode.Stretch:
                            {
                                count = 4;
                            }
                            break;
                        case ParticleSystemRenderMode.Mesh:
                            {
                                if (psr.mesh != null)
                                {
                                    count = psr.mesh.vertexCount;
                                }
                            }
                            break;
                    }
                    count *= ps.main.maxParticles;
                    // todo: 添加 Trails 
                    return count;
                }
                return 0;
            }
        }

        public override int meshVertexAttributeCount
        {
            get
            {
                ParticleSystemRenderer psr = renderer as ParticleSystemRenderer;
                if (psr != null )
                {
                    var count = 0;
                    switch (psr.renderMode)
                    {
                        case ParticleSystemRenderMode.Billboard:
                        case ParticleSystemRenderMode.HorizontalBillboard:
                        case ParticleSystemRenderMode.VerticalBillboard:
                        case ParticleSystemRenderMode.Stretch:
                            {
                                // 位置 + 法线 + UV
                                count = 3 * meshVertexCount;
                            }
                            break;
                        case ParticleSystemRenderMode.Mesh:
                            {
                                if (psr.mesh != null)
                                {
                                    count = psr.mesh.vertexAttributeCount * meshVertexCount;
                                }
                            }
                            break;
                    }
                    return count;
                }
                return 0;
            }
        }


        public override int meshTriangleCount
        {
            get
            {
                ParticleSystemRenderer psr = renderer as ParticleSystemRenderer;
                ParticleSystem ps = renderer.GetComponent<ParticleSystem>();
                if (psr != null && ps != null)
                {
                    var count = 0;
                    switch (psr.renderMode)
                    {
                        case ParticleSystemRenderMode.Billboard:
                        case ParticleSystemRenderMode.HorizontalBillboard:
                        case ParticleSystemRenderMode.VerticalBillboard:
                        case ParticleSystemRenderMode.Stretch:
                            {
                                count = 2;
                            }
                            break;
                        case ParticleSystemRenderMode.Mesh:
                            {
                                if (psr.mesh != null)
                                {
                                    count = psr.mesh.triangles.Length / 3;
                                }
                            }
                            break;
                    }
                    count *= ps.main.maxParticles;;
                    // todo: 添加 Trails 
                    return count;
                }
                return 0;
            }
        }

        public override int renderVertexCount
        {
            get
            {
                if (mainMaterial != null)
                {
                    // todo：粒子特效的第二个材质为 Trails 效果，暂不计入
                    return meshVertexCount * m_MaterialList[0].passCount;
                }
                return 0;
            }
        }

        public override int renderTriangleCount
        {
            get
            {
                if (mainMaterial != null)
                {
                    // todo：粒子特效的第二个材质为 Trails 效果，暂不计入
                    return meshTriangleCount * m_MaterialList[0].passCount;
                }
                return 0;
            }
        }

        public override int totalPassCount
        {
            get
            {
                var count = 0;
                if (mainMaterial != null)
                {
                    count = mainMaterial.passCount;
                    for (int i = 1; i < m_MaterialList.Count; i++)
                    {
                        if (m_MaterialList[i] == null)
                        {
                            continue;
                        }
                        if (m_MaterialList[i].passCount <= 1 && m_MaterialList[i - 1] != null)
                        {
                            if (m_MaterialList[i] == m_MaterialList[i - 1])
                            {
                                continue;
                            }
                        }
                        count += m_MaterialList[i].passCount;
                    }
                }
                return count;
            }
        }


        public override int CompareTo(BaseNode target)
        {
            var result = base.CompareTo(target);
            if (result != 0)
            {
                return result;
            }

            var psr1 = renderer as ParticleSystemRenderer;
            var psr2 = target.renderer as ParticleSystemRenderer;
            if (psr1 == null)
            {
                return -1;
            }
            if (psr2 == null)
            {
                return 1;
            }
            if (psr1.renderMode == psr2.renderMode && psr1.renderMode == ParticleSystemRenderMode.Mesh)
            {
                if (mainMesh == null)
                {
                    return -1;
                }
                if (target.mainMesh == null)
                {
                    return 1;
                }
                MeshModule module = m_EffectData.GetModule<MeshModule>();
                var id1 = module.GetMeshId(mainMesh);
                var id2 = module.GetMeshId(target.mainMesh);
                return id1.CompareTo(id2);
            }
            else
            {
                return psr1.renderMode.CompareTo(psr2.renderMode);
            }
        }

        public override bool IsSameBatch(Renderer renderer, Renderer targetRenderer)
        {
            if (renderer == null || targetRenderer == null || renderer.sharedMaterial != targetRenderer.sharedMaterial)
            {
                return false;
            }
            if (renderer.sharedMaterial.passCount > 1 || targetRenderer.sharedMaterial.passCount > 1)
            {
                return false;
            }

            var psr1 = renderer as ParticleSystemRenderer;
            var psr2 = targetRenderer as ParticleSystemRenderer;
            if (psr1 == null || psr2 == null)
            {
                return false;
            }
            if (psr1.renderMode == ParticleSystemRenderMode.None || psr2.renderMode == ParticleSystemRenderMode.None)
            {
                if (psr1.renderMode != psr2.renderMode)
                {
                    return false;
                }
            }
            if (psr1.renderMode == ParticleSystemRenderMode.Mesh || psr2.renderMode == ParticleSystemRenderMode.Mesh)
            {
                if (psr1.renderMode != psr2.renderMode)
                {
                    return false;
                }
                if (psr1.mesh == null || psr2.mesh == null)
                {
                    return false;
                }
                if ((psr1.mesh.normals == null) ^ (psr2.mesh.normals == null))
                {
                    return false;
                }
                if ((psr1.mesh.tangents == null) ^ (psr2.mesh.tangents == null))
                {
                    return false;
                }
            }

            return true;
        }

        protected override void FindAllMeshes(List<Mesh> meshList)
        {
            meshList.Clear();
            ParticleSystemRenderer psr = renderer as ParticleSystemRenderer;
            if (psr != null && psr.renderMode == ParticleSystemRenderMode.Mesh)
            {
                if (psr.mesh != null)
                {
                    meshList.Add(psr.mesh);
                }
            }
        }
    }
}
