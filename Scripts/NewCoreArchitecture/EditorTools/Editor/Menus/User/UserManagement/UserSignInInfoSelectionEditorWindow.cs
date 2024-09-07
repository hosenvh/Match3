using System;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Menus.User.UserManagement
{
    public class UserSignInInfoSelectionEditorWindow : EditorWindow
    {
        private static string customSelectedUsername;
        private static string customSelectedPassword;
        private static Action<string, string> onUserSigninInfoEntered;

        public static void Open(Action<string, string> onUserSigninInfoEntered)
        {
            UserSignInInfoSelectionEditorWindow.onUserSigninInfoEntered = onUserSigninInfoEntered;
            GetWindow(typeof(UserSignInInfoSelectionEditorWindow));
        }

        private void OnGUI()
        {
            DrawUserIdSelectionWindow(onUserSigninInfoEntered);
        }

        private void DrawUserIdSelectionWindow(Action<string, string> onUserSigninInfoEntered)
        {
            DrawUserSigninInputFields();
            DrawUserSigninInfoSetButton(
                onClick: () => onUserSigninInfoEntered.Invoke(customSelectedUsername, customSelectedPassword));

            void DrawUserSigninInputFields()
            {
                customSelectedUsername = EditorGUILayout.TextField(label: "Target Username", text: customSelectedUsername);
                customSelectedPassword = EditorGUILayout.TextField(label: "Target Password", text: customSelectedPassword);
            }

            void DrawUserSigninInfoSetButton(Action onClick)
            {
                if (GUILayout.Button("Signin"))
                    onClick.Invoke();
            }
        }
    }
}