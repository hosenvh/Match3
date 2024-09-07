using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;
using UnityEngine.Scripting;
using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Configuration;
using Match3.Utility.GolmoradLogging;


namespace Match3
{
    [Preserve]
    // NOTE: The whole class needs a redesign.
    public class AdjustAnalyticsDataProvider : IAnalyticsProvider
    {
        public readonly static string ADJUST_SESSION_NUMBER = "AdjustSessionNumber";

        Adjust adjustPrefab;
        string appToken;


        Dictionary<Type, string> dynamicEventTokens = new Dictionary<Type, string>();

        Dictionary<string, string> firstPurchaseTokens = new Dictionary<string, string>();
        Dictionary<string, string> purchaseTokens = new Dictionary<string, string>();

        Dictionary<int, string> specificLevelLoadTokens = new Dictionary<int, string>();
        Dictionary<int, string> specificLevelWinTokens = new Dictionary<int, string>();
        Dictionary<int, string> specificLevelLoseTokens = new Dictionary<int, string>();

        int AdjustSessionNumber
        {
            get { return PlayerPrefs.GetInt(ADJUST_SESSION_NUMBER, 0); }
            set { PlayerPrefs.SetInt(ADJUST_SESSION_NUMBER, value); }
        }

        public AdjustAnalyticsDataProvider()
        {
            ++AdjustSessionNumber;
            ServiceLocator.Find<ConfigurationManager>().Configure(this);

            // NOTE: It is assumed that the prefab is set to startManually 
            var adjust = UnityEngine.GameObject.Instantiate(adjustPrefab);

            AdjustConfig adjustConfig = new AdjustConfig(this.appToken, adjust.environment, (adjust.logLevel == AdjustLogLevel.Suppress));
            adjustConfig.setLogLevel(adjust.logLevel);
            adjustConfig.setSendInBackground(adjust.sendInBackground);
            adjustConfig.setEventBufferingEnabled(adjust.eventBuffering);
            adjustConfig.setLaunchDeferredDeeplink(adjust.launchDeferredDeeplink);

            Adjust.start(adjustConfig);

        }

        public void SendAnalytics(AnalyticsDataBase analyticsDataBase)
        {
            string eventToken = GetEventToken(analyticsDataBase);

            if (!string.IsNullOrEmpty(eventToken))
            {

                AdjustEvent adjustEvent = new AdjustEvent(eventToken);

                // for revenue events
                if (analyticsDataBase is AnalyticsData_Purchase_Success purchaseData)
                {
                    if (analyticsDataBase.analyticParam.ContainsKey(AnalyticsDataMaker.detail))
                        adjustEvent.setTransactionId(analyticsDataBase.analyticParam[AnalyticsDataMaker.detail].ToString());

                    adjustEvent.setRevenue(purchaseData.revenue, purchaseData.revenueCurrency);
                }


                adjustEvent.addPartnerParameter("session", AdjustSessionNumber.ToString());

                foreach (var item in analyticsDataBase.analyticParam)
                {
                    if (item.Value != null)
                        adjustEvent.addPartnerParameter(item.Key, item.Value.ToString());
                    
                }

                Adjust.trackEvent(adjustEvent);
            }

            TrySendAdditionalEventsFor(analyticsDataBase);

        }

        private void TrySendAdditionalEventsFor(AnalyticsDataBase analyticsDataBase)
        {
            if(analyticsDataBase is AnalyticsData_Global_LevelEntry_Start startDatabase)
            {
                if(specificLevelLoadTokens.TryGetValue(startDatabase.levelNumber, out var token))
                    Adjust.trackEvent(new AdjustEvent(token));
            }
            else if (analyticsDataBase is AnalyticsData_Global_LevelEntry_Win winDatabase)
            {
                if (specificLevelWinTokens.TryGetValue(winDatabase.levelNumber, out var token))
                    Adjust.trackEvent(new AdjustEvent(token));
            }
            else if (analyticsDataBase is AnalyticsData_Global_LevelEntry_GiveUp giveUpDatabase)
            {
                if (specificLevelLoseTokens.TryGetValue(giveUpDatabase.levelNumber, out var token))
                    Adjust.trackEvent(new AdjustEvent(token));
            }
            else if (analyticsDataBase is AnalyticsData_Global_LevelEntry_Retry retryDatabase)
            {
                if (specificLevelLoseTokens.TryGetValue(retryDatabase.levelNumber, out var token))
                    Adjust.trackEvent(new AdjustEvent(token));
            }
        }

