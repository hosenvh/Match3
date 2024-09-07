using AssetUsageFinder;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.Optimization
{
    public partial class GolmoradAssetOptimizationUtilityWindow
    {
        public class UnusedAtlasedSpriteFinder
        {
            public List<Texture2D> Find(string path, DisplayProgress displayProgressAction)
            {
                var assetGUIDs = AssetDatabase.FindAssets("t:texture", new string[] { path });
                var foundTextures = new List<Texture2D>();
                int totalItems = assetGUIDs.Length;

                for (int i = 0; i < assetGUIDs.Length; i++)
                {
                    var texturePath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);

                    if (AssetEditorUtilities.TryLoadTextureByPath(texturePath, out var importer, out var texture)
                        && IsUnusedAtlasedSprite(texturePath, importer, texture))
                            foundTextures.Add(texture);
                    
                    if (displayProgressAction.Invoke((float)(i + 1) / totalItems, $"{i + 1}/{ totalItems}"))
                        break;
                }

                return foundTextures;
            }

            private bool IsUnusedAtlasedSprite(string texturePath, TextureImporter importer, Texture2D texture)
            {
                return importer.textureType == TextureImporterType.Sprite
                    && string.IsNullOrEmpty(importer.spritePackingTag) == false
                    && AssetEditorUtilities.IsInResourcesFolder(texturePath) == false
                    && FindDependencies.Dependencies(
                        texture, showProgress: false, disableCacheSerialization: true, returnOnfirstDepenency: true).Count == 0;
            }
        }
    }
}