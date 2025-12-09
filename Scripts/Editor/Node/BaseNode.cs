using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EffectPerformanceAnalysis
{
    public abstract class BaseNode
    {
        protected List<Material> m_MaterialList = new List<Material>();
        protected List<Texture> m_TextureList = new List<Texture>();
        protected List<Mesh> m_MeshList = new List<Mesh>();

        public abstract ENodeType nodeType { get; }

        public Renderer renderer { get; protected set; }


        public Material mainMaterial
        {
            get
            {
                if (m_MaterialList != null && m_MaterialList.Count > 0)
                {
                    return m_MaterialList[0];
                }
                return null;
            }
        }

        public Mesh mainMesh
        {
            get
            {
                if (m_MeshList != null && m_MeshList.Count > 0)
                {
                    return m_MeshList[0];
                }
                return null;
            }
        }



        #region Init

        public void Init(Renderer renderer)
        {
            this.renderer = renderer;

            FindAllMaterials(m_MaterialList);
            FindAllTextures(m_TextureList);
            FindAllMeshes(m_MeshList);
        }

        protected virtual void FindAllMaterials(List<Material> materialList)
        {
            materialList.Clear();
            if (renderer != null && renderer.sharedMaterials != null)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }
                    if (materialList.Contains(material))
                    {
                        continue;
                    }
                    materialList.Add(material);
                }
            }
            materialList.Sort((a, b) =>
            {
                return a.renderQueue.CompareTo(b.renderQueue);
            });
        }

        protected virtual void FindAllTextures(List<Texture> textureList)
        {
            textureList.Clear();

            if (renderer != null && renderer.sharedMaterials != null)
            {
                var module = Performance.effectData.materialModule;
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null)
                    {
                        continue;
                    }
                    var nameIds = material.GetTexturePropertyNameIDs();
                    if (nameIds == null)
                    {
                        continue;
                    }
                    foreach (var nameId in nameIds)
                    {
                        var texture = material.GetTexture(nameId);
                        if (texture == null)
                        {
                            continue;
                        }
                        if (textureList.Contains(texture))
                        {
                            continue;
                        }
                        textureList.Add(texture);
                    }
                }
            }
        }

        protected virtual void FindAllMeshes(List<Mesh> meshList)
        {
            meshList.Clear();
        }

        #endregion

        #region Metrics

        public virtual int renderQueue
        {
            get
            {
                var value = int.MaxValue;
                if (mainMaterial != null)
                {
                    value = mainMaterial.renderQueue;
                }
                return value;
            }
        }

        public virtual int sortingOrder
        {
            get
            {
                if (renderer != null)
                {
                    return renderer.sortingOrder;
                }
                return int.MaxValue;
            }
        }

        public virtual bool canBatch
        {
            get
            {
                if(mainMaterial == null)
                {
                    return false;
                }
                // todo: 阴影的pass需要剔除
                if(mainMaterial.passCount > 1)
                {
                    return false;
                }
                return true;
            }
        }

        public virtual int batch
        {
            get
            {
                return totalPassCount;
            }
        }

        public virtual int meshVertexCount
        {
            get
            {
                if (mainMesh != null)
                {
                    return mainMesh.vertexCount;
                }
                return 0;
            }
        }

        public virtual int meshVertexAttributeCount
        {
            get
            {
                if (mainMesh != null)
                {
                    return mainMesh.vertexAttributeCount * meshVertexCount;
                }
                return 0;
            }
        }

        public virtual int meshTriangleCount
        {
            get
            {
                if (mainMesh != null)
                {
                    return mainMesh.triangles.Length / 3;
                }
                return 0;
            }
        }

        public virtual int renderVertexCount
        {
            get
            {
                return totalPassCount * meshVertexCount;
            }
        }

        public virtual int renderTriangleCount
        {
            get
            {
                return totalPassCount * meshTriangleCount;
            }
        }

        public virtual int totalPassCount
        {
            get
            {
                var count = 0;
                if (mainMaterial == null)
                {
                    return count;
                }
                MaterialModule module = Performance.effectData.materialModule;
                if (Utils.UsingSRP())
                {
                    count = mainMaterial.passCount;
                    for (int i = 1; i < m_MaterialList.Count; i++)
                    {
                        bool batch = module.SRPBatch(m_MaterialList[i - 1], m_MaterialList[i]);
                        if (batch)
                        {
                            continue;
                        }
                        count += m_MaterialList[i].passCount;
                    }
                }
                else
                {
                    count = mainMaterial.passCount;
                    for (int i = 1; i < m_MaterialList.Count; i++)
                    {
                        bool batch = module.DynamicBatching(m_MaterialList[i - 1], m_MaterialList[i], mainMesh, mainMesh);
                        if (batch)
                        {
                            continue;
                        }
                        count += m_MaterialList[i].passCount;
                    }
                }
                return count;
            }
        }

        public virtual int textureCount
        {
            get
            {
                if (m_TextureList != null)
                {
                    return m_TextureList.Count;
                }
                return 0;
            }
        }

        public virtual int textureSize
        {
            get
            {
                var size = 0;
                if (m_TextureList != null)
                {
                    foreach (var texture in m_TextureList)
                    {
                        size += texture.width * texture.height;
                    }
                }
                return size;
            }
        }

        public virtual int textureMaxWidth
        {
            get
            {
                int maxWidth = 0;
                if (m_TextureList != null)
                {
                    foreach (var texture in m_TextureList)
                    {
                        maxWidth = Mathf.Max(maxWidth, texture.width);
                    }
                }
                return maxWidth;
            }
        }

        public virtual int textureMaxHeight
        {
            get
            {
                int maxHeight = 0;
                if (m_TextureList != null)
                {
                    foreach (var texture in m_TextureList)
                    {
                        maxHeight = Mathf.Max(maxHeight, texture.height);
                    }
                }
                return maxHeight;
            }
        }


        public virtual long textureMemory
        {
            get
            {
                long memory = 0;
                if (m_TextureList != null)
                {
                    foreach (var texture in m_TextureList)
                    {
                        memory += Utils.GetTextureMemory(texture);
                    }
                }
                return memory;
            }
        }
        #endregion

        #region Compare

        public virtual int CompareTo(BaseNode target)
        {
            var result = renderQueue.CompareTo(target.renderQueue);
            if (result != 0)
            {
                return result;
            }

            result = sortingOrder.CompareTo(target.sortingOrder);
            if (result != 0)
            {
                return result;
            }

            if (Utils.UsingSRP())
            {
                // SBP 除粒子外都有可能合批
                result = (nodeType == ENodeType.ParticleSystem).CompareTo(target.nodeType == ENodeType.ParticleSystem);
                if (result != 0)
                {
                    return result;
                }
            }
            else
            {
                result = nodeType.CompareTo(target.nodeType);
                if (result != 0)
                {
                    return result;
                }
            }

            result = canBatch.CompareTo(target.canBatch);
            if (result != 0)
            {
                return result;
            }

            // 对比材质
            {
                var materialCount = Mathf.Min(m_MaterialList.Count, target.m_MaterialList.Count);
                MaterialModule module = Performance.effectData.materialModule;
                for (int i = 0; i < materialCount; i++)
                {
                    var id1 = module.GetMaterialId(m_MaterialList[i]);
                    var id2 = module.GetMaterialId(target.m_MaterialList[i]);
                    result = id1.CompareTo(id2);
                    if (result != 0)
                    {
                        return result;
                    }
                }

                result = m_MaterialList.Count.CompareTo(target.m_MaterialList.Count);
                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }

        public virtual bool IsSameBatch(BaseNode target)
        {
            if (Utils.UsingSRP())
            {
                if (mainMaterial == null || target.mainMaterial == null)
                {
                    return false;
                }
                if (mainMaterial.passCount > 1 || target.mainMaterial.passCount > 1)
                {
                    return false;
                }
                if (mainMaterial.shader != target.mainMaterial.shader)
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

        #endregion
    }

}
