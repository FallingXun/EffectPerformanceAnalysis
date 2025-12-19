using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct RenderUnitData
    {
        public Material material;
        public Mesh mesh;
        public Shader shader;
        public EComponentType componentType;
        public Renderer renderer;

        public RenderUnitData(Material material, Renderer renderer)
        {
            this.material = material;
            this.renderer = renderer;
            this.shader = ShaderUtils.GetShader(material);
            this.componentType = RendererUtils.GetComponentType(renderer);
            this.mesh = MeshUtils.GetMesh(renderer);
        }

        public int CompareTo(RenderUnitData target)
        {
            return ComparerUtils.Compare(this, target);
        }

        public bool CanBatch(RenderUnitData target)
        {
            return BatchUtils.Batch(this, target);
        }
    }
}