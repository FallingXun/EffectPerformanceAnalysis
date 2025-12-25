using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine.Rendering;

namespace EffectPerformanceAnalysis
{
    public class Utils
    {
        public static string GetPackageRootPath()
        {
            var path = Path.GetFullPath(Const.PACKAGE_NAME);
            return path;
        }

        public static bool UsingSRP()
        {
            return GraphicsSettings.currentRenderPipeline != null;
        }


        public static GameObject GetPrefab(GameObject go)
        {
            if (go != null)
            {
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
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (prefab != null)
                    {
                        return prefab;
                    }
                }
            }
            return null;
        }

        public static void CreateScript(string assetsPath, string templatePath)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), Const.ASSET_PATH, assetsPath);
            if (File.Exists(path))
            {
                return;
            }
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            using (var file = File.CreateText(path))
            {
                var content = File.ReadAllText(Path.Combine(GetPackageRootPath(), templatePath));
                if (string.IsNullOrEmpty(content))
                {
                    throw new Exception(string.Format("´´½¨½Å±¾Ê§°Ü£º{0}", templatePath));
                }
                file.Write(content);
            }
        }


    }
}