        public void AddDynamicToken(Type type, string token)
        {
            dynamicEventTokens.Add(type, token);
        }

        public void AddFirstPurchaseToken(string sku, string token)
        {
            firstPurchaseTokens.Add(sku.ToLower(), token);
        }

        public void AddPurchaseToken(string sku, string token)
        {
            purchaseTokens.Add(sku.ToLower(), token);
        }

        public void AddSpecificLevelProgressToken(int levelNumber, string loadToken, string winToken, string loseToken)
        {
            specificLevelLoadTokens[levelNumber] = loadToken;
            specificLevelWinTokens[levelNumber] = winToken;
            specificLevelLoseTokens[levelNumber] = loseToken;
        }

        string GetEventToken(AnalyticsDataBase analyticsDataBase)
        {
            if (analyticsDataBase is AnalyticsData_Purchase_Success purchaseSuccessData)
                return PurchaseTokenFor(purchaseSuccessData);
            else if (analyticsDataBase is AnalyticsData_Flag_First_Purchase firstPurchaseSuccessData)
                return FirstPurchaseTokenFor(firstPurchaseSuccessData);
            else
                return DynamicOrStaticTokenFor(analyticsDataBase);

        }

        private string FirstPurchaseTokenFor(AnalyticsData_Flag_First_Purchase firstPurchaseSuccessData)
        {
            if (firstPurchaseTokens.TryGetValue(firstPurchaseSuccessData.SKU().ToLower(), out var token))
                return token;
            else
            {
                DebugPro.LogWarning<AnalyticsLogTag>($"[AdjustAnalyticsDataProvider] Could not find first purchase token for {firstPurchaseSuccessData.SKU()}");
                return DynamicOrStaticTokenFor(firstPurchaseSuccessData);
            }
        }

        private string PurchaseTokenFor(AnalyticsData_Purchase_Success purchaseSuccessData)
        {
            if (purchaseTokens.TryGetValue(purchaseSuccessData.SKU().ToLower(), out var token))
                return token;
            else
            {
                // DebugPro.LogWarning<AnalyticsLogTag>($"AdjustAnalyticsDataProvider | Could not find purchase token for {purchaseSuccessData.SKU()}");
                return DynamicOrStaticTokenFor(purchaseSuccessData);
            }
        }

        private string DynamicOrStaticTokenFor(AnalyticsDataBase analyticsDataBase)
        {
            if (dynamicEventTokens.TryGetValue(analyticsDataBase.GetType(), out var token))
                return token;
            else
                return GetStaticTokenFor(analyticsDataBase);
        }

