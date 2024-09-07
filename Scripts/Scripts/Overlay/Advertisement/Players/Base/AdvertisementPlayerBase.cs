using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Data;


namespace Match3.Overlay.Advertisement.Players.Base
{
    public interface AdvertisementPlayerBase
    {
        bool IsInitializationCompleted { get; }
        void Initialize(AdvertisementPlayersDataContainer.AdvertisementPlayerData.AdvertisementPlayerPlatformData platformData, Action onComplete, Action onFailed);

        void RequestRewarded(Action onRequestFilled, Action<AdFailReason> onFailed);
        void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed);

        void RequestInterstitial(Action onRequestFilled, Action<AdFailReason> onFailed);
        void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed);
        bool IsFraudSensitive();
    }
}