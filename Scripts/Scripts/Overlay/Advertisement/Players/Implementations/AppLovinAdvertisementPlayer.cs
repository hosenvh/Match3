#pragma warning disable 0618
#pragma warning disable 0067
using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;
using UnityEngine.Scripting;

#if AppLovin

namespace Match3.Overlay.Advertisement.Players.Implementations
{
    [Preserve]
    public class AppLovinAdvertisementPlayer : AdvertisementPlayer
    {
        // NOTE: We must add the SDK_KEY inside applovin integration manager window, too.
        // To add it go to "Third Parties > AppLovin > Integration Manager" and paste the key into "AppLovin SDK Key".
        // Currently, we can't set the SDK_KEY from the server, because AppLovin has some
        // post-processing actions that need to upload the SDK_KEY and get some gradle information.
        // So, it can't do these actions in runtime.
        private const string SDK_KEY = "xnB1yNAwXc1txOHz3tJNzNth9hd5sroVBRZTH5QCRRZ_0kSkBN94mFplG6_HuEG4RoxMv8uwh_nCkEhEAo3p0B";

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
            SetupAppLovinListeners();
            MaxSdk.SetSdkKey(SDK_KEY);
            MaxSdk.InitializeSdk();

            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration => {
                SetupAppLovinListeners();

                MaxSdk.ShowMediationDebugger();
                onComplete.Invoke();
            };
        }

        private void SetupAppLovinListeners()
        {
            SetupRewardedAdListeners();
            SetupInterstitialAdListeners();

            void SetupRewardedAdListeners()
            {
                MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += (adUnitId, adInfo) => onRequestingRewardedFailure.Invoke(AdFailReason.NoAdAvailable);
                MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (adUnitId, adInfo) => onRequestingRewardedSuccess.Invoke();
                MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += (adUnitId, errorInfo, adInfo) => onShowingRewardedFailure.Invoke(AdFailReason.NoAdAvailable);
                MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (adUnitId, adInfo) => onShowinRewardedSuccess.Invoke(AdShownResult.Skipped);
                MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += (adUnitId, adInfo) => onShowinRewardedSuccess.Invoke(AdShownResult.Completed);
            }

            void SetupInterstitialAdListeners()
            {
                MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += (adUnitId, adInfo) => onRequestingInterstitialFailure.Invoke(AdFailReason.NoAdAvailable);
                MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (adUnitId, adInfo) => onRequestingInterstitialSuccess.Invoke();
                MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += (adUnitId, errorInfo, adInfo) => onShowingInterstitialFailure.Invoke(AdFailReason.NoAdAvailable);
                MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += (adUnitId, adInfo) => onShowingInterstitialSuccess.Invoke();
            }
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

                    MaxSdk.LoadRewardedAd(rewardedZoneId);
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

                    MaxSdk.LoadInterstitial(interstitialZoneId);
                }
            );
        }

        public override void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            this.onShowinRewardedSuccess = onShown;
            this.onShowingRewardedFailure = onFailed;

            MaxSdk.ShowRewardedAd(rewardedZoneId);
        }

        public override void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed)
        {
            this.onShowingInterstitialSuccess = onShown;
            this.onShowingInterstitialFailure = onFailed;
            
            MaxSdk.ShowInterstitial(interstitialZoneId);
        }

        public override bool IsFraudSensitive()
        {
            return true;
        }


        private void RequestRewardedAdInternal(Action onAlreadyLoaded, Action onLoadNeeded)
        {
            if (MaxSdk.IsRewardedAdReady(rewardedZoneId))
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
    public class AppLovinAdvertisementPlayer : AdvertisementPlayer
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