using System;
using UnityEngine;


namespace Match3
{
    public static class PurchasePossibilityChecker
    {
        public const int MIN_LEVEL_FOR_BEING_SAFE_TO_OPEN_SHOP = 4;
        public static TimeSpan ACCEPTABLE_UTC_TIME_ZONE = new TimeSpan(hours: 3, minutes: 30, seconds: 0);

        private static IMarketManager marketManager;
        private static ServerConfigData serverConfigData;

        public static void Initialize(IMarketManager marketManager, ServerConfigData serverConfigData)
        {
            PurchasePossibilityChecker.marketManager = marketManager;
            PurchasePossibilityChecker.serverConfigData = serverConfigData;
        }

        public static bool IsPurchasePossible()
        {
            // TODO: `ShouldCheckPurchasePossibility` should not be market dependent, we only need it for iran google plays, find a better solution
            return marketManager.ShouldCheckPurchasePossibility() == false || IsItSafeToEnablePurchase();
        }

        private static bool IsItSafeToEnablePurchase()
        {
            return IsMinRequiredLevelReached() && IsUserTimeZoneAcceptable();
        }

        private static bool IsMinRequiredLevelReached()
        {
            return Base.gameManager.profiler.LastUnlockedLevel + 1 > MIN_LEVEL_FOR_BEING_SAFE_TO_OPEN_SHOP;
        }

        private static bool IsUserTimeZoneAcceptable()
        {
            return IsTimeZoneCheckActiveFromServer() == false ||  TimeZoneInfo.Local.BaseUtcOffset == ACCEPTABLE_UTC_TIME_ZONE;
        }

        private static bool IsTimeZoneCheckActiveFromServer()
        {
            return serverConfigData.config.purchasePossibility.shouldCheckTimeZone;
        }
    }
}