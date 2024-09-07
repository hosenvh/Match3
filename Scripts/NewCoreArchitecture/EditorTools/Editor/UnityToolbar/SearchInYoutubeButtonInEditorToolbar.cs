using Match3.EditorTools.Editor.UnityToolbar.Base;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public class SearchInYoutubeButtonInEditorToolbar
    {
        private const string SEARCH_ADDRESS = "https://www.google.com/search?q=gardenscapes+";
        private const string INPUT_FIELD_NAME = "INPUT_FIELD";

        private static string levelIndex;

        static SearchInYoutubeButtonInEditorToolbar()
        {
            ToolbarExtender.AddDrawerToRightToolbar(priority: 5, DrawToolbarGUI);
        }

        private static void DrawToolbarGUI()
        {
            DrawTextField();
            if (IsEnterDetected())
            {
                LoseFocus();
                SearchInYoutube();
            }
        }

        private static void DrawTextField()
        {
            GUI.SetNextControlName(INPUT_FIELD_NAME);
            levelIndex = GUILayout.TextField(levelIndex, EditorToolbarStyles.GetTextAreaStyle(widthFactor: 0.85f));
        }

        private static bool IsEnterDetected()
        {
            return Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == INPUT_FIELD_NAME;
        }

        private static void LoseFocus()
        {
            GUI.FocusControl("N/A");
        }

        private static void SearchInYoutube()
        {
            Application.OpenURL(SEARCH_ADDRESS + levelIndex);
        }
    }
}