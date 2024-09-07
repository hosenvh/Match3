using Match3.Data.Map;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Match3.EditorTools.Editor.Menus.Optimization
{
    public partial class GolmoradAssetOptimizationUtilityWindow : EditorWindow
    {
        public delegate bool DisplayProgress(float progress, string message);

        [SerializeField] string targetPath = "Assets";

        [SerializeField] string targetAtlas = "";

        [SerializeField] MapsMetaDatabase mapsMetaDatabase;


        UnusedAtlasedSpriteFinder unusedAtlasedSpriteFinder = new UnusedAtlasedSpriteFinder();
        IncludedTextureInAtlasFinder includedTextureInAtlasFinder = new IncludedTextureInAtlasFinder();
        UnusedMapItemElementFinder unusedMapItemElementFinder = new UnusedMapItemElementFinder();
        MipMapsEnabledTextureFinder mipMapsEnabledTextureFinder = new MipMapsEnabledTextureFinder();

        [MenuItem("Golmorad/Optimization/Optimization Utilities", priority = 200)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(GolmoradAssetOptimizationUtilityWindow));
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(10);
            DrawFindUnusedAtlasedTexturesGUI();
            GUILayout.Space(10);
            DrawFindAtlasTexturesGUI();
            GUILayout.Space(10);
            DrawFindUnusedMapItemsGUI();
            GUILayout.Space(10);
            DrawFindTexturesWithMipMapsGUI();

            GUILayout.EndVertical();
        }

        private void DrawFindUnusedAtlasedTexturesGUI()
        {
            GUILayout.BeginVertical("Find Unused Atlased Sprites", GUI.skin.textArea);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label(targetPath);
            if (GUILayout.Button("Select Path"))
                targetPath = AssetEditorUtilities.RelativeAssetPath(EditorUtility.OpenFolderPanel("Select", targetPath, ""));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Find"))
                Show("Following Textures are unused but atlased.", FindUnusedAtlasedTextures(targetPath));
            GUILayout.EndVertical();
        }

        private void DrawFindAtlasTexturesGUI()
        {
            GUILayout.BeginVertical("Find Sprites in Atlas", GUI.skin.textArea);
            GUILayout.Space(20);
            targetAtlas = EditorGUILayout.TextField("Atlas Tag", targetAtlas);

            if (GUILayout.Button("Find"))
                Show("Following Textures are included in atlas.", FindIncludedTexturesInAtlas(targetAtlas));
            GUILayout.EndVertical();
        }

        private void DrawFindUnusedMapItemsGUI()
        {
            GUILayout.BeginVertical("Find Unused Map Items", GUI.skin.textArea);
            GUILayout.Space(20);

            mapsMetaDatabase = (MapsMetaDatabase)EditorGUILayout.ObjectField("Maps Meta Database", mapsMetaDatabase, typeof(MapsMetaDatabase), allowSceneObjects:false);

            if (GUILayout.Button("Find"))
                Show("Following MapElements are unused", FindUnusedMapItemElements(mapsMetaDatabase));
            GUILayout.EndVertical();
        }

        private void DrawFindTexturesWithMipMapsGUI()
        {
            GUILayout.BeginVertical("Find Textures With Mip Maps", GUI.skin.textArea);
            GUILayout.Space(20);
            if (GUILayout.Button("Find"))
                Show("Following Textures have Mip Maps.", FindTrexturesWithMipMaps());
            GUILayout.EndVertical();
        }

        private void Show<T>(string message, List<T> objects) where T: UnityObject
        {
            Debug.Log("------------------------------------");
            Debug.Log(message);
            Debug.Log("===================================");

            if (objects.Count == 0)
                Debug.Log("No object has been found");
            else
                foreach (var obj in objects)
                    Debug.Log(obj, obj);

            Debug.Log("------------------------------------");
        }

        private List<Texture2D> FindUnusedAtlasedTextures(string path)
        {
            DisplayProgress displayProgressAction = (float progress, string message) => EditorUtility.DisplayCancelableProgressBar("", message, progress);

            var result = unusedAtlasedSpriteFinder.Find(path, displayProgressAction);
            
            EditorUtility.ClearProgressBar();

            return result;
        }

        private List<Texture2D> FindIncludedTexturesInAtlas(string atlas)
        {
            DisplayProgress displayProgressAction = (float progress, string message) => EditorUtility.DisplayCancelableProgressBar("", message, progress);

            var result = includedTextureInAtlasFinder.Find(atlas, displayProgressAction);

            EditorUtility.ClearProgressBar();

            return result;
        }

        private List<MapItem_Element> FindUnusedMapItemElements(MapsMetaDatabase mapsMetaDatabase)
        {
            DisplayProgress displayProgressAction = (float progress, string message) => EditorUtility.DisplayCancelableProgressBar("", message, progress);

            var result = unusedMapItemElementFinder.Find(mapsMetaDatabase, displayProgressAction);

            EditorUtility.ClearProgressBar();

            return result;
        }

        private List<Texture2D> FindTrexturesWithMipMaps()
        {
            DisplayProgress displayProgressAction = (float progress, string message) => EditorUtility.DisplayCancelableProgressBar("", message, progress);

            var result = mipMapsEnabledTextureFinder.Find(displayProgressAction);

            EditorUtility.ClearProgressBar();

            return result;
        }

    }
}