using System;
using System.Collections.Generic;
using Firebase.Analytics;
using Match3.Foundation.Base.MoneyHandling;
using UnityEngine.Scripting;

[Preserve]
public class FireBaseAnalyticsDataProvider : IAnalyticsProvider
{
    HashSet<int> specialLevelProgressNumbers = new HashSet<int>() { 1, 10, 100, 1000, 2000 };

    public FireBaseAnalyticsDataProvider()
    {

    }
    
    public void NewRevenue(AnalyticsDataBase analyticsDataBase, Money revenue, int currency, string orderId)
    {
        
    }

    public void SendAnalytics(AnalyticsDataBase data)
    {
        if (!FirebaseFunctionalities.IsFirebaseInitialized) return;
        
        var _params = data.analyticParam;
        
        if (data is AnalyticsData_Purchase_Success purchaseData)
        {
            Parameter[] purchaseParameters = 
            {
                new Firebase.Analytics.Parameter(
                    "Purchase_sku", _params[AnalyticsDataMaker.sku_name].ToString()),
                new Firebase.Analytics.Parameter(
                    FirebaseAnalytics.ParameterCurrency, purchaseData.revenueCurrency),
                new Firebase.Analytics.Parameter(
                    FirebaseAnalytics.ParameterValue, Convert.ToDouble(purchaseData.revenue)),
            };
            
            Firebase.Analytics.FirebaseAnalytics.LogEvent("InAppPurchase_Golmorad", purchaseParameters);
        }
             
        else if (data is AnalyticsData_App_Open || data is AnalyticsData_Tutorial_Start || data is AnalyticsData_Tutorial_Finish || data is AnalyticsData_DayOne_End)
            Firebase.Analytics.FirebaseAnalytics.LogEvent(_params[AnalyticsDataMaker.activity_name].ToString());

        else if(data is AnalyticsData_Global_LevelEntry_Win winData)
        {
            if(specialLevelProgressNumbers.Contains(winData.levelNumber))
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"level_win_{winData.levelNumber}");
        }
        else if (data is AnalyticsData_Global_LevelEntry_GiveUp giveUpData)
        {
            if (specialLevelProgressNumbers.Contains(giveUpData.levelNumber))
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"level_lose_{giveUpData.levelNumber}");
        }
        else if (data is AnalyticsData_Global_LevelEntry_Retry retryData)
        {
            if (specialLevelProgressNumbers.Contains(retryData.levelNumber))
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"level_lose_{retryData.levelNumber}");
        }
        else if (data is AnalyticsData_Global_LevelEntry_Start startData)
        {
            if (specialLevelProgressNumbers.Contains(startData.levelNumber))
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"level_load_{startData.levelNumber}");
        }

        else if (data is AnalyticsData_Flag_First_Open)
            Firebase.Analytics.FirebaseAnalytics.LogEvent(_params[AnalyticsDataMaker.flag_name].ToString());
        
    }


}
