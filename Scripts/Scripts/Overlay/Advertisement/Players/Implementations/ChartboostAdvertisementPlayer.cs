#pragma warning disable 0618
#pragma warning disable 0067
using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;
using UnityEngine.Scripting;




#if Chartboost
using ChartboostSDK;
using Match3.Foundation.Base.ServiceLocating;


namespace Match3.Overlay.Advertisement.Players.Implementations
{
    [Preserve]
    public class ChartboostAdvertisementPlayer : AdvertisementPlayer
    {
        private Action onRequestingRewardedSuccess = delegate { };
        private Action<AdFailReason> onRequestingRewardedFailure = delegate { };
        private Action<AdShownResult> onShowinRewardedSuccess = delegate { };
        private Action<AdFailReason> onShowingRewardedFailure = delegate { };

        private Action onRequestingInterstitialSuccess = delegate { };
        private Action<AdFailReason> onRequestingInterstitialFailure = delegate { };
        private Action onShowingInterstitialSuccess = delegate { };
        private Action<AdFailReason> onShowingInterstitialFailure = delegate { };

        protected override void InternalInitialize(string appId, Action onComplete, Action onFailed)
        {
            SetupChartboostListeners(
                onComplete: () => onComplete.Invoke(),
                onFailed: () => onFailed.Invoke()
            );
            ConfigureChartboost(appId);
            onComplete.Invoke();
        }

        private void SetupChartboostListeners(Action onComplete, Action onFailed)
        {
            Chartboost.didFailToLoadRewardedVideo += (location, error) => onRequestingRewardedFailure.Invoke(AdFailReason.NoAdAvailable);
            Chartboost.didCacheRewardedVideo += (location) => onRequestingRewardedSuccess.Invoke();
            Chartboost.didDismissRewardedVideo += (location) => onShowinRewardedSuccess.Invoke(AdShownResult.Skipped);
            Chartboost.didCompleteRewardedVideo += (location, reward) => onShowinRewardedSuccess.Invoke(AdShownResult.Completed);
            Chartboost.didFailToLoadInterstitial += (location, error) => onRequestingInterstitialFailure.Invoke(AdFailReason.NoAdAvailable);
            Chartboost.didCacheInterstitial += (location) => onRequestingInterstitialSuccess.Invoke();
        }

        private void ConfigureChartboost(string appId)
        {
            Chartboost.CreateWithAppId(appId, appSignature: GetAppSignature());
            Chartboost.setAutoCacheAds(false);

            string GetAppSignature() => ServiceLocator.Find<ServerConfigManager>().data.config.chartboostExtraSettingsServerConfig.appSignature;
        }

        public override void RequestRewarded(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            RequestRewardedAdInternal(
                onAlreadyLoaded: onRequestFilled.Invoke,
                onLoadNeeded: () =>
                {
                    onRequestingRewardedSuccess = () =>
                    {
                        ClearRewardedRequestHandlers();
                        onRequestFilled.Invoke();
                    };
                    onRequestingRewardedFailure = (error) =>
                    {
                        ClearRewardedRequestHandlers();
                        onFailed.Invoke(error);
                    };

                    Chartboost.cacheRewardedVideo(CBLocation.Default);
                }
            );
        }

        public override void RequestInterstitial(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            RequestRewardedAdInternal(
                onAlreadyLoaded: onRequestFilled.Invoke,
                onLoadNeeded: () =>
                {
                    onRequestingInterstitialSuccess = () =>
                    {
                        ClearInterstitialRequestHandlers();
                        onRequestFilled.Invoke();
                    };
                    onRequestingInterstitialFailure = (error) =>
                    {
                        ClearInterstitialRequestHandlers();
                        onFailed.Invoke(error);
                    };

                    Chartboost.cacheInterstitial(CBLocation.Default);
                }
            );
        }

        public override void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            this.onShowinRewardedSuccess = onShown;
            this.onShowingRewardedFailure = onFailed;
            Chartboost.showRewardedVideo(CBLocation.Default);
        }

        public override void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed)
        {
            this.onShowingInterstitialSuccess = onShown;
            this.onShowingInterstitialFailure = onFailed;
            Chartboost.showInterstitial(CBLocation.Default);
        }

        public override bool IsFraudSensitive()
        {
            return true;
        }

        private void RequestRewardedAdInternal(Action onAlreadyLoaded, Action onLoadNeeded)
        {
            if (Chartboost.hasRewardedVideo(CBLocation.Default))
                onAlreadyLoaded.Invoke();
            else
                onLoadNeeded.Invoke();
        }

        private void ClearRewardedRequestHandlers()
        {
            onRequestingRewardedSuccess = delegate { };
            onRequestingRewardedFailure = delegate { };
        }

        private void ClearInterstitialRequestHandlers()
        {
            onRequestingInterstitialSuccess = delegate { };
            onRequestingInterstitialFailure = delegate { };
        }
    }
}


#else
namespace Match3.Overlay.Advertisement.Players.Implementations
{
    [Preserve]
    public class ChartboostAdvertisementPlayer : AdvertisementPlayer
    {
        protected override void InternalInitialize(string appId, Action onComplete, Action onFailed)
        {
            onFailed.Invoke();
        }

        public override void RequestRewarded(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            onFailed.Invoke(AdFailReason.AdPlayerNotAvailableInBuild);
        }

        public override void RequestInterstitial(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            onFailed.Invoke(AdFailReason.AdPlayerNotAvailableInBuild);
        }

        public override void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            onFailed.Invoke(AdFailReason.AdPlayerNotAvailableInBuild);
        }

        public override void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed)
        {
            onFailed.Invoke(AdFailReason.AdPlayerNotAvailableInBuild);
        }

        public override bool IsFraudSensitive()
        {
            return true;
        }
    }
}


#endif