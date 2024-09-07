using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Game.Map;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapItem_UserSelect_Group : MapItem_UserSelect
{

    public List<MapItem_UserSelect_Single> mapItem_UserSelect_Singles = new List<MapItem_UserSelect_Single>();


    public override void Init()
    {
        CheckAndSetSelectedIndexOfChildes();
    }

    private void CheckAndSetSelectedIndexOfChildes()
    {
        var mapItemCenter = gameManager.mapItemManager;
        var groupSelectedIndex = mapItemCenter.GetUserSelectItemSelectedIndex(this);
        if (groupSelectedIndex != -1)
        {
            foreach (var userSelectSingle in mapItem_UserSelect_Singles)
            {
                var selectedIndex = mapItemCenter.GetUserSelectItemSelectedIndex(userSelectSingle);
                if (selectedIndex == -1)
                {
                    userSelectSingle.ShowElement(groupSelectedIndex, false, delegate {  });
                    userSelectSingle.Save(groupSelectedIndex);
                }
            }
        }
    }
    
    public override void ShowElement(int elementIndex, bool withAnimation, Action onFinish)
    {
        if (withAnimation)
        {
            for (int i = 0; i < mapItem_UserSelect_Singles.Count; i++)
            {
                int index = i;
                DelayCall(index * .2f, () =>
                {
                    mapItem_UserSelect_Singles[index].ShowElement(elementIndex, true, (index == mapItem_UserSelect_Singles.Count - 1) ? onFinish : null);
                });
            }
        }
        else
        {
            for (int i = 0; i < mapItem_UserSelect_Singles.Count; i++)
                mapItem_UserSelect_Singles[i].ShowElement(elementIndex, false, null);
        }

    }
    
    public override void Hide() { }
    public override void UnHide() { }


    public override void Save(int elementIndex)
    {
        gameManager.mapItemManager.SetUserSelectItemSelectedIndex(this, elementIndex);

        foreach (var item in mapItem_UserSelect_Singles)
            item.Save(elementIndex);
    }

    public override bool IsAddressableElement(int elementIndex)
    {
        return false;
    }

    public override AddressableMapItemElementData TryGetAddressableElementData(int elementIndex)
    {
        return null;
    }

    public override int TotalItems()
    {
        return TotalNormalItems();
    }
}