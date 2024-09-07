using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using SeganX;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ShopManagement;
using Match3.Game.MainShop;
using Match3.Presentation.TextAdapting;
using UnityEngine.UI;

public class BoosterSelector : Base
{
    #region fields
    
    [SerializeField] int index = default;
    [SerializeField] LocalText boosterCountText = null;

    [SerializeField] private GameObject boosterIconGameObject = null,
        lockImageGameObject = null,
        boosterSelectedImageGameObject = null,
        plusImageGameObject = null;

    [SerializeField] private Image boosterCountFrameImage;
    [SerializeField] private Sprite boosterCountInfinitySprite;
    [SerializeField] private Sprite boosterCountNormalSprite;

    [SerializeField] private GameObject infinityTimerParentObject;
    [SerializeField] private TextAdapter infinityTimerText;
    
    System.Action onGuiChange;

    private BoostersManager boosterManager;
    
    #endregion

    #region methods
    
    
    
    public void Setup(System.Action onGuiChange)
    {
        this.onGuiChange = onGuiChange;

        boosterManager = gameManager.profiler.BoosterManager;

        if (boosterManager.IsInfiniteBoosterAvailable(index))
            StartCoroutine(nameof(InfinityTimerOneSecondUpdate));
        
        UpdateGUI();
    }

    private IEnumerator InfinityTimerOneSecondUpdate()
    {
        var oneSecondWait = new WaitForSeconds(1);
        int remainingTime;
        do
        {
            remainingTime = boosterManager.GetRemainingInfinityBoosterTime(index);
            infinityTimerText.SetText(Utilities.GetFormatedTime(remainingTime));
            yield return oneSecondWait;
        } 
        while (remainingTime >= 0);
        UpdateGUI();
    }

    public void UpdateGUI()
    {
        boosterIconGameObject.SetActive(false);
        lockImageGameObject.SetActive(false);
        plusImageGameObject.SetActive(false);
        boosterCountText.gameObject.SetActive(false);
        boosterSelectedImageGameObject.SetActive(false);
        infinityTimerParentObject.SetActive(false);
        boosterCountFrameImage.sprite = boosterCountNormalSprite;

        if (IsBoosterUnlocked())
            boosterIconGameObject.SetActive(true);
        else
            lockImageGameObject.SetActive(true);
        
        if (boosterManager.IsInfiniteBoosterAvailable(index))
        {
            infinityTimerParentObject.SetActive(true);
            boosterCountFrameImage.sprite = boosterCountInfinitySprite;
        }
        else
        {
            int boosterCount = gameManager.profiler.GetBoosterCount(index);
            boosterCountText.SetText(boosterCount.ToString());

            if (gameManager.profiler.isBoosterSelected[index])
                boosterSelectedImageGameObject.SetActive(true);
            else
            {
                if (boosterCount > 0)
                    boosterCountText.gameObject.SetActive(true);
                else
                    plusImageGameObject.SetActive(true);
            }
        }
    }
    
    
    
    public void OnClick()
    {
        if (gameManager.profiler.BoosterManager.IsInfiniteBoosterAvailable(index))
            return;
        
        gameManager.tutorialManager.CheckAndHideTutorial(28);
        gameManager.tutorialManager.CheckAndHideTutorial(29);
        gameManager.tutorialManager.CheckAndHideTutorial(30);

        if (IsBoosterUnlocked())
        {
            int boosterCount = gameManager.profiler.GetBoosterCount(index);
            if (boosterCount > 0)
            {
                ToggleBoosterSelected();
                UpdateGUI();
            }
            else
            {
                OpenBoosterPurchasePanel();
            }
        }
        else
        {
            ShowUnlockInfoMessage();
        }
    }
    
    private bool IsBoosterUnlocked()
    {
        return gameManager.profiler.LastUnlockedLevel >= gameManager.profiler.BoosterManager.boosterUnlockLevels[index] - 1;
    }

    private void OpenBoosterPurchasePanel()
    {
        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                    string.Format(ScriptLocalization.Message_Purchase.AskPurchaseBooster,
                        gameManager.profiler.GetBoosterPrice(index)), ScriptLocalization.UI_General.Yes,
                    ScriptLocalization.UI_General.No, true, (confirm) =>
                    {
                        if (confirm)
                        {
                            if (gameManager.profiler.CoinCount >= gameManager.profiler.GetBoosterPrice(index))
                            {
                                gameManager.profiler.ChangeCoin(-gameManager.profiler.GetBoosterPrice(index),
                                    "buy booster: " + index);
                                gameManager.profiler.BoosterManager.AddBooster(index, 3);
                                onGuiChange();
                                AnalyticsManager.SendEvent(
                                    new AnalyticsData_Shop_Booster_Success(gameManager.profiler.GetBoosterPrice(index),
                                        index));
                            }
                            else
                            {
                                gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                                    ScriptLocalization.Message_Purchase.NotEnoughCoin, ScriptLocalization.UI_General.Ok,
                                    null, true, (confirm2) =>
                                    {
                                        gameManager.shopCreator.TrySetupMainShop("Booster", onGuiChange);
                                    });
                                AnalyticsManager.SendEvent(
                                    new AnalyticsData_Shop_Booster_Failed(gameManager.profiler.GetBoosterPrice(index),
                                        index));
                            }
                        }
                        else
                            AnalyticsManager.SendEvent(
                                new AnalyticsData_Shop_Booster_Close(gameManager.profiler.GetBoosterPrice(index),
                                    index));
                    });
                AnalyticsManager.SendEvent(new AnalyticsData_Shop_Booster_Popup(gameManager.profiler.GetBoosterPrice(index), index));
    }
    
    private void ToggleBoosterSelected()
    {
        gameManager.profiler.isBoosterSelected[index] = !gameManager.profiler.isBoosterSelected[index];
        AnalyticsManager.SendEvent(new AnalyticsData_LevelInfo_Booster_Select(index.ToString()));
    }

    private void ShowUnlockInfoMessage()
    {
        gameManager.OpenPopup<Popup_ConfirmBox>()
            .Setup(
                string.Format(ScriptLocalization.Message_GamePlay.BoosterUnlockLevel,
                    gameManager.profiler.BoosterManager.boosterUnlockLevels[index]), ScriptLocalization.UI_General.Ok, null, true,
                null);
        AnalyticsManager.SendEvent(new AnalyticsData_LevelInfo_Booster_Select_Lock(index.ToString()));
    }



    #endregion
}