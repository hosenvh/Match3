using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;
using Match3;
using Match3.Game.Map;
using Match3.Utility.GolmoradLogging;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class MapItem_State_Related : MapItem_State
{
    #region fields

    [SerializeField] public MapItem_State_Single[] mapItem_States = null;
    [SerializeField] public MapItem_UserSelect relatedSelector;

    public int SelectedStateIndex { private set; get; } = -1;
    public bool HaveAnyActive { private set; get; } = false;

    #endregion
    
    public override void Init()
    {
        SelectedStateIndex = gameManager.mapItemManager.GetUserSelectItemSelectedIndex(relatedSelector);
        SetMyStateIndexForRelatedState();
    }
    

    public override void ShowState(int stateIndex, bool withAnimation, Action onFinish)
    {
        try
        {
            SelectedStateIndex = gameManager.mapItemManager.GetUserSelectItemSelectedIndex(relatedSelector);
            mapItem_States[SelectedStateIndex].ShowState(stateIndex, withAnimation, onFinish);
            InternalSave(SelectedStateIndex, stateIndex);
            if (stateIndex > 0) HaveAnyActive = true;
        }
        catch (Exception e)
        {
            DebugPro.LogException<MapLogTag>($"Map Item state Related Error (Show State) " +
                                       $"\n Name : {gameObject.name} \n Exception Error : {e.Message} \n Selected State Index : {SelectedStateIndex}");
        }
    }
    
    public override void Hide() { }
    public override void UnHide() { }



    public void SetRelatedStateStep(int stateStep)
    {
        try
        {
            mapItem_States[SelectedStateIndex].ShowState(stateStep, false, null);
            mapItem_States[SelectedStateIndex].Save(stateStep);
        }
        catch (Exception e)
        {
            DebugPro.LogError<MapLogTag>(
                $"Map Item Related Set State Step Error, GameObject Name: {gameObject.name} - State Step: {stateStep}, SelectedIndex: {SelectedStateIndex}, Exception: {e.Message} \n Stack: {e.StackTrace}");
        }
    }


    public void UpdateSelectedState(int newSelectedIndex)
    {
        if (!HaveAnyActive) return;

        if (SelectedStateIndex == -1)
            SelectedStateIndex = gameManager.mapItemManager.GetUserSelectItemSelectedIndex(relatedSelector);
        if (SelectedStateIndex == -1) return;

        if (SelectedStateIndex != newSelectedIndex)
            mapItem_States[SelectedStateIndex].ShowState(0, false, null);

        var stateStep = GetSingleStateStep(mapItem_States[SelectedStateIndex]);
        mapItem_States[newSelectedIndex].ShowState(stateStep, false, null);
        SelectedStateIndex = newSelectedIndex;

        InternalSave(SelectedStateIndex, stateStep);
    }

    public void HideCurrentState(bool hide)
    {
        if (SelectedStateIndex == -1)
            SelectedStateIndex = gameManager.mapItemManager.GetUserSelectItemSelectedIndex(relatedSelector);
        if (SelectedStateIndex == -1) return;
        
        var stateStep = 0;

        if (hide)
        {
            for (var i = mapItem_States.Length - 1; i >= 0; --i)
            {
                if (mapItem_States[i].transform.childCount == 0) continue;

                for (var j = mapItem_States[i].transform.childCount - 1; j >= 0; --j)
                {
                    if (!mapItem_States[i].transform.GetChild(j).gameObject.activeSelf) continue;
                    HaveAnyActive = true;
                    break;
                }
            }

            if (!HaveAnyActive) return;
        }
        else
            stateStep = GetSingleStateStep(mapItem_States[SelectedStateIndex]);
        
        mapItem_States[SelectedStateIndex].ShowState(stateStep, false, null);
    }

    public override void Save(int stateIndex)
    {
        SelectedStateIndex = gameManager.mapItemManager.GetUserSelectItemSelectedIndex(relatedSelector);
        InternalSave(SelectedStateIndex, stateIndex);
    }

    private void InternalSave(int selectedIndex, int stateStep)
    {
        for (var i = 0; i < mapItem_States.Length; i++)
            mapItem_States[i].Save(i == selectedIndex ? stateStep : 0);
    }
    
    
    private int GetSingleStateStep(MapItem_State_Single singleState)
    {
        return gameManager.mapItemManager.GetStateItemStateIndex(singleState, singleState.initIndex);
    }

    private void SetMyStateIndexForRelatedState()
    {
        if(SelectedStateIndex == -1) return;
        var myRelatedStateIndex = gameManager.mapItemManager.GetStateItemStateIndex(this, -1);
        if (myRelatedStateIndex == -1) return;
        ShowState(myRelatedStateIndex, false, delegate {  });
        gameManager.mapItemManager.SetStateItemStateIndex(this, -1);
    }

#if UNITY_EDITOR
    [ContextMenu("Get Elements")]
    private void GetElements()
    {
        mapItem_States = new MapItem_State_Single[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<MapItem_Element>())
                mapItem_States[i] = transform.GetChild(i).GetComponent<MapItem_State_Single>();
    }
#endif
}