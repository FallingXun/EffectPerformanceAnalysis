using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct RenderUnitData
    {
        public Material material;
        public Renderer renderer;
        public Shader shader;
        public Mesh mesh;
        public int sharedMaterialIndex;


        public RenderUnitData(Material material, Renderer renderer, int sharedMaterialIndex)
        {
            this.material = material;
            this.renderer = renderer;
            this.sharedMaterialIndex = sharedMaterialIndex;
            this.shader = ShaderUtils.GetShader(material);
            this.mesh = MeshUtils.GetMesh(renderer);
        }
    }
}