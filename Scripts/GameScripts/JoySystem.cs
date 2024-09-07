using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using SeganX;
using Match3.Foundation.Base.ServiceLocating;
using Match3;
using Match3.Foundation.Base;
using Match3.Foundation.Base.Configuration;

public class JoySystem : Base
{
    #region fields
    string joyValueString = "joyValue";

    private LocalizedStringTerm marketRateMessage;
    
    public int JoyValue
    {
        get { return PlayerPrefs.GetInt(joyValueString, 3); }
        set { PlayerPrefs.SetInt(joyValueString, value); }
    }

    string isPlayerRateTheGameString = "IsPlayerRateTheGame";
    public bool IsPlayerRateTheGame
    {
        get { return PlayerPrefs.GetInt(isPlayerRateTheGameString, 0) == 1; }
        set { PlayerPrefs.SetInt(isPlayerRateTheGameString, value ? 1 : 0); }
    }
    #endregion


    private void Awake()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);
    }

    public void SetMarketRateMessage(LocalizedStringTerm message)
    {
        marketRateMessage = message;
    }
    
    public void CheckShowRateUs(Action onRealRatingOpen, Action onRealRatingCanceled, Action onFailed)
    {
        if (!IsPlayerRateTheGame)
        {
            if (JoyValue > 5)
            {
                gameManager.OpenPopup<Popup_RateBox>().Setup((confirm, rateValue) =>
                {
                    if (confirm)
                    {
                        IsPlayerRateTheGame = true;
                        if (rateValue < 4)
                            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                                ScriptLocalization.Message_JoySystem.ThanksForRating, ScriptLocalization.UI_General.Ok,
                                null, true, result => { onRealRatingCanceled(); });
                        else
                        {
                            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(marketRateMessage,
                                ScriptLocalization.UI_General.Ok, null, true, (confirm2) =>
                                {
                                    onRealRatingOpen();
                                    var marketFunctionality = ServiceLocator.Find<StoreFunctionalityManager>();
                                    marketFunctionality.RequestRating();
                                });
                        }

                        AnalyticsManager.SendEvent(new AnalyticsData_Rate_Confirm(rateValue));
                    }
                    else
                    {
                        onRealRatingCanceled();
                        JoyValue = 0;
                        AnalyticsManager.SendEvent(new AnalyticsData_Rate_Close());
                    }
                });
                AnalyticsManager.SendEvent(new AnalyticsData_Rate_Popup());
            }
            else
                onFailed();
        }
        else
            onFailed();
    }

    public void SetPlayerJoy(bool enjoying)
    {
        if (enjoying)
        {
            JoyValue++;
        }
        else
        {
            JoyValue--;
            JoyValue = Mathf.Max(0, JoyValue);
        }
    }
}