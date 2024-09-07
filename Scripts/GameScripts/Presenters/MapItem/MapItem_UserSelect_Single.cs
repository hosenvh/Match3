using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Match3.Game.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using Match3;
using Match3.Utility.GolmoradLogging;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class MapItem_UserSelect_Single : MapItem_UserSelect, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public int[] costs;
    public List<MapItem_Element> mapItem_Elements = new List<MapItem_Element>();
    public List<string> mapItem_ElementsPaths = new List<string>();

    [Space(10)]
    public MapItem_State_Related myRelatedState;
    public MapItem_State_Related myRelatedState2;
    public MapItem_UserSelect_Single myRelatedUserSelect;
    public MapItem_UserSelect_Single[] myRelatedUserSelects;

    [Space(10)] public bool canBounce = true;
    
#if UNITY_EDITOR
    [Space(10)]
    public bool EDITORIgnoreDynamicResourceLoading = false;
#endif

    public List<AddressableMapItemElementData> addressableElementsData { get; private set; }

    MapItem_Element currentElement;

    int lastSelectedItem, counter;
    bool holding = false, showingPointer = false;
    Vector2 pressedPosition;

    private bool haveParent = false;


    // =================================================================================== \\


    protected override void Awake()
    {
        base.Awake();
        
        if (myRelatedUserSelects.Length > 0)
            for (var i = myRelatedUserSelects.Length - 1; i >= 0; --i)
                myRelatedUserSelects[i].YouHaveParent();
        if (myRelatedUserSelect != null)
            myRelatedUserSelect.YouHaveParent();
    }

    
    public override void Init()
    {
        lastSelectedItem = gameManager.mapItemManager.GetUserSelectItemSelectedIndex(this);

        UpdateAddressableMapItemElementsData();
        
        if (!haveParent)
            ShowElement(lastSelectedItem, false, null);
    }

    private void UpdateAddressableMapItemElementsData()
    {
        addressableElementsData = gameManager.mapItemManager.GetAddressableElements(this);
    }


    public void YouHaveParent()
    {
        haveParent = true;
    }

    
    public override void ShowElement(int elementIndex, bool withAnimation, Action onFinish)
    {
        lastSelectedItem = elementIndex;
        
        if (currentElement != null)
        {
            currentElement.ShowElementState(0, false, null);
            Destroy(currentElement.gameObject);
            currentElement = null;
        }
        
        if (mapItem_ElementsPaths.Count != 0)
        {
            if (elementIndex >= 0)
            {
                if (elementIndex < mapItem_ElementsPaths.Count)
                {
                    currentElement = Instantiate(Resources.Load<MapItem_Element>(mapItem_ElementsPaths[elementIndex]),
                        this.transform, false);
                }
                else
                    currentElement = TryGetAddressableElement(elementIndex);

                if(currentElement != null)
                    currentElement.ShowElementState(1, withAnimation, onFinish);  
            }
        }
        else
        {
            if (elementIndex < 0)
            {
                foreach (var item in mapItem_Elements)
                    item.ShowElementState(0, false, null);
            }
            else if (elementIndex < mapItem_Elements.Count)
            {
                for (int i = 0; i < mapItem_Elements.Count; i++)
                {
                    if (i == elementIndex)
                    {
                        mapItem_Elements[i].ShowElementState(1, withAnimation, onFinish);
                    }
                    else
                    {
                        mapItem_Elements[i].ShowElementState(0, false, null);
                    }
                }
            }
            else
                currentElement = TryGetAddressableElement(elementIndex);
        }

        if (myRelatedUserSelect != null) myRelatedUserSelect.ShowElement(elementIndex, withAnimation, null);
        if (myRelatedUserSelects.Length > 0)
            for (var i = myRelatedUserSelects.Length - 1; i >= 0; --i)
            {
                myRelatedUserSelects[i].ShowElement(elementIndex, withAnimation, null);
            }
    }

    public override void Hide() => currentElement.gameObject.SetActive(false);
    public override void UnHide() => currentElement.gameObject.SetActive(true);

    
    public override void Save(int elementIndex)
    {
        gameManager.mapItemManager.SetUserSelectItemSelectedIndex(this, elementIndex);
        
        if (myRelatedUserSelect != null) myRelatedUserSelect.Save(elementIndex);
        if (myRelatedUserSelects.Length > 0)
            for (var i = myRelatedUserSelects.Length - 1; i >= 0; --i)
            {
                myRelatedUserSelects[i].Save(elementIndex);
            }
    }
    

    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;
        StartCoroutine(CheckDown(eventData.pressPosition, ++counter));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (showingPointer)
        {
            CurrentMapState.pointerSkeletonGraphic.AnimationState.ClearTracks();
            CurrentMapState.pointerSkeletonGraphic.Skeleton.SetToSetupPose();
            showingPointer = false;
        }
        
        if(holding && !(gameManager.CurrentPopup is Popup_MapItemSelector))
            TryBounceForSelfAndRelateds();
        
        holding = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        holding = false;
        if (showingPointer)
        {
            CurrentMapState.pointerSkeletonGraphic.AnimationState.ClearTracks();
            CurrentMapState.pointerSkeletonGraphic.Skeleton.SetToSetupPose();
            showingPointer = false;
        }
    }

    
    IEnumerator CheckDown(Vector2 pressedPosition, int counter)
    {
        yield return new WaitForSeconds(.05f);
        
        if (this.counter == counter && holding)
        {
            var camera = CurrentMapState.mapCameraController.mapCamera;
            // Moving pointer infront of camera.
            CurrentMapState.pointerSkeletonGraphic.transform.position = camera.ScreenToWorldPoint(pressedPosition) + camera.transform.forward * 10;
            CurrentMapState.pointerSkeletonGraphic.AnimationState.SetAnimation(0, "1", false);

            showingPointer = true;
        }
        
        yield return new WaitForSeconds(.15f);
        if (this.counter == counter && holding)
        {
            showingPointer = false;
            
            AnalyticsManager.SendEvent(new AnalyticsData_Map_SelectObject(itemId.ToString(),
                gameManager.mapItemManager.GetUserSelectItemSelectedIndex(this), false));
            
            if (myRelatedState != null)
                myRelatedState.HideCurrentState(true);
            if (myRelatedState2 != null)
                myRelatedState2.HideCurrentState(true);
            
            // change current selected item intractable state to false if exists
            var intractable = currentElement != null
                ? currentElement.GetComponent<Intractable_GameObject>()
                : mapItem_Elements[lastSelectedItem].GetComponent<Intractable_GameObject>();
            if (intractable != null) intractable.ChangeState(false);
            
            TryBounceForSelfAndRelateds();

            UpdateAddressableMapItemElementsData();
            new MapItemSelectorOpener().OpenAndSetup(this, true, (itemIndex) =>
            {
                ShowElement(itemIndex, false, null);
                TryBounceForSelfAndRelateds();
                
            }, (confirm) =>
            {
                if (confirm)
                {
                    Save(lastSelectedItem);
                    AnalyticsManager.SendEvent(new AnalyticsData_Map_ChangeObject(itemId.ToString(), lastSelectedItem, false));
                }
                else
                {
                    int selectedItemIndex = gameManager.mapItemManager.GetUserSelectItemSelectedIndex(this);
                    if (selectedItemIndex != lastSelectedItem)
                        ShowElement(selectedItemIndex, false, null);
                }

                if (myRelatedState != null)
                    myRelatedState.UpdateSelectedState(lastSelectedItem);
                if (myRelatedState2 != null)
                    myRelatedState2.UpdateSelectedState(lastSelectedItem);
            });
            holding = false;
        }
    }
    
    
    private void TryBounceForSelfAndRelateds()
    {
        if(canBounce)
            CurrentMapState.MapItemBounceController.DoBounce(GetSelfAndAllRelatedTransforms());
    }

    private List<Transform> GetSelfAndAllRelatedTransforms()
    {
        List<Transform> relateds = new List<Transform>();
        relateds.Add(transform);
        if(myRelatedState!=null) relateds.Add(myRelatedState.transform);
        if(myRelatedState2!=null) relateds.Add(myRelatedState2.transform);
        if(myRelatedUserSelect!=null) relateds.Add(myRelatedUserSelect.transform);
        if (myRelatedUserSelects != null && myRelatedUserSelects.Length > 0)
            relateds.AddRange(myRelatedUserSelects.Select(x => x.transform));
        return relateds;
    }


    public override bool IsAddressableElement(int elementIndex)
    {
        return elementIndex >= TotalNormalItems();
    }

    public override AddressableMapItemElementData TryGetAddressableElementData(int elementIndex)
    {
        if (IsAddressableElement(elementIndex))
            return addressableElementsData[elementIndex - TotalNormalItems()];
        else
            return null;
    }

    public override int TotalItems()
    {
        return TotalNormalItems() + addressableElementsData.Count;
    }

    private MapItem_Element TryGetAddressableElement(int elementIndex)
    {
        if (elementIndex < TotalItems())
        {
            var addressableElementIndex = elementIndex - TotalNormalItems();
            return gameManager.mapItemManager.CreateAddressableElement(transform,
                addressableElementsData[addressableElementIndex]);
        }
        else
        {
            RaiseOutOfIndexElementError(elementIndex);
            return null;
        }
    }
    
    
    private void RaiseOutOfIndexElementError(int index)
    {
        DebugPro.LogError<MapLogTag>(
            $"try to show element with index of {index.ToString()} that doesn't exists in {gameObject.name} user select.", this);
    }
    
#if UNITY_EDITOR
    [ContextMenu("GetElements")]
    private void GetElements()
    {
        mapItem_Elements.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            MapItem_Element mapItem_Element = transform.GetChild(i).GetComponent<MapItem_Element>();
            if (mapItem_Element)
                mapItem_Elements.Add(mapItem_Element);

        }
    }

    [ContextMenu("Make Resources Dynamic")]
    private void MakeDynamic()
    {
        Match3.EditorTools.Base.MapItems.MapItemResourceDynamicMaker.MakeDynamic(this);
    }

    [ContextMenu("Make Resource Static")]
    private void MakeStatic()
    {
        Match3.EditorTools.Base.MapItems.MapItemResourceDynamicMaker.MakeStatic(this);
    }

#endif
    
    
}