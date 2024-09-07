using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Menus.Utility
{
    public static class ReSerializer
    {
        [MenuItem("Golmorad/Utility/Force ReSerialize Whole Project", isValidateFunction: false, priority = 501)]
        public static void ReserializeWholeProject()
        {
            AssetDatabase.ForceReserializeAssets();
        }

        [MenuItem("Assets/Golmorad/Utility/Force ReSerialize Asset(s)", isValidateFunction: true)]
        public static bool IsPossibleToReserializeSelectedAssets()
        {
            return IsSelectedObjectAnAsset();

            bool IsSelectedObjectAnAsset() => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Selection.activeObject)) == false;
        }

        [MenuItem("Assets/Golmorad/Utility/Force ReSerialize Asset(s)", priority = 1)]
        public static void ReserializeSelectedAssets()
        {
            List<string> selectedPaths = GetSelectedPaths();
            AssetDatabase.ForceReserializeAssets(selectedPaths);
        }

        private static List<string> GetSelectedPaths()
        {
            var selectedPaths = new List<string>();
            foreach (Object selectedAsset in Selection.objects)
            {
                List<string> paths = GetSelectedPathsBasedOnSelectedAsset(selectedAsset);
                selectedPaths.AddRange(paths);
            }

            return selectedPaths;
        }

        private static List<string> GetSelectedPathsBasedOnSelectedAsset(Object selectedAsset)
        {
            List<string> result = new List<string>();

            string assetPath = AssetDatabase.GetAssetPath(selectedAsset);
            if (IsSelectedAssetADirectory())
            {
                List<string> subFilesPaths = GetDirectoryFilesPaths();
                result.AddRange(subFilesPaths);
            }
            else
                result.Add(assetPath);

            return result;

            bool IsSelectedAssetADirectory() => Directory.Exists(assetPath);
            List<string> GetDirectoryFilesPaths() => Directory.GetFiles(assetPath, searchPattern: "*.*", SearchOption.AllDirectories).Where(name => !name.EndsWith(".meta")).ToList();
        }
    }
}