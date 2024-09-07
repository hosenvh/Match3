using System;
using Match3.Utility.GolmoradLogging.Config;
using UnityEditor;
using UnityEngine;


namespace Match3.Utility.GolmoradLogging.Editor
{
    public class GolmoradLoggerConfigEditor : EditorWindow
    {
        private static string configPath = "Assets/Configs";
        private static GolmoradLogsActivenessConfig config;

        [MenuItem(itemName: "Golmorad/Other/Logs Activeness")]
        private static void ShowWindow()
        {
            config = AssetEditorUtilities.FindAssetsByType<GolmoradLogsActivenessConfig>(path: configPath)[index: 0];
            config.UpdateConfig();


            GetWindow<GolmoradLoggerConfigEditor>(title: "LogsActiveness");
        }

        private void OnGUI()
        {
            DrawConfigFolderSelector();

            // EditorGUILayout.LabelField(label: "---------------------------------Options--------------------------------", style: centeredStyle);

            InsertLogTagsConfigs();
            InsertToggleButtons();

            EditorUtility.SetDirty(target: config);
        }

        private void DrawConfigFolderSelector()
        {
            GUILayout.Label(text: "Logs Config Folder Path: " + configPath);
            if (GUILayout.Button(text: "Select Logs Config folder"))
                configPath = AssetEditorUtilities.RelativeAssetPath(absolutePath: EditorUtility.OpenFolderPanel(title: "Select", folder: configPath, defaultName: ""));
        }

        private void InsertLogTagsConfigs()
        {
            EditorGUILayout.BeginVertical();

            foreach (LogTagConfig logTagConfig in config.LogTagsConfigs)
            {
                EditorGUILayout.BeginHorizontal(style: new GUIStyle(other: GUI.skin.box));

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(text: logTagConfig.Type.Name + ": ");
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(pixels: 20);

                logTagConfig.LogTitle = GUILayout.TextField(logTagConfig.LogTitle);
                logTagConfig.ShouldSendInfoLog = GUILayout.Toggle(value: logTagConfig.ShouldSendInfoLog, text: "Info Log");
                logTagConfig.ShouldSendWarningLog = GUILayout.Toggle(value: logTagConfig.ShouldSendWarningLog, text: "Warning Log");
                logTagConfig.ShouldSendErrorLog = GUILayout.Toggle(value: logTagConfig.ShouldSendErrorLog, text: "Error Log");

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void InsertToggleButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(text: "Activate All"))
                SetAllAs(shouldBeActive: true);
            if (GUILayout.Button(text: "Deactivated All"))
                SetAllAs(shouldBeActive: false);

            EditorGUILayout.EndHorizontal();

            void SetAllAs(bool shouldBeActive)
            {
                foreach (LogTagConfig logger in config.LogTagsConfigs)
                {
                    logger.ShouldSendInfoLog = shouldBeActive;
                    logger.ShouldSendWarningLog = shouldBeActive;
                    logger.ShouldSendErrorLog = shouldBeActive;
                }
            }
        }
    }
}