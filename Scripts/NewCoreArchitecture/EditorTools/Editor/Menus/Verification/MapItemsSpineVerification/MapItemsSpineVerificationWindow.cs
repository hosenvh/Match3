using Match3.EditorTools.Editor.Utility;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Menus.Verification.MapItemsSpineVerification
{
    public class MapItemsSpineVerificationWindow : EditorWindow
    {
        private static readonly Vector2 MIN_WINDOW_SIZE = new Vector2(600, 200);
        private static readonly Vector2 MAX_WINDOW_SIZE = new Vector2(700, 1000);

        private static readonly string ASSETS_DEFAULT_PATH = "Assets/Assets/Sprites/Map Items/Spine";
        private static string path;

        [MenuItem("Golmorad/Verification/Map Items Spine Verifier", priority = 101)]
        public static void OpenWindow()
        {
            var window = OpenUnityWindow();
            SetupWindowSize();

            path = ASSETS_DEFAULT_PATH;

            MapItemsSpineVerificationWindow OpenUnityWindow() => GetWindow<MapItemsSpineVerificationWindow>(title: "Map Items Spine Verifier");

            void SetupWindowSize()
            {
                window.minSize = MIN_WINDOW_SIZE;
                window.maxSize = MAX_WINDOW_SIZE;
            }
        }

        private void OnGUI()
        {
            EditorUtilities.InsertTextField(label: "Path to verify", text: ref path);

            if (GUILayout.Button("Verify Map Items"))
            {
                MapItemsSpineVerifier mapItemsSpineVerifier = new MapItemsSpineVerifier();
                mapItemsSpineVerifier.Verify(path);
            }
        }
    }
}
