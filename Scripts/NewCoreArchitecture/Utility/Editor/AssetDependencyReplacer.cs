using UnityEngine;
using UnityEditor;
using System.IO;

namespace Match3.Utility.Editor
{
    public static class AssetDependencyReplacer 
    {
        // NOTE: It is assumed Assets are stored as Text.
        // WARRNING: This is not stable.
        // fileID is based on usage of dependency object (type).  
        public static void Replace(UnityEngine.Object target, UnityEngine.Object oldDependency, UnityEngine.Object newDependency, long fileID)
        {
            var targetPath = AssetDatabase.GetAssetPath(target);

            var oldDependencyData = $"fileID: {fileID}, guid: {GUID(oldDependency)}";
            var newDependecnyData = $"fileID: {fileID}, guid: {GUID(newDependency)}";

            var fileText = File.ReadAllText(targetPath);

            fileText = fileText.Replace(oldDependencyData, newDependecnyData);

            File.WriteAllText(targetPath, fileText);
        }


        private static string GUID(UnityEngine.Object obj)
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
        }
    }
}