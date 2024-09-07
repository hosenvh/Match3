using Match3.Development.DevOptions;
using Match3.EditorTools.Editor.Menus.User.UserManagement;
using UnityEditor;


namespace Match3.EditorTools.Editor.Menus.User.UserCacheManagement
{
    public class UserCacheManagementEditorOptions
    {
        private const string EditorMenuRootPath = "Golmorad/User/Cache";

        [MenuItem(EditorMenuRootPath + "/Clear All And Convert to a New User", priority = 300)]
        public static void ClearAllAndRandomizeUserId()
        {
            ClearAll(shouldKeepUserId: false);
            ChangeUserIdRandomly();

            void ChangeUserIdRandomly() => UserManagementEditorOptions.RandomizeUserId();
        }

        [MenuItem(EditorMenuRootPath + "/Clear All, But Keep userId", priority = 301)]
        private static void ClearAllKeepingUserId()
        {
            ClearAll(shouldKeepUserId: true);
        }

        [MenuItem(EditorMenuRootPath + "/Clear All", priority = 302)]
        private static void ClearAllNotKeepingUserId()
        {
            ClearAll(shouldKeepUserId: false);
        }

        private static void ClearAll(bool shouldKeepUserId)
        {
            UserCacheManagementDevOptions.ClearAll(shouldKeepUserId);
        }

        [MenuItem(EditorMenuRootPath + "/Clear PlayerPrefs Only", priority = 303)]
        private static void ClearPlayerPrefs()
        {
            UserCacheManagementDevOptions.ClearPlayerPrefs(shouldKeepUserId: false);
        }

        [MenuItem(EditorMenuRootPath + "/Clear Files Only", priority = 304)]
        private static void ClearFiles()
        {
            UserCacheManagementDevOptions.ClearFiles();
        }

        [MenuItem(EditorMenuRootPath + "/Clear EditorPrefs", priority = 304)]
        private static void ClearEditorPrefs()
        {
            EditorPrefs.DeleteAll();
        }

        [MenuItem(EditorMenuRootPath + "/PlayerPrefs Utility Tool", priority = 316)]
        private static void ShowUserCacheUtilityWindow()
        {
            UserCacheUtilityEditorWindow.Open();
        }
    }
}