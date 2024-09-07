using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;


namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Folder Hider")]
    public class FolderHidingBuildAction : ScriptableBuildAction
    {
        public enum ExecutionAction { Hide, Unhide};

        public ExecutionAction executionAction;
        public List<string> targetPaths;

        public bool refreshAssetDatabase;



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
            if (!DoesDirectoryExistContainingFiles(path))
                return;
            var nextPath = path + "~";

            if (Directory.Exists(nextPath) && DoesDirectoryContainsAnyFileElseThanUnityMetaFiles(nextPath) == false)
                Directory.Delete(nextPath, recursive: true);

            if (!Directory.Exists(nextPath))
                Directory.Move(path, nextPath);
            else
                Debug.LogError($"Tried To Hide {path} but couldn't, because hidden path already exist");
        }

        private void TryUnhide(string path)
        {
            var previousPath = path + "~";
            if (!DoesDirectoryExistContainingFiles(previousPath))
                return;

            if (Directory.Exists(path) && DoesDirectoryContainsAnyFileElseThanUnityMetaFiles(path) == false)
                Directory.Delete(path, recursive: true);

            if (!Directory.Exists(path)) // TODO: This is not needed, C# Move method itself check if same name directory exists it won't move.
                Directory.Move(previousPath, path);
            else
                Debug.LogError($"Tried To UnHide {path}~ but couldn't, because UnHidden path already exist");
        }

        private bool DoesDirectoryExistContainingFiles(string path)
        {
            return Directory.Exists(path) && DoesDirectoryContainsAnyFileElseThanUnityMetaFiles(path);
        }

        private bool DoesDirectoryContainsAnyFileElseThanUnityMetaFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, searchPattern: "*.*", SearchOption.AllDirectories).Any(name => !name.EndsWith(".meta"));
        }
    }
}
