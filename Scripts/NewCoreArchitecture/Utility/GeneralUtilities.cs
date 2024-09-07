using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PandasCanPlay.HexaWord.Utility
{
    // TODO: Please rename this.
    public static class GeneralUtilities
    {
        public static void CopyToClipboard(string text)
        {
            TextEditor te = new TextEditor();
            te.text = text;
            te.SelectAll();
            te.Copy();
        }
        
        public static IEnumerator ExtractStreamingAssetFilePath(string streamingAssetPath, Action<string> onExtract)
        {
            var streamingPath = Path.Combine(Application.streamingAssetsPath, streamingAssetPath);

            if (Application.platform != RuntimePlatform.Android && Application.platform!=RuntimePlatform.WebGLPlayer)
            {
                onExtract(streamingPath);
            }
            else
            {
                #pragma warning disable 0618
                var requestStreamingAsset = new WWW(streamingPath);
                #pragma warning restore 0618
                yield return requestStreamingAsset;

                if (!string.IsNullOrEmpty(requestStreamingAsset.error))
                {
                    onExtract(null);
                }
                else
                {
                    var filePath = Application.persistentDataPath + "/" + streamingAssetPath;
                    if(!File.Exists(filePath))
                        File.WriteAllBytes(filePath, requestStreamingAsset.bytes);
                    onExtract(filePath);
                }
            }
        }
        
        
    }
}