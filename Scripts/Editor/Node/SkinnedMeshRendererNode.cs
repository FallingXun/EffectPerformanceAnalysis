using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EffectPerformanceAnalysis
{
    public class SkinnedMeshRendererNode : BaseNode
    {
        public override EComponentType nodeType
        {
            get
            {
                return EComponentType.SkinnedMeshRenderer;
            }
        }

        public override bool canBatch
        {
            get
            {
                if (Utils.UsingSRP())
                {
                    if (m_MaterialList != null)
                    {
                        foreach (var material in m_MaterialList)
                        {
                            if (material != null && material.passCount <= 1)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                else
                {
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
                return false;
            }
        }

        protected override void FindAllMeshes(List<Mesh> meshList)
        {
            meshList.Clear();
            var smr = renderer as SkinnedMeshRenderer;
            if (smr != null && smr.sharedMesh != null)
            {
                meshList.Add(smr.sharedMesh);
            }
        }
    }

}
