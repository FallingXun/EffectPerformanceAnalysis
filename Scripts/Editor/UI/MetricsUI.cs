using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


namespace EffectPerformanceAnalysis
{
    public class MetricsUI : IGUI
    {
        private EffectConfigAsset m_EffectConfigAsset;
        private GameObject m_GameObject;
        private Transform m_Transform;
        private GameObject m_Prefab;
        private RootNode m_RootNode;

        private Vector2 m_ScrollViewTitlePos = Vector2.zero;
        private Vector2 m_ScrollViewPos = Vector2.zero;
        private bool m_IsEffect = false;
        private bool m_IsVaild = false;
        private int m_SortingOrderStart = Const.SORTING_ORDER_INVAILD;
        private int m_EffectId = -1;
        private int m_BatchCount = 0;

        private Dictionary<EMetrics, string> m_MetricsNameDict = new Dictionary<EMetrics, string>();
        private Dictionary<EMetrics, bool> m_MetricsStateDict = new Dictionary<EMetrics, bool>();

        private GUIStyle m_MainTitleStyle = null;
        private GUIStyle m_MainStyle = null;
        private GUIStyle m_ToggleStyle = null;
        private GUIStyle m_TitleStyle = null;
        private GUIStyle m_TextStyle = null;
        private int m_IndexWidth = 50;
        private int m_ObjectWidth = 150;
        private int m_MetricsWidth = 100;
        private int m_ButtonWidth = 50;
        private int m_MetricsStateWidth = 200;

        public void Init(GameObject go)
        {
            if (m_MainTitleStyle == null)
            {
                m_MainTitleStyle = new GUIStyle();
                m_MainTitleStyle.normal.textColor = Color.white;
                m_MainTitleStyle.fontSize = 14;
                m_MainTitleStyle.fontStyle = FontStyle.Bold;
                m_MainTitleStyle.fixedWidth = 200;
                m_MainTitleStyle.fixedHeight = 16;
            }
            if (m_MainStyle == null)
            {
                m_MainStyle = new GUIStyle();
                m_MainStyle.normal.textColor = Color.white;
                m_MainStyle.richText = true;
                m_MainStyle.fixedWidth = 100;
            }
            if (m_ToggleStyle == null)
            {
                m_ToggleStyle = new GUIStyle();
                m_ToggleStyle.normal.textColor = Color.white;
                m_ToggleStyle.richText = true;
            }
            if (m_TitleStyle == null)
            {
                m_TitleStyle = new GUIStyle();
                m_TitleStyle.alignment = TextAnchor.MiddleCenter;
                m_TitleStyle.normal.textColor = Color.white;
            }
            if (m_TextStyle == null)
            {
                m_TextStyle = new GUIStyle();
                m_TextStyle.alignment = TextAnchor.MiddleCenter;
                m_TextStyle.normal.textColor = Color.white;
                m_TextStyle.richText = true;
            }

            m_MetricsNameDict.Clear();
            m_MetricsStateDict.Clear();
            foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
            {
                var attributes = metricsType.GetType().GetField(metricsType.ToString()).GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    if (attribute is DisplayAttribute display)
                    {
                        m_MetricsNameDict[metricsType] = display.name;
                    }
                }
                m_MetricsStateDict[metricsType] = PlayerPrefs.GetInt(metricsType.ToString(), 1) > 0;

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

            m_Prefab = Utils.GetPrefab(go);
            if (m_Prefab == null)
            {
                m_IsVaild = false;
            }
            else
            {
                m_IsVaild = true;
            }

            if (m_EffectConfigAsset == null)
            {
                m_EffectConfigAsset = AssetDatabase.LoadAssetAtPath<EffectConfigAsset>(Const.EFFECT_CONFIG_PATH);
                if (m_EffectConfigAsset != null)
                {
                    m_EffectConfigAsset.Init();
                }
            }
            if (m_EffectConfigAsset != null && m_EffectConfigAsset.IsEffect(m_Prefab))
            {
                m_IsEffect = true;
            }
            else
            {
                m_IsEffect = false;
            }

            if (m_IsVaild && m_IsEffect)
            {
                m_RootNode = Performance.Analyze(m_GameObject, m_SortingOrderStart);
                m_SortingOrderStart = m_EffectConfigAsset.GetSortingOrderStart(m_Prefab);
                m_EffectId = m_EffectConfigAsset.GetEffectId(m_Prefab);
                m_BatchCount = m_RootNode.passCount;
            }
            else
            {
                m_RootNode = null;
                m_SortingOrderStart = Const.SORTING_ORDER_INVAILD;
                m_EffectId = -1;
                m_BatchCount = 0;
            }
        }


