using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


namespace EffectPerformanceAnalysis
{
    public class MetricsUI : IGUI
    {
        private GameObject m_GameObject;
        private Transform m_Transform;
        private GameObject m_Prefab;

        private Vector2 m_ScrollViewTitlePos = Vector2.zero;
        private Vector2 m_ScrollViewPos = Vector2.zero;
        private bool m_IsEffect = false;
        private bool m_IsVaild = false;
        private int m_SortingOrderStart = Performance.SORTING_ORDER_INVAILD;

        private Dictionary<EPerformanceMetrics, MetricsItemUI> m_PerformanceMetricsDict = new Dictionary<EPerformanceMetrics, MetricsItemUI>();


        public void Init(GameObject go)
        {
            if (m_PerformanceMetricsDict.Count <= 0)
            {
                m_PerformanceMetricsDict[EPerformanceMetrics.Id] = new MetricsItemUI(EPerformanceMetrics.Id, "���[�ظ�ֵ]", 100, PlayerPrefs.GetInt(EPerformanceMetrics.Id.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.Batch] = new MetricsItemUI(EPerformanceMetrics.Batch, "����", 30, PlayerPrefs.GetInt(EPerformanceMetrics.Batch.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.RendererObject] = new MetricsItemUI(EPerformanceMetrics.RendererObject, "Renderer����", 200, PlayerPrefs.GetInt(EPerformanceMetrics.RendererObject.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.RenderQueue] = new MetricsItemUI(EPerformanceMetrics.RenderQueue, "RenderQueue", 100, PlayerPrefs.GetInt(EPerformanceMetrics.RenderQueue.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.OrderInLayer] = new MetricsItemUI(EPerformanceMetrics.OrderInLayer, "OrderInLayer[�Ƽ�ֵ]", 150, PlayerPrefs.GetInt(EPerformanceMetrics.OrderInLayer.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.Materials] = new MetricsItemUI(EPerformanceMetrics.Materials, "������", 100, PlayerPrefs.GetInt(EPerformanceMetrics.Materials.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.MeshVertexCount] = new MetricsItemUI(EPerformanceMetrics.MeshVertexCount, "Mesh������", 100, PlayerPrefs.GetInt(EPerformanceMetrics.MeshVertexCount.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.MeshTriangleCount] = new MetricsItemUI(EPerformanceMetrics.MeshTriangleCount, "Mesh����", 100, PlayerPrefs.GetInt(EPerformanceMetrics.MeshTriangleCount.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.RenderVertexCount] = new MetricsItemUI(EPerformanceMetrics.RenderVertexCount, "��Ⱦ������", 100, PlayerPrefs.GetInt(EPerformanceMetrics.RenderVertexCount.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.RenderTriangleCount] = new MetricsItemUI(EPerformanceMetrics.RenderTriangleCount, "��Ⱦ����", 100, PlayerPrefs.GetInt(EPerformanceMetrics.RenderTriangleCount.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.TotalPassCount] = new MetricsItemUI(EPerformanceMetrics.TotalPassCount, "Pass����", 100, PlayerPrefs.GetInt(EPerformanceMetrics.TotalPassCount.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.TextureCount] = new MetricsItemUI(EPerformanceMetrics.TextureCount, "��������", 100, PlayerPrefs.GetInt(EPerformanceMetrics.TextureCount.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.TextureSize] = new MetricsItemUI(EPerformanceMetrics.TextureSize, "�����ߴ�", 100, PlayerPrefs.GetInt(EPerformanceMetrics.TextureSize.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.TextureMaxSize] = new MetricsItemUI(EPerformanceMetrics.TextureMaxSize, "�������ߴ�[��-��]", 150, PlayerPrefs.GetInt(EPerformanceMetrics.TextureMaxSize.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.TextureMemory] = new MetricsItemUI(EPerformanceMetrics.TextureMemory, "������С", 100, PlayerPrefs.GetInt(EPerformanceMetrics.TextureMemory.ToString(), 1) > 0);
                m_PerformanceMetricsDict[EPerformanceMetrics.ParticleMaxCount] = new MetricsItemUI(EPerformanceMetrics.ParticleMaxCount, "���������", 100, PlayerPrefs.GetInt(EPerformanceMetrics.ParticleMaxCount.ToString(), 1) > 0);
            }
            if (m_GameObject == go)
            {
                return;
            }
            m_ScrollViewTitlePos = Vector2.zero;
            m_ScrollViewPos = Vector2.zero;
            m_IsEffect = false;

            m_GameObject = go;
            m_Transform = go.transform;

            var assetPath = AssetDatabase.GetAssetPath(go);
            if (string.IsNullOrEmpty(assetPath))
            {
                var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(go);
                if (prefabStage != null)
                {
                    if (prefabStage.prefabContentsRoot == go)
                    {
                        assetPath = prefabStage.assetPath;
                    }
                }
                else
                {
                    if (PrefabUtility.GetNearestPrefabInstanceRoot(go) == go)
                    {
                        assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
                    }
                }
            }
            if (string.IsNullOrEmpty(assetPath) == false)
            {
                m_Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            }
            else
            {
                m_Prefab = null;
            }
            if (m_Prefab == null)
            {
                m_IsVaild = false;
            }
            else
            {
                m_IsVaild = true;
            }
            if (EffectMarkAsset.instance && EffectMarkAsset.instance.IsEffect(m_Prefab))
            {
                m_IsEffect = true;
            }
            else
            {
                m_IsEffect = false;
            }
        }


        public void OnInspectorGUI()
        {
            if (m_IsVaild == false)
            {
                return;
            }

            EditorGUILayout.Space();
            if (m_IsEffect == false)
            {
                return;
            }
            EditorGUILayout.LabelField("����Ч������", GUILayout.Width(100), GUILayout.Height(20));

            m_SortingOrderStart = EffectMarkAsset.instance.GetSortingOrderStart(m_Prefab);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("OrderInLayer ��ʼֵ", GUILayout.Width(150));
            m_SortingOrderStart = EditorGUILayout.IntField(m_SortingOrderStart, GUILayout.Width(100));
            GUILayout.FlexibleSpace();

            PerformanceData performanceData = Performance.Analyze(m_GameObject, m_SortingOrderStart);

            if (GUILayout.Button("����", GUILayout.Width(50)))
            {
                if (performanceData.nodeDataList != null)
                {
                    for (int i = 0; i < performanceData.nodeDataList.Count; i++)
                    {
                        performanceData.nodeDataList[i].renderer.sortingOrder = performanceData.nodeDataList[i].correctSortingOrder - m_SortingOrderStart;
                    }
                    EditorUtility.SetDirty(m_GameObject);
                }
            }

            if (GUILayout.Button("����", GUILayout.Width(50)))
            {
                if (performanceData.nodeDataList != null)
                {
                    for (int i = 0; i < performanceData.nodeDataList.Count; i++)
                    {
                        performanceData.nodeDataList[i].renderer.sortingOrder = performanceData.nodeDataList[i].correctSortingOrder;
                    }
                    EditorUtility.SetDirty(m_GameObject);
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("������:{0}", performanceData.totalBatchCount), GUILayout.Width(200));
            EditorGUILayout.LabelField(string.Format("����������:{0}", performanceData.totalMaterialCount), GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("Mesh�ܶ�����:{0}", performanceData.totalMeshVertexCount), GUILayout.Width(200));
            EditorGUILayout.LabelField(string.Format("Mesh������:{0}", performanceData.totalMeshTriangleCount), GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("��Ⱦ�ܶ�����:{0}", performanceData.totalRenderVertexCount), GUILayout.Width(200));
            EditorGUILayout.LabelField(string.Format("��Ⱦ������:{0}", performanceData.totalRenderTriangleCount), GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("���������:{0}", performanceData.particleMaxCount), GUILayout.Width(200));
            EditorGUILayout.LabelField(string.Format("��������:{0}", performanceData.totalTextureCount), GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("�������ߴ�:{0}x{1}", (int)Math.Sqrt(performanceData.totalTextureSize), (int)Math.Sqrt(performanceData.totalTextureSize)), GUILayout.Width(200));
            EditorGUILayout.LabelField(string.Format("�������ߴ�[��-��]:{0}-{1}", performanceData.totalTextureMaxWidth, performanceData.totalTextureMaxHeight), GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("�������ڴ�:{0}", Utils.GetTextureSizeFormat(performanceData.totalTextureMemory)), GUILayout.Width(200));
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            float totalWidth = 50;
            GUILayout.BeginHorizontal();
            for (int i = 0; i < m_PerformanceMetricsDict.Count; i++)
            {
                var column = m_PerformanceMetricsDict[(EPerformanceMetrics)i];
                if (i % 4 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                bool show = column.show;
                column.show = EditorGUILayout.Toggle(column.show, GUILayout.Width(10), GUILayout.Height(23));
                if (show != column.show)
                {
                    PlayerPrefs.SetInt(column.performanceMetrics.ToString(), column.show ? 1 : 0);
                }
                EditorGUILayout.LabelField(" " + column.title, GUILayout.Width(150), GUILayout.Height(20));
                if (column.show)
                {
                    totalWidth += column.width;
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            if (performanceData.nodeDataList != null && performanceData.nodeDataList.Count > 0)
            {
                var pos1 = new Vector2(m_ScrollViewPos.x, m_ScrollViewTitlePos.y);
                m_ScrollViewTitlePos = GUILayout.BeginScrollView(pos1);
                GUILayout.BeginHorizontal();
                foreach (EPerformanceMetrics performanceMetrics in Enum.GetValues(typeof(EPerformanceMetrics)))
                {
                    var column = m_PerformanceMetricsDict[performanceMetrics];
                    if (column.show == false)
                    {
                        continue;
                    }
                    var width = column.width;
                    if (performanceMetrics == EPerformanceMetrics.Materials)
                    {
                        width *= performanceData.materialMaxCount;
                    }
                    EditorGUILayout.LabelField(" " + column.title, GUILayout.Width(width));
                }
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
                var pos2 = new Vector2(m_ScrollViewTitlePos.x, m_ScrollViewPos.y);
                m_ScrollViewPos = GUILayout.BeginScrollView(pos2, GUILayout.Height(Math.Min(400, performanceData.nodeDataList.Count * 20 + 20)));
                for (int i = 0; i < performanceData.nodeDataList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    foreach (EPerformanceMetrics performanceMetrics in Enum.GetValues(typeof(EPerformanceMetrics)))
                    {
                        var column = m_PerformanceMetricsDict[performanceMetrics];
                        if (column.show == false)
                        {
                            continue;
                        }
                        switch (performanceMetrics)
                        {
                            case EPerformanceMetrics.Id:
                                {
                                    if (performanceData.nodeDataList[i].repeatId > 0)
                                    {
                                        EditorGUILayout.TextField(string.Format("{0}[{1}]", performanceData.nodeDataList[i].id, performanceData.nodeDataList[i].repeatId), GUILayout.Width(column.width));
                                    }
                                    else
                                    {
                                        EditorGUILayout.IntField(performanceData.nodeDataList[i].id, GUILayout.Width(column.width));
                                    }
                                }
                                break;
                            case EPerformanceMetrics.RendererObject:
                                {
                                    EditorGUILayout.ObjectField(performanceData.nodeDataList[i].renderer, typeof(Renderer), GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.RenderQueue:
                                {
                                    EditorGUILayout.IntField(performanceData.nodeDataList[i].renderQueue, GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.OrderInLayer:
                                {
                                    if (performanceData.nodeDataList[i].sortingOrder == performanceData.nodeDataList[i].correctSortingOrder)
                                    {
                                        EditorGUILayout.IntField(performanceData.nodeDataList[i].sortingOrder, GUILayout.Width(column.width));
                                    }
                                    else
                                    {
                                        EditorGUILayout.TextField(string.Format("{0}[{1}]", performanceData.nodeDataList[i].sortingOrder, performanceData.nodeDataList[i].correctSortingOrder), GUILayout.Width(column.width));
                                    }
                                }
                                break;
                            case EPerformanceMetrics.Materials:
                                {
                                    for (int j = 0; j < performanceData.materialMaxCount; j++)
                                    {
                                        Material material = null;
                                        if (j < performanceData.nodeDataList[i].renderer.sharedMaterials.Length)
                                        {
                                            material = performanceData.nodeDataList[i].renderer.sharedMaterials[j];
                                        }
                                        EditorGUILayout.ObjectField(material, typeof(Material), GUILayout.Width(100));
                                    }
                                }
                                break;
                            case EPerformanceMetrics.Batch:
                                {
                                    EditorGUILayout.TextField(performanceData.nodeDataList[i].batch ? "��" : "��", GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.MeshVertexCount:
                                {
                                    EditorGUILayout.IntField(performanceData.nodeDataList[i].meshVertexCount, GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.MeshTriangleCount:
                                {
                                    EditorGUILayout.IntField(performanceData.nodeDataList[i].meshTriangleCount, GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.RenderVertexCount:
                                {
                                    EditorGUILayout.IntField(performanceData.nodeDataList[i].renderVertexCount, GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.RenderTriangleCount:
                                {
                                    EditorGUILayout.IntField(performanceData.nodeDataList[i].renderTriangleCount, GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.TotalPassCount:
                                {
                                    EditorGUILayout.IntField(performanceData.nodeDataList[i].totalPassCount, GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.TextureCount:
                                {
                                    EditorGUILayout.IntField(performanceData.nodeDataList[i].textureCount, GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.TextureSize:
                                {
                                    var size = (int)Math.Sqrt(performanceData.nodeDataList[i].textureSize);
                                    EditorGUILayout.TextField(string.Format("{0}x{1}", size, size), GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.TextureMaxSize:
                                {
                                    EditorGUILayout.TextField(string.Format("{0}-{1}", performanceData.nodeDataList[i].textureMaxWidth, performanceData.nodeDataList[i].textureMaxHeight), GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.TextureMemory:
                                {
                                    EditorGUILayout.TextField(Utils.GetTextureSizeFormat(performanceData.nodeDataList[i].textureMemory), GUILayout.Width(column.width));
                                }
                                break;
                            case EPerformanceMetrics.ParticleMaxCount:
                                {
                                    var count = 0;
                                    var ps = performanceData.nodeDataList[i].renderer.GetComponent<ParticleSystem>();
                                    if (ps != null)
                                    {
                                        count = ps.main.maxParticles;
                                    }
                                    EditorGUILayout.IntField(count, GUILayout.Width(column.width));
                                }
                                break;
                        }
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
        }
    }
}

