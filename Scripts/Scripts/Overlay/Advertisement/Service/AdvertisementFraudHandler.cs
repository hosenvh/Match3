using System;
using Match3.Overlay.Advertisement.Players.Base;


namespace Match3.Overlay.Advertisement.Service
{
    public static class AdvertisementFraudHandler
    {
        private static TimeSpan UNACCEPTABLE_UTC_TIME_ZONE = new TimeSpan(hours: 3, minutes: 30, seconds: 0);

        public static bool IsFraud(AdvertisementPlayerBase adsPlayer)
        {
            return adsPlayer.IsFraudSensitive() && TimeZoneInfo.Local.BaseUtcOffset == UNACCEPTABLE_UTC_TIME_ZONE;
        }
    }
}