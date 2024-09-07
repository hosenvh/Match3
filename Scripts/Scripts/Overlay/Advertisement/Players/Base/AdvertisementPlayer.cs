using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Utility.GolmoradLogging;
using UnityEngine;
using static Match3.Overlay.Advertisement.Players.Data.AdvertisementPlayersDataContainer;


namespace Match3.Overlay.Advertisement.Players.Base
{
    public abstract class AdvertisementPlayer : AdvertisementPlayerBase
    {
        protected string rewardedZoneId;
        protected string interstitialZoneId;

        public bool IsInitializationCompleted { get; private set; }

        public void Initialize(AdvertisementPlayerData.AdvertisementPlayerPlatformData platformData, Action onComplete, Action onFailed)
        {
            DebugPro.LogInfo<AdvertisementLogTag>($"Player | Going to Initialize: {GetType().Name}");

            SetZoneIds(platformData.rewardedZoneId, platformData.interstitialZoneId);
            InternalInitialize(
                platformData.appId,
                onComplete: () =>
                {
                    DebugPro.LogInfo<AdvertisementLogTag>($"Player | Initialization Completed: {GetType().Name}");
                    IsInitializationCompleted = true;
                    onComplete.Invoke();
                },
                onFailed: () =>
                {
                    DebugPro.LogWarning<AdvertisementLogTag>($"Player | Initialization Failed: {GetType().Name}");
                    onFailed.Invoke();
                });
        }

        private void SetZoneIds(string rewardedZoneId, string interstitialZoneId)
        {
            this.interstitialZoneId = interstitialZoneId;
            this.rewardedZoneId = rewardedZoneId;
        }

        protected abstract void InternalInitialize(string appId, Action onComplete, Action onFailed);

        public abstract void RequestRewarded(Action onRequestFilled, Action<AdFailReason> onFailed);
        public abstract void RequestInterstitial(Action onRequestFilled, Action<AdFailReason> onFailed);

        public abstract void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed);
        public abstract void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed);

        public abstract bool IsFraudSensitive();
    }
}