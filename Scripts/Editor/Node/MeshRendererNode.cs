using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public class MeshRendererNode : BaseNode
    {
        public override EComponentType nodeType
        {
            get
            {
                return EComponentType.MeshRenderer;
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
                if (Utils.UsingSRP())
                {
                    // todo: 阴影的 pass 需要剔除
                    foreach (var material in m_MaterialList)
                    {
                        if (material != null && material.passCount <= 1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
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
        }



        public override bool IsSameBatch(Renderer renderer, Renderer targetRenderer)
        {
            if (Utils.UsingSRP())
            {
                if (renderer == null || targetRenderer == null || renderer.sharedMaterial == null || targetRenderer.sharedMaterial == null)
                {
                    return false;
                }
                if (renderer.sharedMaterial.passCount > 1 || targetRenderer.sharedMaterial.passCount > 1)
                {
                    return false;
                }
                if (renderer.sharedMaterial.shader != targetRenderer.sharedMaterial.shader)
                {
                    return false;
                }
                ulong keywords1 = Utils.GetShaderKeywordsGroup(renderer.sharedMaterial, m_ShaderKeywordIdDict);
                ulong keywords2 = Utils.GetShaderKeywordsGroup(targetRenderer.sharedMaterial, m_ShaderKeywordIdDict);
                return keywords1 == keywords2;
            }
            else
            {
                if (renderer != null && targetRenderer != null && renderer.sharedMaterial == targetRenderer.sharedMaterial)
                {
                    if (renderer.sharedMaterial.passCount > 1 || targetRenderer.sharedMaterial.passCount > 1)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        protected override void FindAllMeshes(List<Mesh> meshList)
        {
            meshList.Clear();
            if (renderer != null)
            {
                var mf = renderer.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    meshList.Add(mf.sharedMesh);
                }
            }
        }
    }
}

