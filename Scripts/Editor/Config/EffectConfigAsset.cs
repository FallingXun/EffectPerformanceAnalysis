using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace EffectPerformanceAnalysis
{
    public abstract class EffectConfigAsset : ScriptableObject
    {
        [Header("单个特效分配的 OrderInLayer 数量上限")]
        [SerializeField]
        private int m_SortingOrderLimit = 30;


        [SerializeField]
        [Header("特效列表")]
        private List<EffectConfig> m_ConfigList = new List<EffectConfig>();

        private Dictionary<GameObject, EffectConfig> m_ConfigDict = new Dictionary<GameObject, EffectConfig>();
        private Dictionary<int, GameObject> m_IdDict = new Dictionary<int, GameObject>();

        public virtual void Init()
        {
            Desrialize();
        }

        public virtual bool IsEffect(GameObject go)
        {
            var prefab = Utils.GetPrefab(go);
            if (prefab != null && m_ConfigDict.ContainsKey(prefab))
            {
                return true;
            }
            return false;
        }

        public void AddEffect(GameObject go, int id)
        {
            var prefab = Utils.GetPrefab(go);
            if (prefab == null)
            {
                throw new Exception(string.Format("找不到对应的 prefab ，请检查！ 特效名：{0}", go.name));
            }
            if (id < 0 || id > Const.SORTING_ORDER_INVAILD / m_SortingOrderLimit)
            {
                throw new Exception(string.Format("id 范围必须为 [0, {0}]", Const.SORTING_ORDER_INVAILD / m_SortingOrderLimit));
            }

            if (m_IdDict.TryGetValue(id, out GameObject gameObject))
            {
                if (gameObject != prefab)
                {
                    throw new Exception(string.Format("当前 id 已被使用，请检查！ id：{0} ，已使用特效名：{1} ，当前特效名：{2}", id, prefab.name, go.name));
                }
            }
            m_IdDict[id] = prefab;

            EffectConfig config = new EffectConfig();
            config.id = id;
            config.prefab = prefab;
            config.name = prefab.name;
            m_ConfigDict[prefab] = config;
        }

        public void Save()
        {
            Serialize();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }


        public virtual void Clear()
        {
            m_ConfigList.Clear();
            m_ConfigDict.Clear();
            m_IdDict.Clear();
        }

        public virtual void Serialize()
        {
            m_ConfigList.Clear();
            m_ConfigList.AddRange(m_ConfigDict.Values);
            m_ConfigList.Sort((a, b) =>
            {
                return a.id.CompareTo(b.id);
            });
        }

        public virtual void Desrialize()
        {
            m_ConfigDict.Clear();
            m_IdDict.Clear();
            foreach (var item in m_ConfigList)
            {
                m_ConfigDict[item.prefab] = item;
                m_IdDict[item.id] = item.prefab;
            }
        }

        public int GetSortingOrderStart(GameObject go)
        {
            var prefab = Utils.GetPrefab(go);
            if (prefab != null && m_ConfigDict.TryGetValue(prefab, out EffectConfig config))
            {
                var sortingOrderStart = config.id * m_SortingOrderLimit + Const.SORTING_ORDER_MIN;
                sortingOrderStart = Mathf.Min(sortingOrderStart, Const.SORTING_ORDER_INVAILD);
                return sortingOrderStart;
            }
            return Const.SORTING_ORDER_INVAILD;
        }
    }

    [Serializable]
    public struct EffectConfig
    {
        public int id;
        public GameObject prefab;
        public string name;
    }
}
