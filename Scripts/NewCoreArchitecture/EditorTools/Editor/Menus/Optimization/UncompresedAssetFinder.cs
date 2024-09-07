using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.EditorTools.Editor.Menus.Optimization
{
    public class AssetAuditingWindow : EditorWindow
    {
        string path = "Assets/";

        [MenuItem("Golmorad/Optimization/Asset Auditing Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(AssetAuditingWindow));
        }

        public AssetAuditingWindow()
        {
            path = "Assets";
        }

        void OnGUI()
        {
            GUILayout.Label(path);
            if (GUILayout.Button("SelectPath"))
                path = AssetEditorUtilities.RelativeAssetPath(EditorUtility.OpenFolderPanel("Select", path, ""));

            if (GUILayout.Button("Find uncompressed texture"))
                Find();

            if (GUILayout.Button("Unatlasify textures in prefabs"))
                UnatlasifyTexturesInPrefab();

            if (GUILayout.Button("!!!Compress Textures!!!"))
                Compress();
        }

        void UnatlasifyTexturesInPrefab()
        {
            var prefabGUID = AssetDatabase.FindAssets("t:Prefab", new string[] { path });

            foreach (var guid in prefabGUID)
            {
                try
                {
                    var prefabPath = AssetDatabase.GUIDToAssetPath(guid);

                    var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    var images = prefab.GetComponentsInChildren<Image>();

                    foreach (var image in images)
                    {
                        var texturePath = UnityEditor.AssetDatabase.GetAssetPath(image.sprite.texture);
                        TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(texturePath);
                        if (texImporter.spritePackingTag.IsNullOrEmpty() == false)
                        {
                            texImporter.spritePackingTag = "";
                            EditorUtility.SetDirty(texImporter);
                            AssetDatabase.ImportAsset(texturePath);
                        }
                    }

                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }

            }


        }


        private void Compress()
        {
            var texturespaths = AssetDatabase.FindAssets("t:texture", new string[] { path });
            foreach (var guid in texturespaths)
            {
                try
                {
                    var texturePath = AssetDatabase.GUIDToAssetPath(guid);

                    TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(texturePath);
                    Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
                    if (texImporter.textureCompression == TextureImporterCompression.Uncompressed)
                    {
                        texImporter.textureCompression = TextureImporterCompression.Compressed;

                        EditorUtility.SetDirty(texImporter);
                        AssetDatabase.ImportAsset(texturePath);
                    }

                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }

            }

            AssetDatabase.SaveAssets();
        }

        private void Find()
        {
            var texturespaths = AssetDatabase.FindAssets("t:texture", new string[] { path });
            foreach (var guid in texturespaths)
            {
                try
                {
                    var texturePath = AssetDatabase.GUIDToAssetPath(guid);

                    TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(texturePath);
                    Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
                    if (texImporter.textureCompression == TextureImporterCompression.Uncompressed)
                        UnityEngine.Debug.LogWarningFormat(texture, "Compression of {0} is none", texture.name);
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }

            }
        }
    }
}