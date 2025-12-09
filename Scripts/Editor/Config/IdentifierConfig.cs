using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace EffectPerformanceAnalysis
{
    public abstract class EffectMarkAsset : ScriptableObject, ISerialize
    {
        public const string ASSET_PATH = "Assets/Editor/EffectMark.asset";

        [Header("特效文件夹路径")]
        [SerializeField]
        private string m_RootDirectoryAssetsPath;
        public string rootDirectoryAssetsPath
        {
            get
            {
                return m_RootDirectoryAssetsPath;
            }
        }

        [SerializeField]
        private List<EffectMarkConfig> m_ConfigList = new List<EffectMarkConfig>();
        private Dictionary<GameObject, EffectMarkConfig> m_ConfigDict = new Dictionary<GameObject, EffectMarkConfig>();

        private static EffectMarkAsset m_Instance;
        public static EffectMarkAsset instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = AssetDatabase.LoadAssetAtPath<EffectMarkAsset>(ASSET_PATH);
                    if (m_Instance != null)
                    {
                        m_Instance.Desrialize();
                    }
                }
                return m_Instance;
            }
        }

        protected virtual int interval
        {
            get
            {
                return 30;
            }
        }

        private void Awake()
        {
            var path = AssetDatabase.GetAssetPath(this).Replace("\\", "/");
            if (path != ASSET_PATH)
            {
                var asset = AssetDatabase.LoadAssetAtPath<EffectMarkAsset>(ASSET_PATH);
                if (asset != null)
                {
                    Debug.LogErrorFormat("{0} 已存在，请先删除！", ASSET_PATH);
                    return;
                }
                AssetDatabase.MoveAsset(path, ASSET_PATH);
            }
        }

        protected abstract int GetSortingOrderStart(string path, GameObject prefab);

        protected virtual RenderQueueArea[] GetRenderQueues(string path, GameObject prefab)
        {
            return null;
        }

        public abstract bool IsEffect(GameObject prefab);

        private EffectMarkConfig GetConfig(string path, GameObject prefab)
        {
            EffectMarkConfig config = new EffectMarkConfig();
            config.path = path;
            config.prefab = prefab;
            config.sortingOrderStart = Mathf.Clamp(GetSortingOrderStart(path, prefab), Performance.SORTING_ORDER_MIN, Performance.SORTING_ORDER_MAX);
            config.renderQueues = GetRenderQueues(path, prefab);
            config.name = string.Format("{0}[{1}]", path, config.sortingOrderStart);
            return config;
        }

        public void Mark()
        {
            if (string.IsNullOrWhiteSpace(m_RootDirectoryAssetsPath))
            {
                Debug.LogError("请先设置特效文件夹的路径!");
                return;
            }
            var rootPath = Path.Combine(Directory.GetCurrentDirectory(), m_RootDirectoryAssetsPath).Replace("\\", "/");
            var files = Directory.GetFiles(rootPath, "*.prefab", SearchOption.AllDirectories);
            if (files == null)
            {
                return;
            }
            Clear();
            Desrialize();
            foreach (var file in files)
            {
                var path = file.Substring(rootPath.Length + 1);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(m_RootDirectoryAssetsPath, path));
                if (prefab != null && IsEffect(prefab))
                {
                    m_ConfigDict[prefab] = GetConfig(path, prefab);
                }
            }
            Serialize();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            m_Instance = null;
        }

        public virtual void Clear()
        {
            m_ConfigList.Clear();
            m_ConfigDict.Clear();
        }

        public virtual void Serialize()
        {
            m_ConfigList.Clear();
            foreach (var item in m_ConfigDict.Values)
            {
                m_ConfigList.Add(item);
            }
            m_ConfigList.Sort((a, b) =>
            {
                return a.path.CompareTo(b.path);
            });
        }

        public virtual void Desrialize()
        {
            m_ConfigDict.Clear();
            foreach (var item in m_ConfigList)
            {
                m_ConfigDict[item.prefab] = item;
            }
        }

        public int GetSortingOrderStart(GameObject prefab)
        {
            if (prefab != null && m_ConfigDict.TryGetValue(prefab, out EffectMarkConfig config))
            {
                return config.sortingOrderStart;
            }
            return Performance.SORTING_ORDER_INVAILD;
        }
    }

    [Serializable]
    public struct EffectMarkConfig
    {
        public string name;
        public string path;
        public int sortingOrderStart;
        public RenderQueueArea[] renderQueues;
        public GameObject prefab;
    }

    public struct RenderQueueArea
    {
        public int renderQueue;
        public int min;
        public int max;
    }
}
