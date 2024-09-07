using System.Collections.Generic;
using System.IO;
using ArmanCo.ShapeRunner.Utility;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Intractable_GameObject : Intractable
{
    
    // ------------------------------------------- Public Fields ------------------------------------------- \\
    
    public List<GameObject> effectObjects;
    public List<string> effectObjectsResourcePath;

#if UNITY_EDITOR
    [Button("Make Effects Dynamic", nameof(MakeEffectObjectDynamic))]
#endif
    public string makeEffectObjectsDynamic;

#if UNITY_EDITOR
    [Button("Make Effects Static", nameof(MakeEffectsObjectsStatic))]
#endif
    public string makeEffectObjectsStatic;
    
    // ------------------------------------------- Properties ------------------------------------------- \\

    public bool ShouldActiveEffects => CurrentStateIndex == 1;

    // ------------------------------------------- Private Fields ------------------------------------------- \\

    private const string RESOURCES_ROOT_FOLDER = "Resources";
    private const string BASE_INTRACTABLES_PATH = "Intractables";
    
    // ====================================================================================================== \\


    protected override void InternalClickDown()
    {
        // Nothing for this implementation
    }

    protected override void InternalClickUp()
    {
        if (effectObjects.Count <= 0) return;
        CurrentStateIndex = CurrentStateIndex == 1 ? 0 : 1;
        ChangeState(ShouldActiveEffects);
    }
    
    

    public void ChangeState(bool state)
    {
        foreach (var effectObject in effectObjects)
            effectObject.SetActive(state);

        CurrentStateIndex = state ? 1 : 0;
        SaveState();
    }

    

    protected override void InternalLoadState()
    {
        for(int i=effectObjects.Count-1; i>=0; i--)
            effectObjects[i].SetActive(ShouldActiveEffects);
    }

    protected override void InternalSaveState()
    {
        // Nothing
    }
    
#if UNITY_EDITOR
    
    // CAUTION: Loading and unloading effects dynamically need more design and work, so following codes
    // are just a starting point and not ready yet.
    public void MakeEffectObjectDynamic()
    {
        if (effectObjects.Count == 0) return;
        
        Undo.RecordObject(gameObject, "Unload Effects Objects");
        
        effectObjectsResourcePath = new List<string>(effectObjects.Count);
        var effectsResourceFolderPath = Path.Combine(BASE_INTRACTABLES_PATH, gameObject.name);
        var effectsAssetsFolderPath = Path.Combine(RESOURCES_ROOT_FOLDER, effectsResourceFolderPath);
        CreateEffectsDirectory(effectsAssetsFolderPath);
        
        foreach (var effectObject in effectObjects)
        {
            var effectFileFullPath = Path.Combine("Assets", effectsAssetsFolderPath, effectObject.name);
            var effectFileResourcePath = Path.Combine(effectsResourceFolderPath, effectObject.name);

            PrefabUtility.SaveAsPrefabAssetAndConnect(
                effectObject,
                $"{effectFileFullPath}.prefab", 
                UnityEditor.InteractionMode.AutomatedAction, 
                out bool isSucceeded);
            
            if (!isSucceeded)
            {
                Debug.LogErrorFormat("Couldn't create prefab for {0} in {1}", effectObject.name, effectsResourceFolderPath);
                return;
            }
            
            effectObjectsResourcePath.Add(effectFileResourcePath);
            Undo.DestroyObjectImmediate(effectObject);
        }
        
        effectObjects.Clear();
    }

    public void MakeEffectsObjectsStatic()
    {
        if (effectObjectsResourcePath.Count == 0) return;
        
        Undo.RecordObject(gameObject, "Load Effects Objects");

        foreach (var effectResourcePath in effectObjectsResourcePath)
        {
            var effectResource = Resources.Load<GameObject>(effectResourcePath);
            var effectObject = Instantiate(effectResource, transform, false);
            effectObject.name = effectResource.gameObject.name;
            effectObjects.Add(effectObject);
            Undo.RegisterCreatedObjectUndo(effectObject, $"Created Effect GO {effectObject}");
        }
        
        effectObjectsResourcePath.Clear();
    }
    
    
    public void CreateEffectsDirectory(string assetFolderPath)
    {
        var fullPath = Path.Combine(Application.dataPath, assetFolderPath);
        
        if (Directory.Exists(fullPath) == false)
            Directory.CreateDirectory(fullPath);
    }
    
#endif

}
