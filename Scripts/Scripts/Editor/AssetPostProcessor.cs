using System;
using System.Collections.Generic;
using UnityEditor;

public class AssetPostProcessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        TryToCrunchTextureAssets(importedAssets);
    }

    private static void TryToCrunchTextureAssets(string[] importedAssets)
    {
        const int CRUNCH_QUALITY = 100;
        List<TextureImporter> textureAssets = FindTextureAssets();
        CrunchTextureAssets(textureAssets);

        List<TextureImporter> FindTextureAssets()
        {
            List<TextureImporter> textureAssets = new List<TextureImporter>();

            foreach(string path in importedAssets)
            {
                AssetImporter asset = GetAssetAtPath(path);
                Type assetType = GetAssetType(asset);

                if (IsTypeOfTextureImporter(assetType))
                    textureAssets.Add(asset as TextureImporter);
            }

            return textureAssets;
        }

        AssetImporter GetAssetAtPath(string path) => AssetImporter.GetAtPath(path);

        Type GetAssetType(AssetImporter asset) => asset.GetType();

        bool IsTypeOfTextureImporter(Type type) => type == typeof(TextureImporter);

        void CrunchTextureAssets(List<TextureImporter> textureAssets)
        {
            foreach(TextureImporter texture in textureAssets)
            {
                if (IsTextureAlreadyCrunched(texture))
                    continue;

                texture.crunchedCompression = true;
                texture.compressionQuality = CRUNCH_QUALITY;
            }
        }

        bool IsTextureAlreadyCrunched(TextureImporter texture) => texture.crunchedCompression;
    }
}
