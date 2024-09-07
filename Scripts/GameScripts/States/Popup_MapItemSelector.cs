using System.Collections.Generic;
using I2.Loc;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using UnityEngine;
using UnityEngine.UI;
using SeganX;
using Match3.Game.KeyShop;



public class Popup_MapItemSelector : GameState
{
    public bool IsUserInteractionPossible { get; private set; }

    #region fields

    [SerializeField]
    private GameObject allContentsParent = null;
    
    [SerializeField]
    private GameObject cancelButtonGameObject = null;

    [Space(10)]
    [SerializeField] public List<SelectableItemButtonController> selectableButtons = default;

    [Space(10)] [SerializeField] private Button confirmButton = default;

    [Space(10)]
    [SerializeField] private Transform selectablesParent = default;
    [SerializeField] private GameObject selectablePrefab = default;

    [Space(10)]
    [SerializeField] private PurchaseBalloonController balloonController = default;
    [SerializeField] private KeyHudController keyHudController = default;
    

    private int currentSelectedItemIndex;
    private bool[] itemsPurchasedCondition;
    private MapItem_UserSelect currentUserSelector;

    System.Action<int> onItemSelect;
    System.Action<bool> onFinish;

    #endregion



    public Popup_MapItemSelector Setup(MapItem_UserSelect mapItem_UserSelect, bool showCancel, System.Action<int> onItemSelect, System.Action<bool> onFinish)
    {
        IsUserInteractionPossible = false;
        allContentsParent.SetActive(false);

        gameManager.mapManager.CurrentMap.mapCameraController.FocusOnMapItem(mapItem_UserSelect.transform, () =>
        {
            allContentsParent.SetActive(true);
            
            currentUserSelector = mapItem_UserSelect;
            var totalElementsCount = mapItem_UserSelect.TotalItems();
        
            if (totalElementsCount > selectableButtons.Count)
            {
                var newSelectableCount = totalElementsCount - selectableButtons.Count;
                for (; newSelectableCount > 0; --newSelectableCount)
                {
                    GenerateSelectableButton();
                }
            }

            for (var i = 0; i < totalElementsCount; i++)
            {
                selectableButtons[i].gameObject.SetActive(true);
                selectableButtons[i].Init(i, OnSelectableItemButtonClicked);
                if (mapItem_UserSelect.IsAddressableElement(i))
                {
                    var addressableElement = mapItem_UserSelect.TryGetAddressableElementData(i);
                    selectableButtons[i].LoadIcon(addressableElement.iconReference);
                }
                else
                    selectableButtons[i].SetIcon(currentUserSelector.IconOf(i));
            }

            InitForPurchasableItems(currentUserSelector);

            cancelButtonGameObject.SetActive(showCancel);

            this.onItemSelect = onItemSelect;
            this.onFinish = onFinish;

            SetSelectedIcon(gameManager.mapItemManager.GetUserSelectItemSelectedIndex(currentUserSelector));
            IsUserInteractionPossible = true;
        });
        
        return this;
    }
    
    
    
    public void OnConfirmClick(bool confirm)
    {
        gameManager.fxPresenter.PlayClickAudio();

        gameManager.tutorialManager.CheckAndHideTutorial(3);

        gameManager.ClosePopup();
        onFinish(confirm);
    }

    public override void Back()
    {
        if (cancelButtonGameObject && IsUserInteractionPossible)
            OnConfirmClick(false);
    }


    public void OnSelectableItemButtonClicked(int id)
    {
        currentSelectedItemIndex = id;

        gameManager.tutorialManager.CheckAndHideTutorial(2);
        SetSelectedIcon(id);

        if (currentUserSelector is MapItem_UserSelect_Single singleSelector && !itemsPurchasedCondition[id])
        {
            ActivePurchaseBalloon(true, singleSelector.costs[id], id);
            confirmButton.interactable = false;
        }
        else
        {
            ActivePurchaseBalloon(false);
            confirmButton.interactable = true;
        }

        onItemSelect(id);
    }


    private void InitForPurchasableItems(MapItem_UserSelect mapItem_UserSelect)
    {
        if (!(mapItem_UserSelect is MapItem_UserSelect_Single singleCast)) return;

        var itemPurchaseService = ServiceLocator.Find<MapItemPurchaseManager>();
        itemsPurchasedCondition = itemPurchaseService.GetItemsPurchaseCondition(singleCast);

        for (var i = itemsPurchasedCondition.Length - 1; i >= 0; --i)
        {
            selectableButtons[i].SetLock(!itemsPurchasedCondition[i]);
        }
    }


    // --------------------------------------------- Purchase Balloon methods --------------------------------------------- \\ 

    private void ActivePurchaseBalloon(bool active, int cost = 0, int itemIndex = 0)
    {
        if (active && currentUserSelector is MapItem_UserSelect_Single userSelectSingle)
        {
            balloonController.Open(selectableButtons[itemIndex].gameObject.GetRectTransform().position.x, userSelectSingle, itemIndex, PurchaseItemCallback);
        }
        else
            balloonController.Close();
    }

    private void PurchaseItemCallback(bool result)
    {
        itemsPurchasedCondition[currentSelectedItemIndex] = result;
        confirmButton.interactable = result;
        if (result) balloonController.Close();
        selectableButtons[currentSelectedItemIndex].SetLock(!result);

        keyHudController.UpdateKeyHud();

        if (!result)
        {
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Purchase.NotEnoughKey, ScriptLocalization.UI_General.Ok,
                "", true,
                res =>
                {
                    if (!res) return;
                    OpenKeyShop();
                });
        }
    }


    // =========================================================================================================== \\


    public void OpenKeyShop()
    {
        gameManager.shopCreator.TrySetupKeyShop("map", keyHudController.UpdateKeyHud);
    }
    
    
    
    private void SetSelectedIcon(int index)
    {
        for (var i = selectableButtons.Count - 1; i >= 0; --i)
        {
            selectableButtons[i].SetSelect(index >= 0 && index == i);
        }
    }

    private void GenerateSelectableButton()
    {
        var newSelectable = Instantiate(selectablePrefab, selectablesParent);
        var newSelectableController = newSelectable.GetComponent<SelectableItemButtonController>();
        newSelectableController.Init(selectableButtons.Count, OnSelectableItemButtonClicked);
        selectableButtons.Add(newSelectableController);
    }
}