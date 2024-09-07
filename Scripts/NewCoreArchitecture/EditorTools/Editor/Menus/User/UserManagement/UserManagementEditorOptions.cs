using System;
using Match3.Development.DevOptions;
using UnityEditor;
using ServerDevOptions = Match3.ServerCommunication.Development.ServerDevOptions;


namespace Match3.EditorTools.Editor.Menus.User.UserManagement
{
    public class UserManagementEditorOptions
    {
        private const string EditorMenuRootPath = "Golmorad/User/UserId";

        [MenuItem(EditorMenuRootPath + "/Log UserIds")]
        private static void LogUserIds()
        {
            UserManagementDevOptions.LogUserIds();
        }

        [MenuItem(EditorMenuRootPath + "/Randomize UserId (Convert to a New User)")]
        public static void RandomizeUserId()
        {
            UserManagementDevOptions.ChangeUserIdRandomly();
        }

        [MenuItem(EditorMenuRootPath + "/Reset UserId To Original")]
        private static void ResetUserIdToOriginal()
        {
            UserManagementDevOptions.ResetUserIdToOriginal();
        }

        [MenuItem(EditorMenuRootPath + "/Set Custom UserId")]
        private static void SetCustomUserId()
        {
            OpenUserIdSelectionWindow(onUserIdSelected: SetUserId);

            void OpenUserIdSelectionWindow(Action<string> onUserIdSelected) => UserIdSelectionEditorWindow.Open(onUserIdSelected);
            void SetUserId(string userId) => UserManagementDevOptions.SetUserId(userId);
        }

        [MenuItem(EditorMenuRootPath + "/Clear User Signin info")]
        private static void ClearUserSigninInfo()
        {
            ServerDevOptions.ClearSigninInfo();
        }

        [MenuItem(EditorMenuRootPath + "/Signin")]
        public static void Signin()
        {
            OpenUserSigninInfoSelectionWindow(onUserSigninInfoEntered: ServerDevOptions.Signin);

            void OpenUserSigninInfoSelectionWindow(Action<string, string> onUserSigninInfoEntered)
            {
                UserSignInInfoSelectionEditorWindow.Open(onUserSigninInfoEntered);
            }
        }
    }
}