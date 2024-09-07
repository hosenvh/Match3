using Match3.Foundation.Base.MoneyHandling;
using SeganX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class MetrixAnalyticsManager : IAnalyticsProvider
    {    
        
        public MetrixAnalyticsManager()
        {
            MetrixSDK.Metrix.Initialize("ctiolnymwfusysh");
        }

        public void SendAnalytics(AnalyticsDataBase analyticsDataBase)
        {
            string eventToken = GetEventToken(analyticsDataBase);
            if (!string.IsNullOrEmpty(eventToken))
            {
                MetrixSDK.Metrix.NewEvent(eventToken);
                
            }

            if(analyticsDataBase is AnalyticsData_Purchase_Success purchaseData)
            {
                string revenueEventToken = GetRevenueEventToken(analyticsDataBase);

                ConvertToMetrixRevenueAndCurrency(purchaseData, out double revenue, out int currencyCode);

                if (revenueEventToken != null)
                    MetrixSDK.Metrix.NewRevenue(revenueEventToken, revenue, currencyCode);
            }
        }

        private void ConvertToMetrixRevenueAndCurrency(AnalyticsData_Purchase_Success purchaseData, out double revenue, out int currencyCode)
        {
            // NOTE: We already convert IRR to USD in the data. Here it is reverting the conversion.
            if(purchaseData.revenueCurrency.Equals("USD"))
            {
                revenue = purchaseData.revenue * 10000;
                currencyCode = 0;
            }
            else
            {
                revenue = purchaseData.revenue;
                currencyCode = ConvertToCurrencyCode(purchaseData.revenueCurrency);
            }
        }

        private int ConvertToCurrencyCode(string revenueCurrency)
        {
            // NOTE: These are from the implmenetation of Metrix itself
            switch(revenueCurrency)
            {
                case "IRR":
                    return 0;
                case "USD":
                    return 1;
                case "EUR":
                    return 2;
            }

            UnityEngine.Debug.LogError($"Currency {revenueCurrency} is not supported by Metrix");
            return -1;
        }

        string GetRevenueEventToken(AnalyticsDataBase analyticsDataBase)
        {
            if (analyticsDataBase is AnalyticsData_Purchase_Success_Direct)
                return "ctcil";
            if (analyticsDataBase is AnalyticsData_Purchase_Success)
                return "gdncw";
            return null;
        }

        string GetEventToken(AnalyticsDataBase analyticsDataBase)
        {
            if (analyticsDataBase is AnalyticsData_App_Open)
                return "lxxhz";
            if (analyticsDataBase is AnalyticsData_App_Load)
                return "ohdpr";
            if (analyticsDataBase is AnalyticsData_App_Close)
                return "comxt";
            if (analyticsDataBase is AnalyticsData_Tutorial_Start)
                return "ezyal";
            if (analyticsDataBase is AnalyticsData_Tutorial_Finish)
                return "llaei";
            if (analyticsDataBase is AnalyticsData_Flag_First_Purchase)
                return "pqnad";
            if (TryGetLevelSpecificTokenFor(analyticsDataBase, out var token))
                return token;

            return null;
        }

        private bool TryGetLevelSpecificTokenFor(AnalyticsDataBase analyticsDataBase, out string token)
        {
            token = "";

            if(analyticsDataBase is AnalyticsData_Global_LevelEntry_Win winData)
            {
                switch(winData.levelNumber)
                {
                    case 5:
                        token = "dgccs";
                        return true;
                    case 10:
                        token = "llflw";
                        return true;
                    case 15:
                        token = "ritoj";
                        return true;
                    case 20:
                        token = "jkstm";
                        return true;
                    case 45:
                        token = "ryzdr";
                        return true;
                    case 75:
                        token = "kumvj";
                        return true;
                    case 100:
                        token = "clcbx";
                        return true;
                    case 150:
                        token = "blcbm";
                        return true;
                }
            }

            return false;
        }
    }
}