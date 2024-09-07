using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.Optimization
{
    public partial class GolmoradAssetOptimizationUtilityWindow
    {
        public class MipMapsEnabledTextureFinder
        {
            public List<Texture2D> Find(DisplayProgress displayProgressAction)
            {
                var foundTextures = new List<Texture2D>();
                var texturesGUIDs = AssetDatabase.FindAssets("t:texture");

                var totalTextures = texturesGUIDs.Length;

                for (int i = 0; i < totalTextures; i++)
                {
                    if (AssetEditorUtilities.TryLoadTextureByGUID(texturesGUIDs[i], out var importer, out var texture))
                    {
                        if (importer.mipmapEnabled)
                            foundTextures.Add(texture);
                    }

                    if (displayProgressAction.Invoke((float)(i + 1) / totalTextures, $"{i + 1}/{totalTextures}"))
                        break;
                }

                return foundTextures;

            }
        }

    }
}