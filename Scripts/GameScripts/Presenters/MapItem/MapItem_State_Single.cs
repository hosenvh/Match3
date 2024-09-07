using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Match3;
using Match3.Game.Map;
using Match3.Utility.GolmoradLogging;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapItem_State_Single : MapItem_State
{
    public MapItem_Element[] mapItem_Elements;
    public List<string> mapItem_ElementsPaths;

#if UNITY_EDITOR
    public bool EDITORIgnoreDynamicResourceLoading = false;
#endif

    private int currentStateIndex, hiddenElementIndex;
    public int initIndex = 1;

    List<MapItem_Element> currentElements = new List<MapItem_Element>();

    public override void Init()
    {
        var stateIndex = gameManager.mapItemManager.GetStateItemStateIndex(this, initIndex);
        try
        {
            ShowState(stateIndex, false, null);
        }
        catch (Exception e)
        {
            DebugPro.LogException<MapLogTag>($"Map Item state single ShowState Error \n Name : {gameObject.name} \n Exception Error : {e.Message} \n State Index : {stateIndex} Current Day: {gameManager.taskManager.CurrentDay} \n Last Task ID: {gameManager.taskManager.LastTaskOfCurrentDay.id}");
        }
    }

    public override void ShowState(int stateIndex, bool withAnimation, Action onFinish)
    {
        currentStateIndex = stateIndex;
        if (mapItem_ElementsPaths.Count > 0)
            ShowStateWithDynamicLoading(stateIndex, withAnimation, onFinish);
        else
            for (int i = 0; i < mapItem_Elements.Length; i++)
                mapItem_Elements[i].ShowElementState(stateIndex, withAnimation, i == mapItem_Elements.Length - 1 ? onFinish : null);
    }

    public override void Hide() { }
    public override void UnHide() { }

    void ShowStateWithDynamicLoading(int stateIndex, bool withAnimation, Action onFinish)
    {
        if (stateIndex == 0)
        {
            if (currentElements.Count == 0)
            {
                onFinish?.Invoke();
                return;
            }
            for (int i = 0; i < currentElements.Count; i++)
            {
                var element = currentElements[i];
                if (i == currentElements.Count - 1)
                    element.ShowElementState(stateIndex, withAnimation, () => { onFinish?.Invoke(); Destroy(element.gameObject); });
                else
                    element.ShowElementState(stateIndex, withAnimation, () => Destroy(element.gameObject));
            }
            currentElements.Clear();
        }
        else
        {
            if (currentElements.Count == 0)
            {
                foreach (var path in mapItem_ElementsPaths)
                    currentElements.Add(Instantiate(Resources.Load<MapItem_Element>(path), this.transform, false));
            }

            for (int i = 0; i < currentElements.Count; i++)
                currentElements[i].ShowElementState(stateIndex, withAnimation, i == currentElements.Count - 1 ? onFinish : null);

        }
    }

    public override void Save(int stateIndex)
    {
        gameManager.mapItemManager.SetStateItemStateIndex(this, stateIndex);
    }

#if UNITY_EDITOR
    [ContextMenu("Get Elements")]
    private void GetElements()
    {
        mapItem_Elements = new MapItem_Element[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<MapItem_Element>())
                mapItem_Elements[i] = transform.GetChild(i).GetComponent<MapItem_Element>();
    }

    [ContextMenu("Make Resources Dynamic")]
    private void MakeDynamic()
    {
        Match3.EditorTools.Base.MapItems.MapItemResourceDynamicMaker.MakeDynamic(this);
    }

    [ContextMenu("Make Resources Static")]
    private void MakeStatic()
    {
        Match3.EditorTools.Base.MapItems.MapItemResourceDynamicMaker.MakeStatic(this);
    }
#endif
}