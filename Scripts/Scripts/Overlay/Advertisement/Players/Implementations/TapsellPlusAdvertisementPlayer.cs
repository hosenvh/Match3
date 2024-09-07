using UnityEngine;
using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Utility.GolmoradLogging;
using UnityEngine.Scripting;


#if TapsellPlus
using TapsellPlusSDK;

namespace Match3.Overlay.Advertisement.Players.Implementations
{
    [Preserve]
    public class TapsellPlusAdvertisementPlayer : AdvertisementPlayer
    {
        private string loadedRewardedAdId;
        private string loadedInterstitialAdId;

        protected override void InternalInitialize(string appId, Action onComplete, Action onFailed)
        {
            TapsellPlus.Initialize(
                appId,
                onInitializeSuccess: s => onComplete.Invoke(),
                onInitializeFailed: error =>
                {
                    DebugPro.LogError<AdvertisementLogTag>($"Tapsell Initialization Failed : {error}");
                    onFailed.Invoke();
                });
            TapsellPlus.SetGdprConsent(true);
        }

        public override void RequestRewarded(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            RequestAd(rewardedZoneId, onRequestFilled, onFailed);
        }

        public override void RequestInterstitial(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            RequestAd(interstitialZoneId, onRequestFilled, onFailed);
        }

        private void RequestAd(string zoneId, Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            TapsellPlus.RequestRewardedVideoAd(
                zoneId,
                onRequestResponse: tapsellPlusAdModel =>
                {
                    loadedRewardedAdId = tapsellPlusAdModel.responseId;
                    onRequestFilled();
                },
                onRequestError: error =>
                {
                    loadedRewardedAdId = string.Empty;
                    onFailed(AdFailReason.NoAdAvailable);
                });
        }

        public override void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            ShowAdInternal(isRewardedAd: true, onShown, onFailed);
        }

        private void ShowAdInternal(bool isRewardedAd, Action<AdShownResult> onSuccessCallback, Action<AdFailReason> onFailureCallback)
        {
            bool isAllowedToShow = loadedRewardedAdId.IsNullOrEmpty() == false;
            if (!isAllowedToShow)
                return;

            if (isRewardedAd)
                ShowRewardedAdInternal(onSuccessCallback, onFailureCallback);
            else
                ShowInterstitialAdInternal(onSuccessCallback, onFailureCallback);
        }

        private void ShowRewardedAdInternal(Action<AdShownResult> onSuccessCallback, Action<AdFailReason> onFailureCallback)
        {
            TapsellPlus.ShowRewardedVideoAd(
                responseId: loadedRewardedAdId,
                onAdOpened: tapsellPlusAdModel =>
                {
                }, onAdRewarded: tapsellPlusAdModel =>
                {
                    AdShowingFinished(isRewardedAd: true, finishState: AdShownResult.Completed, onSuccessCallback, onFailureCallback);
                }, onAdClosed: tapsellPlusAdModel =>
                {
                    AdShowingFinished(isRewardedAd: true, finishState: AdShownResult.Skipped, onSuccessCallback, onFailureCallback);
                }, onShowError: error =>
                {
                    loadedRewardedAdId = string.Empty;
                    onFailureCallback.Invoke(AdFailReason.UnknownError);
                });
        }

        private void ShowInterstitialAdInternal(Action<AdShownResult> onSuccessCallback, Action<AdFailReason> onFailureCallback)
        {
            TapsellPlus.ShowInterstitialAd(
                responseId: interstitialZoneId,
                onAdOpened: tapsellPlusAdModel =>
                {
                }, onAdClosed: tapsellPlusAdModel =>
                {
                    AdShowingFinished(isRewardedAd: false, finishState: AdShownResult.Skipped, onSuccessCallback, onFailureCallback);
                }, onShowError: error =>
                {
                    interstitialZoneId = string.Empty;
                    onFailureCallback.Invoke(AdFailReason.UnknownError);
                });
        }

        private void AdShowingFinished(bool isRewardedAd, AdShownResult finishState, Action<AdShownResult> onSuccessCallback, Action<AdFailReason> onFailureCallback)
        {
            if (isRewardedAd == false && finishState == AdShownResult.Skipped)
                finishState = AdShownResult.Completed;

            onSuccessCallback(finishState);

            if (isRewardedAd)
                loadedRewardedAdId = string.Empty;
            else
                interstitialZoneId = string.Empty;
        }

        public override void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed)
        {
            ShowAdInternal(isRewardedAd: false, onSuccessCallback: result => onShown.Invoke(), onFailed);
        }

        public override bool IsFraudSensitive()
        {
            return false;
        }
    }
}

#else
namespace TapsellPlusSDK
{
    public class TapsellPlusNativeBannerAd
    {
        public Texture iconImage;
        public Texture landscapeBannerImage;
        public string title;
        public string description;

        public void RegisterImageGameObject(GameObject gameObject)
        {
        }
    }

    public class TapsellPlusRequestError
    {
    }

    public class TapsellPlusErrorModel
    {
    }
}

namespace Match3.Overlay.Advertisement.Players.Implementations
{
    [Preserve]
    public class TapsellPlusAdvertisementPlayer : AdvertisementPlayer
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