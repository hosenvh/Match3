using System;
using UnityEditor;
using UnityEngine;



namespace Match3.EditorTools.Editor.Menus.User.UserManagement
{
    public class UserIdSelectionEditorWindow : EditorWindow
    {
        private static string customSelectedUserId = "Debug_";
        private static Action<string> onUserIdSelected;

        public static void Open(Action<string> onUserIdSelected)
        {
            UserIdSelectionEditorWindow.onUserIdSelected = onUserIdSelected;
            GetWindow(typeof(UserIdSelectionEditorWindow));
        }

        private void OnGUI()
        {
            DrawUserIdSelectionWindow(onUserIdSetRequest: onUserIdSelected);
        }

        private void DrawUserIdSelectionWindow(Action<string> onUserIdSetRequest)
        {
            DrawUserIdInputField();
            DrawUserIdSetButton(
                onClick: () => onUserIdSetRequest.Invoke(customSelectedUserId));

            void DrawUserIdInputField()
            {
                customSelectedUserId = EditorGUILayout.TextField(label: "Target UserId", text: customSelectedUserId);
            }

            void DrawUserIdSetButton(Action onClick)
            {
                if (GUILayout.Button("Set UserId"))
                    onClick.Invoke();
            }
        }
    }
}