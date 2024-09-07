using System;
using UnityEngine;
using Match3.Foundation.Unity;
using Match3.Game.Map;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using Match3.Data.Map;
using UnityEditor;


#endif

public abstract class MapItem_UserSelect : Base
{
    [FormerlySerializedAs("index")] [SerializeField] protected int itemId = 0;

    public ResourceSpriteAsset[] iconResourceSprites;

    protected State_Map CurrentMapState => gameManager.mapManager.CurrentMap;
    

    protected virtual void Awake()
    {
        gameManager.mapItemManager.RegisterMapItem_UserSelect(this);
    }
    

    public abstract void Init();
    public abstract void ShowElement(int elementIndex, bool withAnimation, Action onFinish);
    public abstract void Hide();
    public abstract void UnHide();
    public abstract void Save(int elementIndex);


    public int TotalNormalItems()
    {
        // TODO: This is not a good indication of total items. Find a better way.
        return iconResourceSprites.Length;
    }

    public Sprite IconOf(int itemIndex)
    {
        return iconResourceSprites[itemIndex].Load();
    }
    
    public int GetItemId() { return itemId; }


    public abstract bool IsAddressableElement(int elementIndex);
    public abstract AddressableMapItemElementData TryGetAddressableElementData(int elementIndex);
    public abstract int TotalItems();


#if UNITY_EDITOR
    [ContextMenu("Set Id And Register To Map Items Database")]
    private void SetItemIdAndRegisterToMapItemsDatabase()
    {
        State_Map currentMapState;
        var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if(prefabStage!=null)
            currentMapState = prefabStage.prefabContentsRoot.GetComponent<State_Map>();
        else
            currentMapState = FindObjectOfType<State_Map>();
        
        var userSelectsOfCurrentMapState = currentMapState.GetComponentsInChildren<MapItem_UserSelect>(true);
        
        int[] indexes = new int[1000];
        foreach (var item in userSelectsOfCurrentMapState)
        {
            indexes[item.GetItemId()] = 1;
        }

        for (int i = 0; i < indexes.Length; i++)
        {
            if (indexes[i] == 0)
            {
                itemId = i;
                break;
            }
        }
        
        var mapsItemsDatabaseGuid = AssetDatabase.FindAssets("t:MapsItemsDatabase", new[] {"Assets/Configs"});
        var mapsItemsDatabase = AssetDatabase.LoadAssetAtPath<MapsItemsDatabase>(AssetDatabase.GUIDToAssetPath(mapsItemsDatabaseGuid[0]));
        mapsItemsDatabase.InsertOrUpdateUserSelect(currentMapState.MapId, this);
        EditorUtility.SetDirty(mapsItemsDatabase);
        EditorUtility.SetDirty(currentMapState);
    }
    
#endif
}