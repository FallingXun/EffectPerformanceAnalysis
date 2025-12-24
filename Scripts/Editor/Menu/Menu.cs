using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
using System;

namespace EffectPerformanceAnalysis
{
    public class Menu : Editor
    {
        private const string EFFECT_PERFORMANCE_ANALYSIS = "EffectPerformanceAnalysis/";

        private const string GROUP_SCRIPTS = EFFECT_PERFORMANCE_ANALYSIS + "Scripts";
        private const string SCRIPTS_CREATE_ALL = EFFECT_PERFORMANCE_ANALYSIS + "Create All Scripts";
        private const string SCRIPTS_CREATE_EFFECT_CONFIG_ASSET = EFFECT_PERFORMANCE_ANALYSIS + "Create Effect Config Asset Scripts";
        private const string SCRIPTS_CREATE_CUSTOM_EDITOR = EFFECT_PERFORMANCE_ANALYSIS + "Create Custom Editor Scripts";

        private const string GROUP_DATA = EFFECT_PERFORMANCE_ANALYSIS + "Data";
        private const string DATA_CREATE_EFFECT_CONFIG_ASSET = EFFECT_PERFORMANCE_ANALYSIS + "Create Effect Config Asset";
        private const string DATA_COLLECT_ALL_EFFECTS = EFFECT_PERFORMANCE_ANALYSIS + "Collect All Effects";

        private const string GROUP_OTHER = EFFECT_PERFORMANCE_ANALYSIS + "Other";
        private const string OTHER_UPDATE_ALL_EFFECTS = EFFECT_PERFORMANCE_ANALYSIS + "Update All Effects 's Order In Layer";


        [MenuItem(GROUP_SCRIPTS, false, 1)]
        private static void _GROUP_SCRIPTS()
        {

        }

        [MenuItem(GROUP_SCRIPTS, true, 1)]
        private static bool __GROUP_SCRIPTS()
        {
            return false;
        }

        [MenuItem(SCRIPTS_CREATE_ALL, false, 2)]
        private static void _SCRIPTS_CREATE_ALL()
        {
            Utils.CreateScript("Editor/CustomEditor/CustomEffectConfigAsset.cs", "Template/CustomEffectConfigTemplate.txt");
            Utils.CreateScript("Editor/CustomEditor/MeshRendererEditor.cs", "Template/CustomEditorMeshRenderer.txt");
            Utils.CreateScript("Editor/CustomEditor/SkinnedMeshRendererEditor.cs", "Template/CustomEditorSkinnedMeshRenderer.txt");
            Utils.CreateScript("Editor/CustomEditor/TransformEditor.cs", "Template/CustomEditorTransform.txt");
            Utils.CreateScript("Editor/CustomEditor/CustomEffectConfigAssetEditor.cs", "Template/CustomEditorEffectConfig.txt");
            AssetDatabase.Refresh();
        }


        [MenuItem(SCRIPTS_CREATE_EFFECT_CONFIG_ASSET, false, 3)]
        private static void _SCRIPTS_CREATE_EFFECT_CONFIG_ASSET()
        {
            Utils.CreateScript("Editor/CustomEditor/CustomEffectConfigAsset.cs", "Template/CustomEffectConfigTemplate.txt");
            AssetDatabase.Refresh();
        }

        [MenuItem(SCRIPTS_CREATE_CUSTOM_EDITOR, false, 4)]
        private static void _SCRIPTS_CREATE_CUSTOM_EDITOR()
        {
            Utils.CreateScript("Editor/CustomEditor/MeshRendererEditor.cs", "Template/CustomEditorMeshRenderer.txt");
            Utils.CreateScript("Editor/CustomEditor/SkinnedMeshRendererEditor.cs", "Template/CustomEditorSkinnedMeshRenderer.txt");
            Utils.CreateScript("Editor/CustomEditor/TransformEditor.cs", "Template/CustomEditorTransform.txt");
            Utils.CreateScript("Editor/CustomEditor/CustomEffectConfigAssetEditor.cs", "Template/CustomEffectConfigTemplate.txt");
            AssetDatabase.Refresh();
        }


        [MenuItem(GROUP_DATA, false, 101)]
        private static void _GROUP_DATA()
        {

        }

        [MenuItem(GROUP_DATA, true, 101)]
        private static bool __GROUP_DATA()
        {
            return false;
        }

        [MenuItem(DATA_CREATE_EFFECT_CONFIG_ASSET, false, 102)]
        private static void _DATA_CREATE_EFFECT_CONFIG_ASSET()
        {
            var type = Assembly.Load("Assembly-CSharp-Editor").GetType("EffectPerformanceAnalysis.CustomEffectConfigAsset");
            if (type == null)
            {
                return;
            }
            var customEffectConfigAssetSO = ScriptableObject.CreateInstance(type);
            var customConfigDirPath = Path.GetDirectoryName(Path.Combine(Directory.GetCurrentDirectory(), Const.EFFECT_CONFIG_PATH));
            if (Directory.Exists(customConfigDirPath) == false)
            {
                Directory.CreateDirectory(customConfigDirPath);
            }
            AssetDatabase.CreateAsset(customEffectConfigAssetSO, Const.EFFECT_CONFIG_PATH);
            AssetDatabase.Refresh();
        }


        [MenuItem(DATA_COLLECT_ALL_EFFECTS, false, 103)]
        private static void _DATA_COLLECT_ALL_EFFECT()
        {
            var effectConfigAsset = AssetDatabase.LoadAssetAtPath<EffectConfigAsset>(Const.EFFECT_CONFIG_PATH);
            if (effectConfigAsset == null)
            {
                throw new Exception(string.Format("未找到特效配置文件，请先通过 '{0}' 生成！", DATA_CREATE_EFFECT_CONFIG_ASSET));
            }
            effectConfigAsset.Init();
            var dict = Pools.Get<Dictionary<int, GameObject>>();
            effectConfigAsset.CollectAllEffects(dict);
            if (dict.Count <= 0)
            {
                throw new Exception(string.Format("未找到特效预制体，请检查 CustomEffectConfigAsset.CollectAllEffects 是否正确实现！"));
            }
            effectConfigAsset.Clear();
            foreach (var item in dict)
            {
                effectConfigAsset.AddEffect(item.Value, item.Key);
            }
            effectConfigAsset.Save();
            Pools.Release(dict);
        }

        [MenuItem(GROUP_OTHER, false, 201)]
        private static void _GROUP_OTHER()
        {

        }

        [MenuItem(GROUP_OTHER, true, 201)]
        private static bool __GROUP_OTHER()
        {
            return false;
        }

        [MenuItem(OTHER_UPDATE_ALL_EFFECTS, false, 202)]
        private static void _OTHER_UPDATE_ALL_EFFECTS()
        {
            var effectConfigAsset = AssetDatabase.LoadAssetAtPath<EffectConfigAsset>(Const.EFFECT_CONFIG_PATH);
            if (effectConfigAsset == null)
            {
                throw new Exception(string.Format("未找到特效配置文件，请先通过 '{0}' 生成！", DATA_CREATE_EFFECT_CONFIG_ASSET));
            }
            effectConfigAsset.Init();
            if (effectConfigAsset.configList == null)
            {
                return;
            }
            foreach (var config in effectConfigAsset.configList)
            {
                if(config.prefab == null)
                {
                    continue;
                }
                var sortingOrderStart = effectConfigAsset.GetSortingOrderStart(config.prefab);
                var rootNode = Performance.Analyze(config.prefab, sortingOrderStart);
                if(rootNode == null)
                {
                    continue;
                }
                rootNode.Update();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

}