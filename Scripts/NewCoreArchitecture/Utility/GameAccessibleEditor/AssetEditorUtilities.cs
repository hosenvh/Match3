
#pragma warning disable 0618
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class AssetEditorUtilities
{

    public static List<T> FindAssetsByType<T>(string[] paths, bool searchInChildFolders = false) where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();

        foreach (var path in paths)
            assets.AddRange(FindAssetsByType<T>(path, searchInChildFolders));

        return assets;
    }

    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        return FindAssetsByType<T>("", true);
    }

    public static List<T> FindAssetsByType<T>(string path, bool searchInChildFolders = false) where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = new string[0];

        if (path.Equals(""))
            guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        else
        {
            if (Directory.Exists(path))
                guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)), new string[] { path });
            else if (File.Exists(path))
                guids = new string[] { AssetDatabase.AssetPathToGUID(path) };
        }
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                if (searchInChildFolders == true || IsInRoot(assetPath, path))
                    assets.Add(asset);

            }
        }
        return assets;
    }

    public static List<object> FindAssetsByType(string path, Type type, bool searchInChildFolders = false)
    {
        List<object> assets = new List<object>();
        string[] guids;

        if (path.Equals(""))
            guids = AssetDatabase.FindAssets(string.Format("t:{0}", type));
        else
            guids = AssetDatabase.FindAssets(string.Format("t:{0}", type), new string[] { path });
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            object asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
            if (asset != null)
            {
                if (searchInChildFolders == true || IsInRoot(assetPath, path))
                    assets.Add(asset);

            }
        }
        return assets;
    }

    public static List<T> FindComponentsInPrefabsAtPaths<T>(string[] paths, bool includeInActives = false) where T : Component
    {
        var prefabAssetsGuids = AssetDatabase.FindAssets("t:Prefab", paths);
        return GetComponentsAtPrefabs<T>(prefabAssetsGuids, includeInActives);
    }

    public static List<T> FindComponentsInPrefabsByType<T>(bool includeInActive) where T : UnityEngine.Component
    {
        var prefabsGuids = AssetDatabase.FindAssets($"t:Prefab");
        return GetComponentsAtPrefabs<T>(prefabsGuids, includeInActive);
    }

    private static List<T> GetComponentsAtPrefabs<T>(string[] prefabGUIDs, bool includeInActive) where T : UnityEngine.Component
    {
        List<T> components = new List<T>();
        foreach (var guid in prefabGUIDs)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            components.AddRange(prefab.GetComponentsInChildren<T>(includeInActive));
        }
        
        return components;
    }
    
    public static List<T> FindComponentsInScenesByType<T>(bool includeInActive) where T : UnityEngine.Component
    {
        var prefabsGuids = AssetDatabase.FindAssets($"t:Scene");
        List<T> components = new List<T>();

        var initialyOpenedScenes = new HashSet<Scene>(EditorSceneManager.GetAllScenes());

        foreach (var guid in prefabsGuids)
        {
            var scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(guid), OpenSceneMode.Additive);

            foreach(var obj in scene.GetRootGameObjects())
                components.AddRange(obj.GetComponentsInChildren<T>(includeInActive));

            if(initialyOpenedScenes.Contains(scene) == false)
                EditorSceneManager.CloseScene(scene, true);
        }

        return components;
    }

    public static List<T> FindComponentsInScenesAndPrefabsByType<T>(bool includeInActive) where T : UnityEngine.Component
    {
        List<T> components = new List<T>();
        components.AddRange(FindComponentsInPrefabsByType<T>(includeInActive));
        components.AddRange(FindComponentsInScenesByType<T>(includeInActive));
        return components;
    }


    private static bool IsInRoot(string assetPath, string rootPath)
    {
        var assetFolder = Path.GetDirectoryName(assetPath);


        return Path.GetFullPath(assetFolder).Equals(Path.GetFullPath(rootPath), StringComparison.OrdinalIgnoreCase);
    }

    public static string AssetFolder(UnityEngine.Object asset)
    {
        return Path.GetDirectoryName(AssetDatabase.GetAssetPath(asset));
    }

    public static string RelativeAssetPath(string absolutePath)
    {
        string relativePath = absolutePath;
        if (absolutePath.StartsWith(Application.dataPath, StringComparison.Ordinal))
            relativePath = "Assets" + absolutePath.Substring(Application.dataPath.Length);


        return relativePath;
    }


    public static bool IsInResourcesFolder(string assetPath)
    {
        // TODO: Find a better way to check this.
        return assetPath.Contains("Resources/");
    }

    public static string RelativeAssetPathInResources(string assetPath)
    {
        const string RESOURCES = "Resources/";
        // TODO: Find a better way
        return Path.ChangeExtension(assetPath.Remove(0, assetPath.IndexOf(RESOURCES) + RESOURCES.Length), null);
    }


    public static void FindAllAssetsOfType<T>(Action<T> action) where T : UnityEngine.Object
    {
        var groupsGuids = AssetDatabase.FindAssets($"t:{typeof(T)}");

        foreach (var guid in groupsGuids)
        {
            action(AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));
        }
    }

    public static bool TryLoadTextureByPath(string texturePath, out TextureImporter importer, out Texture2D texture)
    {
        try
        {
            importer = (TextureImporter)TextureImporter.GetAtPath(texturePath);
            texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
            return true;
        }
        catch (Exception)
        {
            importer = null;
            texture = null;
            return false;
        }
    }

    public static bool TryLoadTextureByGUID(string guid, out TextureImporter importer, out Texture2D texture)
    {
        var texturePath = AssetDatabase.GUIDToAssetPath(guid);
        return TryLoadTextureByPath(texturePath, out importer, out texture);
    }
}
#endif