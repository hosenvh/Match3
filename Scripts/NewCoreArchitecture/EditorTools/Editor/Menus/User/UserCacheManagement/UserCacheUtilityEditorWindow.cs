using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Menus.User.UserCacheManagement
{
    public class UserCacheUtilityEditorWindow : EditorWindow
    {
        private string selectedPlayerPrefsKey;

        private string[] valueTypes = {"int", "float", "string"};
        private int selectedValueTypeIndex = -1;
        private string targetValue;

        private string SelectedValueType => valueTypes[selectedValueTypeIndex];

        public static void Open()
        {
            GetWindow(typeof(UserCacheUtilityEditorWindow));
        }

        private void OnGUI()
        {
            DrawKeyInputField();
            DrawKeyTypeSelectionField();

            if (CanProceed() == false)
                return;

            DrawEmptySpace();

            DrawClearKeyButton(onClick: ClearSelectedKeyFromPlayerPrefs);
            DrawCheckKeyExistenceButton(onClick: LogSelectedKeyExistence);
            DrawGetValueButton(onClick: LogSelectedKeyValue);

            DrawEmptySpace();

            DrawSetToTargetValueField(onSetRequest: SetKeyToTargetValue);

            void DrawEmptySpace() => GUILayout.Space(pixels: 20);
            void DrawKeyInputField() => selectedPlayerPrefsKey = EditorGUILayout.TextField(label: "PlayerPrefs Key", text: selectedPlayerPrefsKey);
            bool CanProceed() => selectedPlayerPrefsKey.IsNullOrEmpty() == false && selectedValueTypeIndex != -1;
            void DrawKeyTypeSelectionField() => selectedValueTypeIndex = EditorGUILayout.IntPopup(label: "Value Type", selectedValue: selectedValueTypeIndex, displayedOptions: valueTypes, optionValues: new[] {0, 1, 2});

            void DrawClearKeyButton(Action onClick) => DrawButton(title: "Clear From PlayerPrefs", onClick);
            void DrawCheckKeyExistenceButton(Action onClick) => DrawButton(title: "Check Existence In PlayerPrefs", onClick);
            void DrawGetValueButton(Action onClick) => DrawButton(title: "Get Value From PlayerPrefs", onClick);

            void DrawSetToTargetValueField(Action onSetRequest)
            {
                GUILayout.BeginHorizontal();
                targetValue = EditorGUILayout.TextField("Target Value:", targetValue);
                DrawButton(title: "Set Value", onClick: onSetRequest);
                GUILayout.EndHorizontal();
            }

            void DrawButton(string title, Action onClick)
            {
                if (GUILayout.Button(title))
                    onClick.Invoke();
            }
        }

        private void ClearSelectedKeyFromPlayerPrefs()
        {
            PlayerPrefs.DeleteKey(selectedPlayerPrefsKey);
        }

        private void LogSelectedKeyExistence()
        {
            Debug.Log(message: $"PlayerPrefs Key {selectedPlayerPrefsKey} Existence: {PlayerPrefs.HasKey(key: selectedPlayerPrefsKey)}");
        }

        private void LogSelectedKeyValue()
        {
            if (PlayerPrefs.HasKey(selectedPlayerPrefsKey) == false)
            {
                LogSelectedKeyExistence();
                return;
            }

            Debug.Log(message: $"PlayerPrefs Key {selectedPlayerPrefsKey} Value is: {FetchValue()}");

            string FetchValue()
            {
                switch (SelectedValueType)
                {
                    case "int":
                        return PlayerPrefs.GetInt(selectedPlayerPrefsKey, defaultValue: 0).ToString();
                    case "float":
                        return PlayerPrefs.GetFloat(selectedPlayerPrefsKey, defaultValue: 0f).ToString(provider: CultureInfo.InvariantCulture);
                    case "string":
                        return PlayerPrefs.GetString(selectedPlayerPrefsKey, defaultValue: "");
                }
                return "Null";
            }
        }

        private void SetKeyToTargetValue()
        {
            switch (SelectedValueType)
            {
                case "int":
                    bool canParseToInt = int.TryParse(targetValue, out int targetIntValue);
                    if (canParseToInt)
                        PlayerPrefs.SetInt(selectedPlayerPrefsKey, targetIntValue);
                    else
                        Debug.LogError("Cant Parse Entered Target Value to Int");
                    break;
                case "float":
                    bool canParseToFloat = float.TryParse(targetValue, out float targetFloatValue);
                    if (canParseToFloat)
                        PlayerPrefs.SetFloat(selectedPlayerPrefsKey, targetFloatValue);
                    else
                        Debug.LogError("Cant Parse Entered Target Value to Int");
                    break;
                case "string":
                    PlayerPrefs.SetString(selectedPlayerPrefsKey, targetValue);
                    break;
            }
        }
    }
}