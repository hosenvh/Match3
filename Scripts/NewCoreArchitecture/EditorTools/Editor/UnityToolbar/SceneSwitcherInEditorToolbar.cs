using Match3.EditorTools.Editor.UnityToolbar.Base;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        static SceneSwitchLeftButton()
        {
            ToolbarExtender.AddDrawerToLeftToolbar(priority: 3, DrawToolbarGUI);
        }

        private static void DrawToolbarGUI()
        {
            if (GUILayout.Button(new GUIContent(text: "Open Scene"), style: EditorToolbarStyles.GetCommandButtonStyle()))
                DrawSceneSelectionMenu();
        }

        private static void DrawSceneSelectionMenu()
        {
            GenericMenu menu = new GenericMenu();

            AddMenuItem(menuPath: "Main Scene", onSelected: () => OpenScene(sceneName: "Main.unity"));
            DrawSeparator();
            AddMenuItem(menuPath: "Game Scene", onSelected: () => OpenScene(sceneName: "Game.unity"));
            DrawSeparator();
            AddMenuItem(menuPath: "UI Scene", onSelected: () => OpenScene(sceneName: "UIPrefabEditingScene.unity"));
            DrawSeparator();
            AddMenuItem(menuPath: "Loading Scene", onSelected: () => OpenScene(sceneName: "LoadingScene.unity"));

            menu.ShowAsContext();

            void DrawSeparator() => menu.AddSeparator("");

            void AddMenuItem(string menuPath, GenericMenu.MenuFunction onSelected)
            {
                menu.AddItem(new GUIContent(menuPath), on: false, onSelected);
            }

            void OpenScene(string sceneName)
            {
                SceneHelper.StartScene($"Assets/Scenes/{sceneName}");
            }
        }
    }

    static class SceneHelper
    {
        static string scenePathToOpen;

        public static void StartScene(string scenePath)
        {
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;

            scenePathToOpen = scenePath;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (scenePathToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePathToOpen);
            scenePathToOpen = null;
        }
    }
}