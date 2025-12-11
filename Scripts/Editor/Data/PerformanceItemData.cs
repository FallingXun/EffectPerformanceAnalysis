using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EffectPerformanceAnalysis
{
    public struct PerformanceNodeData
    {
        public Renderer renderer;
        public BaseNode effect;
        public int renderQueue;
        public int sortingOrder;
        public EComponentType componentType;
        public bool batch;
        public int batchCount;
        public int meshVertexCount;
        public int meshTriangleCount;
        public int renderVertexCount;
        public int renderTriangleCount;
        public int totalPassCount;
        public int textureCount;
        public int textureSize;
        public int textureMaxWidth;
        public int textureMaxHeight;
        public long textureMemory;

        public int correctSortingOrder;
        public int id;
        public int repeatId;

        public PerformanceNodeData(Renderer renderer, BaseNode effect)
        {
            this.renderer = renderer;
            this.effect = effect;
            this.renderQueue = effect.GetRenderQueue(renderer);
            this.sortingOrder = effect.GetSortingOrder(renderer);
            this.componentType = effect.nodeType;
            this.batch = effect.GetBatch(renderer);
            this.batchCount = effect.GetBatchCount(renderer);
            this.meshVertexCount = effect.GetMeshVertexCount(renderer);
            this.meshTriangleCount = effect.GetMeshTriangleCount(renderer);
            this.renderVertexCount = effect.GetRenderVertexCount(renderer);
            this.renderTriangleCount = effect.GetRenderTriangleCount(renderer);
            this.totalPassCount = effect.GetTotalPassCount(renderer);
            this.textureCount = effect.GetTextureCount(renderer);
            this.textureSize = effect.GetTextureSize(renderer);
            var textureMaxSize = effect.GetTextureMaxSize(renderer);
            this.textureMaxWidth = textureMaxSize.x;
            this.textureMaxHeight = textureMaxSize.y;
            this.textureMemory = effect.GetTextureMemory(renderer);

            this.correctSortingOrder = this.sortingOrder;
            this.id = 0;
            this.repeatId = 0;
        }

        public PerformanceNodeData SetCorrectSortingOrder(int correctSortingOrder)
        {
            this.correctSortingOrder = correctSortingOrder;
            return this;
        }

        public PerformanceNodeData SetId(int id)
        {
            this.id = id;
            return this;
        }

        public PerformanceNodeData SetRepeatId(int repeatId)
        {
            this.repeatId = repeatId;
            return this;
        }


        public int CompareTo(PerformanceNodeData target)
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
                result = (componentType == EComponentType.ParticleSystemRenderer).CompareTo(target.componentType == EComponentType.ParticleSystemRenderer);
                if (result != 0)
                {
                    return result;
                }
            }
            else
            {
                result = componentType.CompareTo(target.componentType);
                if (result != 0)
                {
                    return result;
                }
            }

            result = batch.CompareTo(target.batch);
            if (result != 0)
            {
                return result;
            }

            result = effect.CompareTo(renderer, target.renderer);
            return result;
        }

        public bool IsSameBatch(PerformanceNodeData target)
        {
            if (batch == false || target.batch == false)
            {
                return false;
            }
            if (Utils.UsingSRP())
            {
                if ((componentType == EComponentType.ParticleSystemRenderer) ^ (target.componentType == EComponentType.ParticleSystemRenderer))
                {
                    return false;
                }
                if (componentType == EComponentType.ParticleSystemRenderer)
                {
                    if (renderQueue != target.renderQueue)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (renderQueue != target.renderQueue)
                {
                    return false;
                }
                if (componentType != target.componentType)
                {
                    return false;
                }
            }
            return effect.IsSameBatch(renderer, target.renderer);
        }
    }

}
