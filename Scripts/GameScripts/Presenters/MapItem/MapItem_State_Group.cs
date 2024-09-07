using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapItem_State_Group : MapItem_State
{
    [SerializeField]
    public List<MapItem_State_Single> mapItem_State_Singles = new List<MapItem_State_Single>();

    public override void Init()
    {
        CheckAndSetStateOfChildes();
    }

    private void CheckAndSetStateOfChildes()
    {
        var mapItemCenter = gameManager.mapItemManager;
        var groupStateIndex = mapItemCenter.GetStateItemStateIndex(this, -1);
        if (groupStateIndex != -1)
        {
            foreach (var stateSingle in mapItem_State_Singles)
            {
                var index = mapItemCenter.GetStateItemStateIndex(stateSingle, stateSingle.initIndex);
                if (index == stateSingle.initIndex)
                {
                    stateSingle.ShowState(groupStateIndex, false, delegate {});
                    stateSingle.Save(groupStateIndex);
                }
            }
        }
    }
    
    public override void ShowState(int stateIndex, bool withAnimation, Action onFinish)
    {
        for (int i = 0; i < mapItem_State_Singles.Count; i++)
        {
            int index = i;
            DelayCall(index * .2f, () =>
            {
                mapItem_State_Singles[index].ShowState(stateIndex, withAnimation, (index == mapItem_State_Singles.Count - 1) ? onFinish : null);
            });
        }
    }
    
    public override void Hide() { }
    public override void UnHide() { }


    public override void Save(int elementIndex)
    {
        foreach (var item in mapItem_State_Singles)
            item.Save(elementIndex);
    }
}