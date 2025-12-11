using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct NodeUnitData
    {
        public Renderer renderer;
        public EComponentType componentType;
        public List<RenderUnitData> renderUnitList;
        public int rendererIndex;

        public NodeUnitData(Renderer renderer,int rendererIndex)
        {
            this.renderer = renderer;
            this.rendererIndex = rendererIndex;
            this.componentType = RendererUtils.GetComponentType(renderer);
            var materials = Pools.Get<List<Material>>();
            MaterialUtils.GetMaterials(materials, renderer);
            if (materials.Count > 0)
            {
                this.renderUnitList = new List<RenderUnitData>();
                foreach (var material in materials)
                {
                    var rendererUnit = new RenderUnitData(material, renderer, rendererIndex);
                    renderUnitList.Add(rendererUnit);
                }
                renderUnitList.Sort((a, b) =>
                {
                    return a.CompareTo(b);
                });
            }
            else
            {
                this.renderUnitList = null;
            }
            Pools.Release(materials);
        }

        public int CompareTo(NodeUnitData target)
        {
            return ComparerUtils.Compare(this, target);
        }
    }
}