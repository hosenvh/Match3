using System;
using Match3.Data.Map;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;


#endif

public abstract class MapItem_State : Base
{
    [FormerlySerializedAs("index")] [SerializeField] protected int itemId;

    private void Awake()
    {
        gameManager.mapItemManager.RegisterMapItem_State(this);
    }

    public abstract void Init();
    public abstract void ShowState(int stateIndex, bool withAnimation, Action onFinish);
    public abstract void Hide();
    public abstract void UnHide();
    public abstract void Save(int stateIndex);

    public int GetItemId() { return itemId; }

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
        
        var stateItemsOfCurrentMapState = currentMapState.GetComponentsInChildren<MapItem_State>(true);
        
        int[] indexes = new int[1000];

        foreach (var item in stateItemsOfCurrentMapState)
            indexes[item.GetItemId()] = 1;

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
        mapsItemsDatabase.InsertOrUpdateState(currentMapState.MapId, this);
        EditorUtility.SetDirty(mapsItemsDatabase);
        EditorUtility.SetDirty(currentMapState);
    }
#endif
}