        private string GetStaticTokenFor(AnalyticsDataBase analyticsDataBase)
        {
            if (analyticsDataBase is AnalyticsData_App_Open)
                return "8fnkku";
            if (analyticsDataBase is AnalyticsData_App_Load)
                return "v6r5xn";
            if (analyticsDataBase is AnalyticsData_App_Close)
                return "nyz5ze";
            if (analyticsDataBase is AnalyticsData_App_Minimize)
                return "lmpfr5";
            if (analyticsDataBase is AnalyticsData_Snapshot_Open)
                return null;
            if (analyticsDataBase is AnalyticsData_Snapshot_Result)
                return null;
            if (analyticsDataBase is AnalyticsData_Tutorial_Step)
                return "vh6egc";
            if (analyticsDataBase is AnalyticsData_Tutorial_Start)
                return "9hszgd";
            if (analyticsDataBase is AnalyticsData_Tutorial_Finish)
                return "p7hpcc";
            if (analyticsDataBase is AnalyticsData_Heart_Refill_Coin)
                return "g0qr8j";
            if (analyticsDataBase is AnalyticsData_LevelInfo_Open)
                return "by8b1y";
            if (analyticsDataBase is AnalyticsData_LevelInfo_Start)
                return "n3476s";
            if (analyticsDataBase is AnalyticsData_LevelInfo_Close)
                return "35mal3";
            if (analyticsDataBase is AnalyticsData_LevelInfo_Booster_Select)
                return "tr3gj8";
            if (analyticsDataBase is AnalyticsData_LevelInfo_Booster_Select_Lock)
                return "7z0r4f";
            //if (analyticsDataBase is AnalyticsData_LevelResult_Popup)
            //  return "wa1dlk";
            if (analyticsDataBase is AnalyticsData_Global_LevelEntry_Win)
                return "y79qxt";
            if (analyticsDataBase is AnalyticsData_Global_LevelEntry_Lose)
                return "w85g65";
            if (analyticsDataBase is AnalyticsData_Global_LevelEntry_GiveUp)
                return "gfwyyi";
            if (analyticsDataBase is AnalyticsData_Global_LevelEntry_Continue_Extra_Move)
                return "vzdbgk";
            //if (analyticsDataBase is AnalyticsData_LevelResult_Back)
            //  return "xc3fu8";
            if (analyticsDataBase is AnalyticsData_Global_LevelEntry_Retry)
                return "4mbljd";
            if (analyticsDataBase is AnalyticsData_Global_LevelEntry_Start)
                return "1oz4el";
            if (analyticsDataBase is AnalyticsData_Ingame_UseHint)
                return "m23mmn";
            if (analyticsDataBase is AnalyticsData_Ingame_Normal_Success)
                return "eqsbel";
            if (analyticsDataBase is AnalyticsData_Ingame_Special_Move)
                return "bs0qv9";
            if (analyticsDataBase is AnalyticsData_Ingame_Move_Failed)
                return "7a9tke";
            if (analyticsDataBase is AnalyticsData_Ingame_Explosion_Rocket)
                return "v539sv";
            if (analyticsDataBase is AnalyticsData_Ingame_Explosion_Bomb)
                return "99zlau";
            if (analyticsDataBase is AnalyticsData_Ingame_Explosion_Dynamite)
                return "r1enn8";
            if (analyticsDataBase is AnalyticsData_Ingame_Explosion_Tnt)
                return "mrowzf";
            if (analyticsDataBase is AnalyticsData_Ingame_Use_Rainbow)
                return "csb7do";
            if (analyticsDataBase is AnalyticsData_Ingame_Use_Double_Rainbow)
                return "wzg7b5";
            if (analyticsDataBase is AnalyticsData_Ingame_Mission_Done)
                return "33upjy";
            if (analyticsDataBase is AnalyticsData_Ingame_Mission_Step)
                return "p7namw";
            if (analyticsDataBase is AnalyticsData_InGame_Powerup_Hammer)
                return "nlp1ua";
            if (analyticsDataBase is AnalyticsData_InGame_Powerup_Sweeper)
                return "720ph1";
            if (analyticsDataBase is AnalyticsData_InGame_Powerup_Hand)
                return "waug0l";
            //if (analyticsDataBase is AnalyticsData_Ingame_DoneLevel)
            //  return "wo2epb";
            if (analyticsDataBase is AnalyticsData_Ingame_Create_Booster)
                return "ykt4j0";
            if (analyticsDataBase is AnalyticsData_Ingame_Blocked)
                return "v08yw3";
            if (analyticsDataBase is AnalyticsData_Ingame_In_Level_Back)
                return "t45nli";
            if (analyticsDataBase is AnalyticsData_Ingame_Extra_Move)
                return "pqnzoc";
            if (analyticsDataBase is AnalyticsData_Ingame_Create_Rainbow)
                return "b4ftn1";
            if (analyticsDataBase is AnalyticsData_Ingame_AiMove)
                return "gsq7dh";
            if (analyticsDataBase is AnalyticsData_Advertisement_Request)
                return "z65b5i";
            if (analyticsDataBase is AnalyticsData_Advertisement_Complete)
                return "q9yqjc";
            if (analyticsDataBase is AnalyticsData_Advertisement_InComplete)
                return "ilsjdp";
            if (analyticsDataBase is AnalyticsData_Advertisement_NoAd)
                return "2g8qij";
            if (analyticsDataBase is AnalyticsData_Map_SelectObject)
                return "6kncp2";
            if (analyticsDataBase is AnalyticsData_Map_ChangeObject)
                return "tw13ba";
            if (analyticsDataBase is AnalyticsData_Map_HomeTap)
                return "2zud5b";
            if (analyticsDataBase is AnalyticsData_TaskAction_ManagerOpen)
                return "4wc1w6";
            if (analyticsDataBase is AnalyticsData_TaskAction_Done)
                return "iw6v52";
            if (analyticsDataBase is AnalyticsData_TaskAction_Failed)
                return "83rdf5";
            if (analyticsDataBase is AnalyticsData_TaskAction_ManagerClose)
                return "6yndd0";
            if (analyticsDataBase is AnalyticsData_TaskAction_Reward)
                return "lxc7d4";
            if (analyticsDataBase is AnalyticsData_Story)
                return "z9asdg";
            if (analyticsDataBase is AnalyticsData_ShopOpen)
                return "mm6m5j";
            //if (analyticsDataBase is AnalyticsData_Purchase_Success_Direct)
            //    return "26vo6t";
            if (analyticsDataBase is AnalyticsData_Purchase_Success)
                return "ajoykv";
            if (analyticsDataBase is AnalyticsData_Purchase_Failed)
                return "wldv1g";
            if (analyticsDataBase is AnalyticsData_Purchase_Instagram)
                return "miwc6b";
            if (analyticsDataBase is AnalyticsData_Purchase_Telegram)
                return "14yole";
            if (analyticsDataBase is AnalyticsData_Shop_Booster_Popup)
                return "woyx1g";
            if (analyticsDataBase is AnalyticsData_Shop_Booster_Success)
                return "3feln6";
            if (analyticsDataBase is AnalyticsData_Shop_Booster_Failed)
                return "m51aio";
            if (analyticsDataBase is AnalyticsData_Shop_Booster_Close)
                return "n64beg";
            if (analyticsDataBase is AnalyticsData_InGame_PowerUp_Popup)
                return "flf1sl";
            if (analyticsDataBase is AnalyticsData_InGame_PowerUp_Success)
                return "ewkzgu";
            if (analyticsDataBase is AnalyticsData_InGame_PowerUp_Failed)
                return "rxtotf";
            if (analyticsDataBase is AnalyticsData_InGame_PowerUp_Close)
                return "t83pnh";
            if (analyticsDataBase is AnalyticsData_Sink_Source)
                return "8dhojq";
            if (analyticsDataBase is AnalyticsData_Rate_Close)
                return "412pd0";
            if (analyticsDataBase is AnalyticsData_Rate_Confirm)
                return "hl7jpr";
            if (analyticsDataBase is AnalyticsData_Rate_Popup)
                return "cfyqi0";
            if (analyticsDataBase is AnalyticsData_Spinner_Reward)
                return "ow5kbs";
            if (analyticsDataBase is AnalyticsData_Flag_First_Open)
                return "7rehev";
            if (analyticsDataBase is AnalyticsData_Flag_First_CompleteAd)
                return "bzijid";
            if (analyticsDataBase is AnalyticsData_Flag_First_TaskDone)
                return "quu1na";
            if (analyticsDataBase is AnalyticsData_Flag_First_Purchase)
                return "njauq3";
            if (analyticsDataBase is AnalyticsData_Flag_Cohort_Assign)
                return "gef2hd";



            return null;
        }


        public void SetAdjustPrefab(Adjust adjustPrefab)
        {
            this.adjustPrefab = adjustPrefab;
        }

        public void SetAppToken(string appToken)
        {
            this.appToken = appToken;
        }
    }
}