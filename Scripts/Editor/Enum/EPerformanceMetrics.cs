namespace EffectPerformanceAnalysis
{
    public enum EPerformanceMetrics
    {
        [Display("序号")]
        Id = 0,
        [Display("Renderer对象")]
        RendererObject,
        [Display("RenderQueue")]
        RenderQueue,
        [Display("OrderInLayer")]
        OrderInLayer,
        [Display("材质球")]
        MaterialObjects,
        [Display("材质组")]
        MaterialGroup,
        [Display("合批")]
        Batch,
        [Display("Mesh顶点数")]
        MeshVertices,
        [Display("Mesh顶点属性数")]
        MeshVertexAttributes,
        [Display("Mesh面数")]
        MeshTriangles,
        [Display("渲染顶点数")]
        RenderVertices,
        [Display("渲染面数")]
        RenderTriangles,
        [Display("Pass数量")]
        PassCount,
        [Display("纹理数量")]
        TextureCount,
        [Display("纹理大小")]
        TextureSize,

        Materials,
        MeshVertexCount,
        MeshTriangleCount,
        RenderVertexCount,
        RenderTriangleCount,
        TotalPassCount,
        TextureMaxSize,
        TextureMemory,
        ParticleMaxCount,
    }
}