using System;
using Match3.EditorTools.Editor.UnityToolbar.Base;
using Match3.EditorTools.Editor.Utility;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public class MakeReadyToCommitEditorToolbar
    {
        static MakeReadyToCommitEditorToolbar()
        {
            ToolbarExtender.AddDrawerToLeftToolbar(priority: 5, DrawToolbarGUI);
        }

        private static void DrawToolbarGUI()
        {
            if (IsNotSafeToExecute())
                DisableGUI();
            InsertCommitReadyButton(onClick: MakerReadyForCommitting);
            ReEnableGUI();

            void InsertCommitReadyButton(Action onClick)
            {
                EditorUtilities.InsertButton(
                    title: "Commit Ready",
                    style: EditorToolbarStyles.GetCommandButtonStyle(widthFactor: 1.2f),
                    onClick,
                    tooltip: "Removing Empty Folders + Project Saving");
            }

            void DisableGUI() => GUI.enabled = false;
            void ReEnableGUI() => GUI.enabled = true;
        }

        private static bool IsNotSafeToExecute() => Application.isPlaying;

        public static void MakerReadyForCommitting()
        {
            if (IsNotSafeToExecute())
                LogErrorGettingReadyIsNotSafe();
            RemoveEmptyFolders();
            SaveUnityProject();

            void LogErrorGettingReadyIsNotSafe() => Debug.LogError(message: "Ohoooy, Exit Play mode, otherwise it won't be safe to save");
            void RemoveEmptyFolders() => EmptyFoldersRemover.CleanUpWholeProject();

            void SaveUnityProject()
            {
                AssetDatabase.SaveAssets();
                #pragma warning disable 0618
                EditorApplication.SaveScene();
                #pragma warning restore 0618
            }
        }
    }
}