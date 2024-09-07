using System;
using Match3.EditorTools.Editor.Drawers;
using Match3.Foundation.Base.DataStructures;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Utility
{
    public static class EditorUtilities
    {
        private static readonly GUIStyle DefaultToggleStyle;
        private static readonly GUIStyle DefaultLabelStyle;
        private static readonly GUIStyle DefaultButtonStyle;
        private static float bufferedLabelWidth;

        static EditorUtilities()
        {
            DefaultToggleStyle = new GUIStyle(other: EditorStyles.toggle);
            DefaultLabelStyle = new GUIStyle(other: EditorStyles.label);
            DefaultButtonStyle = new GUIStyle(other: EditorStyles.toolbarButton);
        }

        public static void InsertToggle(ref bool isEnable, string title, params GUILayoutOption[] guiLayoutOption)
        {
            InsertToggle(ref isEnable, title, DefaultToggleStyle, guiLayoutOption);
        }

        public static void InsertToggle(ref bool isEnable, string title, GUIStyle toggleStyle, params GUILayoutOption[] guiLayoutOption)
        {
            if (GUILayout.Toggle(value: isEnable, text: title, toggleStyle, guiLayoutOption))
                isEnable = true;
            else
                isEnable = false;
        }

        public static void InsertToggle(Func<bool> enablenessEvaluator, string title, Action onEnable, Action onDisable, params GUILayoutOption[] guiLayoutOption)
        {
            InsertToggle(enablenessEvaluator, title, onEnable, onDisable, DefaultToggleStyle, guiLayoutOption);
        }

        public static void InsertToggle(Func<bool> enablenessEvaluator, string title, Action onEnable, Action onDisable, GUIStyle toggleStyle, params GUILayoutOption[] guiLayoutOption)
        {
            if (GUILayout.Toggle(value: enablenessEvaluator.Invoke(), text: title, toggleStyle, guiLayoutOption))
                onEnable.Invoke();
            else
                onDisable.Invoke();
        }

        public static void ShowConfirmationPopup(string title, string message, Action onConfirm, Action onReject, string confirmText = "Yes", string rejectText = "No")
        {
            bool confirmed = EditorUtility.DisplayDialog(title, message, ok: confirmText, cancel: rejectText);
            if (confirmed)
                onConfirm.Invoke();
            else
                onReject.Invoke();
        }

        public static void InsertButton(string title, Action onClick, string tooltip = "")
        {
            InsertButton(title, DefaultButtonStyle, onClick, tooltip);
        }

        public static void InsertButton(string title, GUIStyle style, Action onClick, string tooltip = "")
        {
            InsertButton(title, icon: null, style, onClick, tooltip);
        }

        public static void InsertButton(string title, Texture icon, GUIStyle style, Action onClick, string tooltip = "")
        {
            if (GUILayout.Button(new GUIContent(title, icon, tooltip), style))
                onClick.Invoke();
        }

        public static void InsertLabel(string text, params GUILayoutOption[] guiLayoutOption)
        {
            InsertLabel(text, DefaultLabelStyle, guiLayoutOption);
        }

        public static void InsertLabel(string text, GUIStyle labelStyle, params GUILayoutOption[] guiLayoutOption)
        {
            GUILayout.Label(text, labelStyle, guiLayoutOption);
        }

        public static void InsertEmptySpace(int pixels)
        {
            GUILayout.Space(pixels);
        }

        public static Rect DrawInHorizontallyArea(Action toDraw, GUIStyle style = null, params GUILayoutOption[] options)
        {
            Rect rect;
            if (style == null)
                rect = EditorGUILayout.BeginHorizontal(options);
            else
                rect = EditorGUILayout.BeginHorizontal(style, options);
            toDraw.Invoke();
            GUILayout.EndHorizontal();
            return rect;
        }

        public static Rect DrawInVerticallyArea(Action toDraw, GUIStyle style = null, params GUILayoutOption[] options)
        {
            Rect rect;
            if (style == null)
                rect = EditorGUILayout.BeginVertical(options);
            else
                rect = EditorGUILayout.BeginHorizontal(style, options);
            toDraw.Invoke();
            GUILayout.EndVertical();
            return rect;
        }

        public static void InsertIntBoundField(ref Bound<int> bound, string label)
        {
            InsertIntField(ref bound.min, label: $"Min {label}:");
            InsertIntField(ref bound.max, label: $"Max {label}:");
        }

        public static void InsertIntField(ref int value, string label, float labelsWidth = 100)
        {
            ChangeLabelWidthTemporary(labelsWidth);
            value = EditorGUILayout.IntField(label, value, GUILayout.ExpandWidth(false));
            RestoreLabelWidth();
        }

        public static void InsertTextField(ref string text, string label, float labelsWidth = 100)
        {
            ChangeLabelWidthTemporary(labelsWidth);
            text = EditorGUILayout.TextField(label, text, GUILayout.ExpandWidth(true));
            RestoreLabelWidth();
        }

        public static void InsertEnumDropDown<T>(ref T value, string label, float labelsWidth = 100, params GUILayoutOption[] options) where T : Enum
        {
            ChangeLabelWidthTemporary(labelsWidth);
            value = (T) EditorGUILayout.EnumPopup(label, value);
            RestoreLabelWidth();
        }

        public static void InsertTypeSelectionDropDown(ref Type currentValue, string label, TypeDropdownDrawer typeDropdownDrawer, float labelsWidth = 100, float labelHeight = 20, params GUILayoutOption[] options)
        {
            ChangeLabelWidthTemporary(labelsWidth);

            var rect = GetCurrentGuiRect();
            currentValue = typeDropdownDrawer.Draw(rect, label, currentValue);

            RestoreLabelWidth();

            Rect GetCurrentGuiRect() => EditorGUILayout.GetControlRect(hasLabel: true, labelHeight);
        }

        private static void ChangeLabelWidthTemporary(float targetLabelWidth)
        {
            bufferedLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = targetLabelWidth;
        }

        private static void RestoreLabelWidth()
        {
            EditorGUIUtility.labelWidth = bufferedLabelWidth;
        }
    }
}