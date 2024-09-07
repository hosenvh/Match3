using Match3.EditorTools.Editor.Menus.User.UserCacheManagement;
using Match3.EditorTools.Editor.Menus.User.UserManagement;
using Match3.EditorTools.Editor.UnityToolbar.Base;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;


namespace Match3.EditorTools.Editor.UnityToolbar
{
    [InitializeOnLoad]
    public class UserManagementButtonInEditorToolbar
    {
        static UserManagementButtonInEditorToolbar()
        {
            ToolbarExtender.AddDrawerToRightToolbar(priority: 4, DrawToolbarGUI);
        }

        private static void DrawToolbarGUI()
        {
            if (GUILayout.Button(new GUIContent("Signin", "Set User Signin Info"), EditorToolbarStyles.GetCommandButtonStyle(widthFactor: 0.7f)))
                UserManagementEditorOptions.Signin();

            if (GUILayout.Button(new GUIContent("New User", "Convert to a new user"), EditorToolbarStyles.GetCommandButtonStyle()))
                UserManagementEditorOptions.RandomizeUserId();

            if (GUILayout.Button(new GUIContent("Clear Data", "Clear Data And Convert to a New User"), EditorToolbarStyles.GetCommandButtonStyle()))
                UserCacheManagementEditorOptions.ClearAllAndRandomizeUserId();
        }
    }
}