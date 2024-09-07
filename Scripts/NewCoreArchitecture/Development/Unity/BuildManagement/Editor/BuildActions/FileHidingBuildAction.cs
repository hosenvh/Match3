using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/File Hider")]
    public class FileHidingBuildAction : ScriptableBuildAction
    {
        public enum ExecutionAction { Hide, Unhide};

        public bool refreshAssetDatabase;
        public ExecutionAction executionAction;
        public List<string> targetPaths;

        
        [ContextMenu("Execute")]
        public override void Execute()
        {
            switch (executionAction)
            {
                case ExecutionAction.Hide:
                    TryHideAll();
                    break;
                case ExecutionAction.Unhide:
                    TryUnhideAll();
                    break;
            }

            if (refreshAssetDatabase)
                AssetDatabase.Refresh();
        }

        [ContextMenu("Revert")]
        public override void Revert()
        {
            switch (executionAction)
            {
                case ExecutionAction.Hide:
                    TryUnhideAll();
                    break;
                case ExecutionAction.Unhide:
                    TryHideAll();
                    break;
            }

            if (refreshAssetDatabase)
                AssetDatabase.Refresh();
        }
        
        
        private void TryUnhideAll()
        {
            foreach (var path in targetPaths)
                TryUnhide(path);
        }

        
        private void TryHideAll()
        {
            foreach (var path in targetPaths)
                TryHide(path);
        }
        
        

        private void TryHide(string path)
        {
            var targetFileInfo = new FileInfo(path);
            var newPath = Path.Combine(targetFileInfo.Directory.FullName, "." + targetFileInfo.Name);
            if(!File.Exists(newPath) && File.Exists(path))
                File.Move(path, newPath);
        }

        private void TryUnhide(string path)
        {
            var targetFileInfo = new FileInfo(path);
            var currentPath = Path.Combine(targetFileInfo.Directory.FullName, "." + targetFileInfo.Name);
            
            if (!File.Exists(path) && File.Exists(currentPath))
                File.Move(currentPath, path);
        }
        
        
    }
}
