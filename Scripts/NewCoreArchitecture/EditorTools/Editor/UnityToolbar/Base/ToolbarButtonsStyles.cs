using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.UnityToolbar.Base
{
    internal static class EditorToolbarStyles
    {
        private static float ButtonBaseFontSize => Mathf.Min(0.005f * ScreenWidth, 10);
        private static float ButtonBaseWidth => 0.035f * ScreenWidth;
        private static float ScreenWidth => Screen.width / EditorGUIUtility.pixelsPerPoint;

        public static GUIStyle GetCommandButtonStyle(float widthFactor = 1, float fontSizeFactor = 1)
        {
            return new GUIStyle("Command")
                   {
                       alignment = TextAnchor.MiddleCenter, imagePosition = ImagePosition.ImageAbove,
                       fontStyle = FontStyle.Bold,
                       fixedWidth = ButtonBaseWidth * widthFactor,
                       fontSize = Mathf.RoundToInt(ButtonBaseFontSize * fontSizeFactor)
                   };
        }

        public static GUIStyle GetTextAreaStyle(float widthFactor = 1, float fontSizeFactor = 1)
        {
            return new GUIStyle(GUI.skin.textField)
                   {
                       alignment = TextAnchor.MiddleCenter, imagePosition = ImagePosition.TextOnly,
                       fontStyle = FontStyle.Normal,
                       fixedHeight = 21,
                       fixedWidth = ButtonBaseWidth * widthFactor,
                       fontSize = Mathf.RoundToInt(ButtonBaseFontSize * fontSizeFactor)
                   };
        }

    }
}