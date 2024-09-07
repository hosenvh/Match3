using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public static class MainScenePlayerInEditorToolbar
    {
        static MainScenePlayerInEditorToolbar()
        {

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            ToolbarExtender.AddDrawerToCenterToolbar(priority: 1, DrawToolBarGui);
        }

        private static void DrawToolBarGui()
        {
            var tex = EditorGUIUtility.IconContent(@"Animation.LastKey").image;

            if (EditorApplication.isPlaying == false && GUILayout.Button(new GUIContent(null, tex, ""), "Command"))
            {
                var lastScenes = new List<string>();
                for (int i = 0; i < SceneManager.sceneCount; ++i)
                    lastScenes.Add(SceneManager.GetSceneAt(i).path);
                SaveLastScenes(lastScenes);

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);
                    EditorApplication.isPlaying = true;
                }
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var lastScenes = LoadLastScenes();

                foreach (var scene in lastScenes)
                    EditorSceneManager.OpenScene(scene, OpenSceneMode.Additive);

                EditorSceneManager.CloseScene(SceneManager.GetSceneAt(0), true);
                lastScenes.Clear();
                SaveLastScenes(lastScenes);
            }
        }


        private static List<string> LoadLastScenes()
        {
            if (EditorPrefs.HasKey("LAST_SCENES") == false)
                return new List<string>();

            return JsonUtility.FromJson<SerializableStringList>(EditorPrefs.GetString("LAST_SCENES")).data;

        }

        private static void SaveLastScenes(List<string> lastScenes)
        {

            EditorPrefs.SetString("LAST_SCENES", JsonUtility.ToJson(new SerializableStringList(lastScenes)));
        }

        [System.Serializable]
        class SerializableStringList
        {
            public List<string> data;

            public SerializableStringList(List<string> data)
            {
                this.data = data;
            }
        }
    }
}