        public void OnInspectorGUI()
        {
            if (m_RootNode == null)
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Const.METRICS_UI_LINE);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Const.METRICS_UI_TITLE, m_MainTitleStyle);

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("{0}£º{1}", Const.METRICS_UI_EFFECT_ID, m_EffectId), m_MainStyle);
            EditorGUILayout.LabelField(string.Format("{0}£º{1}", Const.METRICS_UI_BATCH_COUNT, m_BatchCount), m_MainStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("{0}£º{1}", Const.METRICS_UI_SORTING_ORDER_START, m_SortingOrderStart), m_MainStyle);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(Const.METRICS_UI_RESET, GUILayout.Width(m_ButtonWidth)))
            {
                if (m_RootNode.count > 0)
                {
                    for (int i = 0; i < m_RootNode.count; i++)
                    {
                        m_RootNode[i][0].renderer.sortingOrder = m_RootNode[i].sortingOrderRecommend - m_SortingOrderStart;
                    }
                    EditorUtility.SetDirty(m_GameObject);
                }
            }

            if (GUILayout.Button(Const.METRICS_UI_UPDATE, GUILayout.Width(m_ButtonWidth)))
            {
                if (m_RootNode.count > 0)
                {
                    for (int i = 0; i < m_RootNode.count; i++)
                    {
                        m_RootNode[i][0].renderer.sortingOrder = m_RootNode[i].sortingOrderRecommend;
                    }
                    EditorUtility.SetDirty(m_GameObject);
                }
            }
            GUILayout.EndHorizontal();


            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
            {
                if ((int)metricsType % 3 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                var name = m_MetricsNameDict[metricsType];
                var value = GetMetricsValue(m_RootNode, metricsType);
                var state = m_MetricsStateDict[metricsType];
                m_MetricsStateDict[metricsType] = EditorGUILayout.ToggleLeft(string.Format("{0}£º{1}", name, value), state, m_ToggleStyle, GUILayout.Width(m_MetricsStateWidth));
                if (state != m_MetricsStateDict[metricsType])
                {
                    PlayerPrefs.SetInt(metricsType.ToString(), m_MetricsStateDict[metricsType] ? 1 : 0);
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Const.METRICS_UI_LINE);
            if (m_RootNode.count > 0)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Const.METRICS_UI_INDEX, m_TitleStyle, GUILayout.Width(m_IndexWidth));
                EditorGUILayout.LabelField(Const.METRICS_UI_OBJECT, m_TitleStyle, GUILayout.Width(m_ObjectWidth));
                var pos1 = new Vector2(m_ScrollViewPos.x, m_ScrollViewTitlePos.y);
                m_ScrollViewTitlePos = GUILayout.BeginScrollView(pos1);
                GUILayout.BeginHorizontal();
                foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
                {
                    if (m_MetricsStateDict[metricsType] == false)
                    {
                        continue;
                    }
                    EditorGUILayout.LabelField(m_MetricsNameDict[metricsType], m_TitleStyle, GUILayout.Width(m_MetricsWidth));
                }
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                for (int i = 0; i < m_RootNode.count; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField((i + 1).ToString(), m_TextStyle, GUILayout.Width(m_IndexWidth));
                    EditorGUILayout.ObjectField(m_RootNode[i][0].renderer, typeof(Renderer), GUILayout.Width(m_ObjectWidth));
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                var pos2 = new Vector2(m_ScrollViewTitlePos.x, m_ScrollViewPos.y);
                m_ScrollViewPos = GUILayout.BeginScrollView(pos2, GUILayout.Height(Math.Min(400, m_RootNode.count * 20 + 20)));
                for (int i = 0; i < m_RootNode.count; i++)
                {
                    GUILayout.BeginHorizontal();
                    foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
                    {
                        if (m_MetricsStateDict[metricsType] == false)
                        {
                            continue;
                        }
                        var value = GetMetricsValue(m_RootNode[i], metricsType);
                        EditorGUILayout.LabelField(value, m_TextStyle, GUILayout.Width(m_MetricsWidth));
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
                GUILayout.EndHorizontal();
            }
        }

        public string GetMetricsValue(IMetrics metrics, EMetrics metricsType)
        {
            string value = "";
            switch (metricsType)
            {
                case EMetrics.OrderInLayer:
                    {
                        if (metrics is RenderNode renderNode)
                        {
                            value = string.Format("{0}[{1}]", renderNode.sortingOrder, renderNode.sortingOrderRecommend);
                        }
                        else
                        {
                            value = metrics.sortingOrder.ToString();
                        }
                    }
                    break;
                case EMetrics.RenderQueue:
                    {
                        value = metrics.renderQueue.ToString();
                    }
                    break;
                case EMetrics.MeshVertexCount:
                    {
                        value = metrics.meshVertexCount.ToString();
                    }
                    break;
                case EMetrics.MeshVertexAttributes:
                    {
                        value = metrics.meshVertexAttributeCount.ToString();
                    }
                    break;
                case EMetrics.MeshTriangleCount:
                    {
                        value = metrics.meshTriangleCount.ToString();
                    }
                    break;
                case EMetrics.RenderVertexCount:
                    {
                        value = metrics.renderVertexCount.ToString();
                    }
                    break;
                case EMetrics.RenderTriangleCount:
                    {
                        value = metrics.renderTriangleCount.ToString();
                    }
                    break;
                case EMetrics.MaterialCount:
                    {
                        value = metrics.materialCount.ToString();
                    }
                    break;
                case EMetrics.PassCount:
                    {
                        value = metrics.passCount.ToString();
                    }
                    break;
                case EMetrics.TextureCount:
                    {
                        value = metrics.textureCount.ToString();
                    }
                    break;
                case EMetrics.TextureSize:
                    {
                        value = TextureUtils.GetTextureSizeFormat(metrics.textureSize);
                    }
                    break;
                case EMetrics.TextureMaxWidth:
                    {
                        value = metrics.textureMaxWidth.ToString();
                    }
                    break;
                case EMetrics.TextureMaxHeight:
                    {
                        value = metrics.textureMaxHeight.ToString();
                    }
                    break;
                case EMetrics.TextureMemory:
                    {
                        value = TextureUtils.GetTextureMemoryFormat(metrics.textureMemory);
                    }
                    break;
                case EMetrics.ParticleMaxCount:
                    {
                        value = metrics.particleMaxCount.ToString();
                    }
                    break;
            }
            return value;
        }

    }
}

