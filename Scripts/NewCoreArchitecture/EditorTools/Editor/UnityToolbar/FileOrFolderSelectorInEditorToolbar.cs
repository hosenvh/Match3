using System;
using System.Reflection;
using Match3.EditorTools.Editor.UnityToolbar.Base;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using Object = UnityEngine.Object;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public class FolderSelectorRightButton
    {
        private const string MAP_PREFABS_FOLDER_PATH = "Assets/Resources/Prefabs/UI/";
        private const string VILLA_MAP_PREFAB_PATH = MAP_PREFABS_FOLDER_PATH + "State_Map_GolmoradVilla.prefab";
        private const string MANSION_MAP_PREFAB_PATH = MAP_PREFABS_FOLDER_PATH + "State_Map_GolmoradMansion.prefab";

        static FolderSelectorRightButton()
        {
            ToolbarExtender.AddDrawerToLeftToolbar(priority: 1, DrawToolbarGUI);
        }

        private static void DrawToolbarGUI()
        {
            if (GUILayout.Button(new GUIContent("Villa", "Open Villa Map Prefab"), EditorToolbarStyles.GetCommandButtonStyle(widthFactor: 0.8f)))
                HandleMapOpeningButtonPressed(VILLA_MAP_PREFAB_PATH);

            if (GUILayout.Button(new GUIContent("Mansion", "Open Mansion Map Prefab"), EditorToolbarStyles.GetCommandButtonStyle(widthFactor: 0.8f)))
                HandleMapOpeningButtonPressed(MANSION_MAP_PREFAB_PATH);

            void HandleMapOpeningButtonPressed(string targetMapPrefabPath)
            {
                PrefabSelectorHelper.OpenPrefab(targetMapPrefabPath);
                FileOrFolderSelectorHelper.SelectInProjectWindow(targetMapPrefabPath);
            }
        }
    }

    internal static class PrefabSelectorHelper
    {
        public static void OpenPrefab(string path)
        {
            int instanceID = PathHelper.GetInstanceIdOf(path);
            AssetDatabase.OpenAsset(instanceID);
            SceneView.FrameLastActiveSceneView();
        }

    }

    internal static class FileOrFolderSelectorHelper
    {
        public static void SelectInProjectWindow(string path)
        {
            int instanceID = PathHelper.GetInstanceIdOf(path);
            OpenProjectWindowIfNeeded();
            Selection.activeInstanceID = instanceID;
        }

        private static void OpenProjectWindowIfNeeded()
        {
            Assembly editorAssembly = typeof(UnityEditor.Editor).Assembly;
            Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");
            Object[] projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);

            if (projectBrowserInstances.Length == 0)
                OpenNewProjectBrowser(projectBrowserType);
        }

        private static void OpenNewProjectBrowser(Type projectBrowserType)
        {
            EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);
            projectBrowser.Show();

            MethodInfo init = projectBrowserType.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
            init.Invoke(projectBrowser, null);

            projectBrowser.GetType().GetMethod("SetOneColumn", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(projectBrowser, null);
        }
    }

    internal static class PathHelper
    {
        public static int GetInstanceIdOf(string path)
        {
            var getInstanceIDMethod = typeof(AssetDatabase).GetMethod("GetMainAssetInstanceID", BindingFlags.Static | BindingFlags.NonPublic);
            return (int) getInstanceIDMethod.Invoke(null, new object[] {path});
        }
    }
}