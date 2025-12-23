using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

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
    }

}