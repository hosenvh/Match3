using System;
using System.Collections.Generic;
using System.Linq;
using KitchenParadise.Utiltiy.Base;
using Match3.EditorTools.Editor.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Match3.EditorTools.Editor.Menus.Verification
{
    public class DuplicateMetaFileGuidFinder : EditorWindow
    {
        private class Asset
        {
            public string Path { get; }
            public string Guid { get; }

            public Asset(string path)
            {
                Path = path;
                Guid = AssetDatabase.AssetPathToGUID(path);
            }
        }

        [MenuItem("Golmorad/Verification/Duplicate MetaFiles' Guid Finder", priority = 124)]
        public static void OpenWindow()
        {
            OpenUnityWindow();

            void OpenUnityWindow() => GetWindow<DuplicateMetaFileGuidFinder>(title: "Duplicate MetaFiles' Guid Finder");
        }

        private void OnGUI()
        {
            DrawStartFindingButton(onClick: StartLoggingDuplicateGuids);

            void DrawStartFindingButton(Action onClick) => EditorUtilities.InsertButton(title: "Find Duplicate Guids", onClick);
        }

        private void StartLoggingDuplicateGuids()
        {
            List<Asset> assetsWithDuplicateGuids = FindAssetsWithDuplicateGuids();
            LogAssetsHasDuplicateGuid(assetsWithDuplicateGuids);
        }

        private List<Asset> FindAssetsWithDuplicateGuids()
        {
            IEnumerable<Asset> allAssets = GetAllAssets();
            return allAssets.FindDuplicates(asset => asset.Guid).ToList();

            IEnumerable<Asset> GetAllAssets()
            {
                string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
                return allAssetPaths.Select(assetPath => new Asset(assetPath)).ToList();
            }
        }

        private void LogAssetsHasDuplicateGuid(List<Asset> assetsWithDuplicateGuids)
        {
            assetsWithDuplicateGuids.DoForEachElement(todo: LogAssetHasDuplicateGuid);

            void LogAssetHasDuplicateGuid(Asset asset) => Debug.LogError(message: $"Duplicate GUID Found. Asset GUID: {asset.Guid}, Asset Path {asset.Path}.", context: AssetDatabase.LoadAssetAtPath<Object>(asset.Path));
        }
    }
}