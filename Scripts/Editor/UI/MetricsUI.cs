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

        private Vector2 m_ScrollViewTitlePos = Vector2.zero;
        private Vector2 m_ScrollViewPos = Vector2.zero;
        private bool m_IsEffect = false;
        private bool m_IsVaild = false;
        private int m_SortingOrderStart = Const.SORTING_ORDER_INVAILD;

        private Dictionary<EMetrics, string> m_MetricsNameDict = new Dictionary<EMetrics, string>();
        private Dictionary<EMetrics, bool> m_MetricsStateDict = new Dictionary<EMetrics, bool>();

        private GUIStyle m_TitleStyle = null;

        public void Init(GameObject go)
        {
            if (m_TitleStyle == null)
            {
                m_TitleStyle = new GUIStyle();
                m_TitleStyle.alignment = TextAnchor.MiddleCenter;
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

            if(m_EffectConfigAsset == null)
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
            EditorGUILayout.LabelField(Const.METRICS_UI_TITLE, GUILayout.Width(100), GUILayout.Height(20));

            m_SortingOrderStart = m_EffectConfigAsset.GetSortingOrderStart(m_Prefab);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Const.METRICS_UI_SORTING_ORDER_START, GUILayout.Width(150));
            m_SortingOrderStart = EditorGUILayout.IntField(m_SortingOrderStart, GUILayout.Width(100));
            GUILayout.FlexibleSpace();

            RootNode rootNode = Performance.Analyze(m_GameObject, m_SortingOrderStart);

            if (GUILayout.Button(Const.METRICS_UI_RESET, GUILayout.Width(50)))
            {
                if (rootNode.count > 0)
                {
                    for (int i = 0; i < rootNode.count; i++)
                    {
                        rootNode[i][0].renderer.sortingOrder = rootNode[i].sortingOrderRecommend - m_SortingOrderStart;
                    }
                    EditorUtility.SetDirty(m_GameObject);
                }
            }

            if (GUILayout.Button(Const.METRICS_UI_UPDATE, GUILayout.Width(50)))
            {
                if (rootNode.count > 0)
                {
                    for (int i = 0; i < rootNode.count; i++)
                    {
                        rootNode[i][0].renderer.sortingOrder = rootNode[i].sortingOrderRecommend;
                    }
                    EditorUtility.SetDirty(m_GameObject);
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
            {
                if ((int)metricsType % 2 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                var name = m_MetricsNameDict[metricsType];
                var value = GetMetricsValue(rootNode, metricsType);
                var state = m_MetricsStateDict[metricsType];
                m_MetricsStateDict[metricsType] = EditorGUILayout.ToggleLeft(string.Format("{0}:{1}", name, value), state, GUILayout.Width(200));
                if (state != m_MetricsStateDict[metricsType])
                {
                    PlayerPrefs.SetInt(metricsType.ToString(), m_MetricsStateDict[metricsType] ? 1 : 0);
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Const.METRICS_UI_LINE);
            if (rootNode.count > 0)
            {
                var pos1 = new Vector2(m_ScrollViewPos.x, m_ScrollViewTitlePos.y);
                m_ScrollViewTitlePos = GUILayout.BeginScrollView(pos1);
                GUILayout.BeginHorizontal();
                foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
                {
                    if (m_MetricsStateDict[metricsType] == false)
                    {
                        continue;
                    }
                    EditorGUILayout.LabelField(m_MetricsNameDict[metricsType], m_TitleStyle, GUILayout.Width(150));
                }
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
                var pos2 = new Vector2(m_ScrollViewTitlePos.x, m_ScrollViewPos.y);
                m_ScrollViewPos = GUILayout.BeginScrollView(pos2, GUILayout.Height(Math.Min(400, rootNode.count * 20 + 20)));
                for (int i = 0; i < rootNode.count; i++)
                {
                    GUILayout.BeginHorizontal();
                    foreach (EMetrics metricsType in Enum.GetValues(typeof(EMetrics)))
                    {
                        if (m_MetricsStateDict[metricsType] == false)
                        {
                            continue;
                        }
                        var value = GetMetricsValue(rootNode[i], metricsType);
                        EditorGUILayout.TextField(value, m_TitleStyle, GUILayout.Width(150));
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
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

