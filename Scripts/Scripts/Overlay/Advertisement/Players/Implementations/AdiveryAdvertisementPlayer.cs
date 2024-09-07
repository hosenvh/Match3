using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;
using UnityEngine.Scripting;


#if Adivery
using AdiveryUnity;


namespace Match3.Overlay.Advertisement.Players.Implementations
{
    [Preserve]
    public class AdiveryAdvertisementPlayer : AdvertisementPlayer
    {
        private AdiveryListener adiveryListener;

        private Action<object, AdiveryError> onAdRequestErrorHandler = delegate { };
        private Action<object, string> onRewardedAdRequestSuccessHandler = delegate { };
        private Action<object, string> onInterstitialAdRequestSuccessHandler = delegate { };
        private Action<object, AdiveryReward> onRewardedAdClosedHandler = delegate { };
        private Action<object, string> onInterstitialAdClosedHandler = delegate { };

        protected override void InternalInitialize(string appId, Action onComplete, Action onFailed)
        {
            Adivery.Configure(appId);
            SetupAdiveryListener();
            onComplete.Invoke(); // TODO: Check this to see if adivery plus can be initialized (configured) in one frame, or not, if not then first give a fuck to adivery and call onSuccess after a specific amount of delay
        }

        private void SetupAdiveryListener()
        {
            adiveryListener = new AdiveryListener();

            adiveryListener.OnError += (sender, error) => onAdRequestErrorHandler.Invoke(sender, error);
            adiveryListener.OnRewardedAdLoaded += (sender, zoneId) => onRewardedAdRequestSuccessHandler.Invoke(sender, zoneId);
            adiveryListener.OnInterstitialAdLoaded += (sender, zoneId) => onInterstitialAdRequestSuccessHandler.Invoke(sender, zoneId);
            adiveryListener.OnRewardedAdClosed += (sender, reward) => onRewardedAdClosedHandler.Invoke(sender, reward);
            adiveryListener.OnInterstitialAdClosed += (sender, zoneId) => onInterstitialAdClosedHandler.Invoke(sender, zoneId);

            Adivery.AddListener(adiveryListener);
        }

        public override void RequestRewarded(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            RequestRewardedAdInternal
            (
                zoneId: rewardedZoneId,
                onAlreadyLoaded: onRequestFilled.Invoke,
                onLoadNeeded: zoneId =>
                {
                    onAdRequestErrorHandler = (sender, error) =>
                    {
                        ClearRewardedRequestHandlers();
                        onFailed.Invoke(AdFailReason.NoAdAvailable);
                    };
                    onRewardedAdRequestSuccessHandler = (sender, placementId) =>
                    {
                        ClearRewardedRequestHandlers();
                        onRequestFilled.Invoke();
                    };
                    Adivery.PrepareRewardedAd(zoneId);
                }
            );
        }

        public override void RequestInterstitial(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            RequestRewardedAdInternal
            (
                zoneId: interstitialZoneId,
                onAlreadyLoaded: onRequestFilled.Invoke,
                onLoadNeeded: zoneId =>
                {
                    onAdRequestErrorHandler = (sender, error) =>
                    {
                        ClearInterstitialRequestHandlers();
                        onFailed.Invoke(AdFailReason.NoAdAvailable);
                    };
                    onInterstitialAdRequestSuccessHandler = (sender, placementId) =>
                    {
                        ClearInterstitialRequestHandlers();
                        onRequestFilled.Invoke();
                    };
                    Adivery.PrepareInterstitialAd(zoneId);
                }
            );
        }

        private void RequestRewardedAdInternal(string zoneId, Action onAlreadyLoaded, Action<string> onLoadNeeded)
        {
            if (Adivery.IsLoaded(zoneId))
                onAlreadyLoaded.Invoke();
            else
                onLoadNeeded.Invoke(zoneId);
        }

        public override void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            TryToShowAd
            (
                zoneId: rewardedZoneId,
                onReadyToShow: () =>
                {
                    onRewardedAdClosedHandler = (sender, reward) =>
                    {
                        onRewardedAdClosedHandler = delegate { };
                        onShown.Invoke(reward.IsRewarded ? AdShownResult.Completed : AdShownResult.Skipped);
                    };
                },
                onNotReadyToShow: result => onFailed.Invoke(result)
            );
        }

        public override void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed)
        {
            TryToShowAd
            (
                zoneId: interstitialZoneId,
                onReadyToShow: () =>
                {
                    onInterstitialAdClosedHandler = (sender, placementId) =>
                    {
                        onInterstitialAdClosedHandler = delegate { };
                        onShown.Invoke();
                    };
                },
                onNotReadyToShow: onFailed.Invoke
            );
        }

        public override bool IsFraudSensitive()
        {
            return false;
        }

        private void TryToShowAd(string zoneId, Action onReadyToShow, Action<AdFailReason> onNotReadyToShow)
        {
            if (Adivery.IsLoaded(zoneId))
            {
                onReadyToShow.Invoke();
                Adivery.Show(zoneId);
            }
            else
                onNotReadyToShow.Invoke(AdFailReason.NoAdReady);
        }

        private void ClearRewardedRequestHandlers()
        {
            onAdRequestErrorHandler = delegate { };
            onRewardedAdRequestSuccessHandler = delegate { };
        }

        private void ClearInterstitialRequestHandlers()
        {
            onAdRequestErrorHandler = delegate { };
            onInterstitialAdRequestSuccessHandler = delegate { };
        }
    }
}


#else
namespace Match3.Overlay.Advertisement.Players.Implementations
{
    [Preserve]
    public class AdiveryAdvertisementPlayer : AdvertisementPlayer
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
            return false;
        }
    }
}
#endif