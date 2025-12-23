namespace EffectPerformanceAnalysis
{
    public static class Const
    {
        public const int SORTING_ORDER_MIN = -32768;
        public const int SORTING_ORDER_MAX = 32767;
        public const int SORTING_ORDER_INVAILD = 32768;

        public const string METRICS_UI_TITLE = "【特效分析】";
        public const string METRICS_UI_SORTING_ORDER_START = "OrderInLayer 起始值";
        public const string METRICS_UI_EFFECT_ID = "特效 ID";
        public const string METRICS_UI_BATCH_COUNT = "批次";
        public const string METRICS_UI_RESET = "重置";
        public const string METRICS_UI_UPDATE = "更新";
        public const string METRICS_UI_LINE = "______________________________________________________________________________________________________________________________________________________________________________________________________";
        public const string METRICS_UI_INDEX = "序号";
        public const string METRICS_UI_OBJECT = "对象";

        public const string METRICS_NAME_SORTING_ORDER = "OrderInLayer";
        public const string METRICS_NAME_RENDER_QUEUE = "RenderQueue";
        public const string METRICS_NAME_MESH_VERTEX_COUNT = "网格顶点数";
        public const string METRICS_NAME_MESH_VERTEX_ATTRIBUTES = "网格顶点属性数";
        public const string METRICS_NAME_MESH_TRIANGLE_COUNT = "网格面数";
        public const string METRICS_NAME_RENDER_VERTEX_COUNT = "渲染顶点数";
        public const string METRICS_NAME_RENDER_TRIANGLE_COUNT = "渲染面数";
        public const string METRICS_NAME_MATERIAL_COUNT = "材质球数量";
        public const string METRICS_NAME_PASS_COUNT = "Pass数量";
        public const string METRICS_NAME_TEXTURE_COUNT = "纹理数量";
        public const string METRICS_NAME_TEXTURE_SIZE = "纹理尺寸";
        public const string METRICS_NAME_TEXTURE_MAX_WIDTH = "纹理最大宽度";
        public const string METRICS_NAME_TEXTURE_MAX_HEIGHT = "纹理最大高度";
        public const string METRICS_NAME_TEXTURE_MEMORY = "纹理内存";
        public const string METRICS_NAME_PARTICILE_MAX_COUNT = "粒子最大数量";



        public const string PACKAGE_NAME = "Packages/com.xun.effectperformanceanalysis";
        public const string ASSET_PATH = "Assets/EffectPerformanceAnalysis";
        public const string EFFECT_CONFIG_PATH = ASSET_PATH + "/Editor/CustomEffectConfig.asset";


    }
}