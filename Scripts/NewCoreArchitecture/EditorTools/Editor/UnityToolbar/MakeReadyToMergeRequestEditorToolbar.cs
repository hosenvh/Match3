using System;
using Match3.EditorTools.Editor.Menus.Utility;
using Match3.EditorTools.Editor.UnityToolbar.Base;
using Match3.EditorTools.Editor.Utility;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public static class MakeReadyToMergeRequestEditorToolbar
    {
        static MakeReadyToMergeRequestEditorToolbar()
        {
            ToolbarExtender.AddDrawerToLeftToolbar(priority: 6, DrawToolbarGUI);
        }

        private static void DrawToolbarGUI()
        {
            if (IsNotSafeToExecute())
                DisableGUI();
            InsertMergeRequestReadyButton(onClick: MakerReadyForMergeRequesting);
            ReEnableGUI();

            void InsertMergeRequestReadyButton(Action onClick)
            {
                EditorUtilities.InsertButton(
                    title: "MR Ready",
                    style: EditorToolbarStyles.GetCommandButtonStyle(),
                    onClick,
                    tooltip: "Serialize Whole Project + Commit Ready");
            }
            void DisableGUI() => GUI.enabled = false;
            void ReEnableGUI() => GUI.enabled = true;
        }

        private static bool IsNotSafeToExecute()
        {
            return Application.isPlaying;
        }

        private static void MakerReadyForMergeRequesting()
        {
            if (IsNotSafeToExecute())
                LogErrorGettingReadyIsNotSafe();
            ReSerializer.ReserializeWholeProject();
            MakeReadyToCommitEditorToolbar.MakerReadyForCommitting();

            void LogErrorGettingReadyIsNotSafe() => Debug.LogError(message: "Ohoooy, Exit Play mode, otherwise it won't be safe to Serialize Project");
        }
    }
}