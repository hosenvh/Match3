using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;
using UnityEngine.Scripting;

#if AdMob
using Reward = GoogleMobileAds.Api.Reward;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using UnityEngine;
using Match3.Utility.GolmoradLogging;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

#endif


namespace Match3.Overlay.Advertisement.Players.Implementations
{

    #if AdMob

    [Preserve]
    public class AdMobAdvertisementPlayer : AdvertisementPlayer
    {
        private const string RewardedTestVideoZoneId = "ca-app-pub-3940256099942544/5224354917";
        private const string InterstitialTestZoneId = "ca-app-pub-3940256099942544/1033173712";

        private bool testMode = true;
        // private bool isInitialized = false;

        private bool adRewarded = false;
        private bool isRewardedRequestInProgress = false;
        private RewardedAd rewardedVideoAd;
        private Action onSuccessRequestRewardedVideo;
        private Action<AdFailReason> onFailureRequestRewardedVideo;
        private Action<AdShownResult> onSuccessShowRewardedVideo;
        private Action<AdFailReason> onFailureShowRewardedVideo;

        private bool isInterstitialRequestedInProgress = false;
        private InterstitialAd interstitialAd;
        private Action onSuccessRequestInterstitial;
        private Action<AdFailReason> onFailureRequestInterstitial;
        private Action onSuccessShowInterstitial;

        private UnityTimeScheduler TimeScheduler => ServiceLocator.Find<UnityTimeScheduler>();
        

        protected override void InternalInitialize(string appId, Action onComplete, Action onFailed)
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
            
            RequestConfiguration requestConfiguration = new RequestConfiguration()
            {
                TagForChildDirectedTreatment = TagForChildDirectedTreatment.Unspecified,
                MaxAdContentRating = MaxAdContentRating.G,
                TagForUnderAgeOfConsent = TagForUnderAgeOfConsent.Unspecified
            };

            MobileAds.SetRequestConfiguration(requestConfiguration);

            MobileAds.Initialize(initStatus =>
            {
                // Callbacks from GoogleMobileAds are not guaranteed to be called on main thread.
                // In this example we use MobileAdsEventExecutor to schedule these calls on the next Update() loop.
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    // The main usage of this callback is for when we use Ad Mediation and want to make sure all
                    // Ad networks initialized
                    onComplete.Invoke();
                });
            });
        }

        public void SetTestMode(bool testMode)
        {
            this.testMode = testMode;
        }

        public override void RequestRewarded(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            if (rewardedVideoAd != null && rewardedVideoAd.CanShowAd())
            {
                onRequestFilled();
                return;
            }

            if (isRewardedRequestInProgress)
            {
                onFailed(AdFailReason.RequestIsAlreadyInProgress);
                return;
            }

            isRewardedRequestInProgress = true;

            onSuccessRequestRewardedVideo = onRequestFilled;
            onFailureRequestRewardedVideo = onFailed;

            CreateRewardedVideoAd();
        }

        public override void RequestInterstitial(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                onRequestFilled();
                return;
            }

            if (isInterstitialRequestedInProgress)
            {
                onFailed(AdFailReason.None);
                return;
            }

            CheckToDestroyInterstitialAd();

            isInterstitialRequestedInProgress = true;

            onSuccessRequestInterstitial = onRequestFilled;
            onFailureRequestInterstitial = onFailed;

            CreateInterstitialAd();
        }

        public override void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            if (rewardedVideoAd != null && rewardedVideoAd.CanShowAd())
            {
                onSuccessShowRewardedVideo = onShown;
                onFailureShowRewardedVideo = onFailed;
                rewardedVideoAd.Show((Reward reward) =>
                {
                    TimeScheduler.Schedule(Time.deltaTime, () => HandleUserEarnedReward(reward), this);
                });
            }
            else
            {
                onFailed.Invoke(AdFailReason.NoAdReady);
            }
        }

        public override void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed)
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                onSuccessShowInterstitial = onShown;
                interstitialAd.Show();
            }
            else
            {
                onFailed.Invoke(AdFailReason.NoAdReady);
            }
        }

        public override bool IsFraudSensitive()
        {
            return true;
        }


        // ---------------------------------------- Helper Methods ---------------------------------------- \\

        private void CreateRewardedVideoAd()
        {
            string zoneId = testMode ? RewardedTestVideoZoneId : rewardedZoneId;

            RewardedAd videoAd = null;
            RewardedAd.Load(zoneId, new AdRequest(),
                (RewardedAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null || ad == null)
                    {
                        TimeScheduler.Schedule(Time.deltaTime, () => HandleRewardedAdFailedToLoad(loadError), this);
                        return;
                    }
                    
                    videoAd = ad;
                    videoAd.OnAdFullScreenContentFailed += (err) => TimeScheduler.Schedule(Time.deltaTime, () => HandleRewardedAdFailedToShow(err), this);
                    videoAd.OnAdFullScreenContentClosed += () => TimeScheduler.Schedule(Time.deltaTime, HandleRewardedAdClosed, this);
                    videoAd.OnAdFullScreenContentOpened += () => TimeScheduler.Schedule(Time.deltaTime, HandleRewardedAdOpening, this);
                    
                    SetRewardedVideoAd();
                    HandleRewardedAdLoaded();
                });

            void SetRewardedVideoAd() => rewardedVideoAd = videoAd;
        }

        private void CreateInterstitialAd()
        {
            string zoneId = testMode ? InterstitialTestZoneId : interstitialZoneId;
            
            InterstitialAd createInterstitialAd = null;
            InterstitialAd.Load(zoneId, new AdRequest(),
                (InterstitialAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null || ad == null)
                    {
                        HandleOnInterstitialAdFailedToLoad(loadError);
                        return;
                    }
                    
                    createInterstitialAd = ad;
                    createInterstitialAd.OnAdFullScreenContentClosed += HandleOnInterstitialAdClosed;

                    SetInterstitialVideoAd();
                    HandleOnInterstitialAdLoaded();
                });
            
            void SetInterstitialVideoAd() => interstitialAd = createInterstitialAd;
        }

        private void CheckToDestroyInterstitialAd()
        {
            interstitialAd?.Destroy();
        }

        // ---------------------------------------------------------------------------------------------------------- \\

        // ---------------------------------------- Rewarded video Ad Events ---------------------------------------- \\

        private void HandleRewardedAdFailedToLoad(LoadAdError e)
        {
            onFailureRequestRewardedVideo(AdFailReason.None);
            isRewardedRequestInProgress = false;
            DebugPro.LogError<AdvertisementLogTag>($"AdMob | load ad failed, Reason: {e.GetMessage()}");
        }

        private void HandleRewardedAdLoaded()
        {
            onSuccessRequestRewardedVideo();
            isRewardedRequestInProgress = false;
        }

        private void HandleRewardedAdOpening()
        {
            adRewarded = false;
        }

        private void HandleRewardedAdClosed()
        {
            try
            {
                if (!adRewarded)
                    onSuccessShowRewardedVideo(AdShownResult.Skipped);
            }
            catch (Exception exception)
            {
                Debug.LogError($"AdMob Rewarded Closed Error, Error: {exception.Message}  --  Stack: {exception.StackTrace}");
            }
        }

        private void HandleUserEarnedReward(Reward e)
        {
            try
            {
                adRewarded = true;
                onSuccessShowRewardedVideo?.Invoke(AdShownResult.Completed);
            }
            catch (Exception exception)
            {
                Debug.LogError($"AdMob Rewarding Error, Error: {exception.Message}  --  Stack: {exception.StackTrace}");
            }
        }

        private void HandleRewardedAdFailedToShow(AdError e)
        {
            Debug.LogError($"AdMob ad failed to show, Reason: {e.GetMessage()}");
            onFailureShowRewardedVideo?.Invoke(AdFailReason.UnknownError);
        }


        // ---------------------------------------------------------------------------------------------------------- \\

        // ----------------------------------------- Interstitial Ad Events ----------------------------------------- \\

        private void HandleOnInterstitialAdFailedToLoad(LoadAdError e)
        {
            onFailureRequestInterstitial(AdFailReason.UnknownError);
            isInterstitialRequestedInProgress = false;
            Debug.LogError($"Admob Interstitial Ad Failed To Load, Reason: {e.GetMessage()}");
        }

        private void HandleOnInterstitialAdLoaded()
        {
            onSuccessRequestInterstitial();
            isInterstitialRequestedInProgress = false;
        }

        private void HandleOnInterstitialAdClosed()
        {
            onSuccessShowInterstitial();
            interstitialAd.Destroy();
        }

        // ---------------------------------------------------------------------------------------------------------- \\
    }

    #else

    [Preserve]
    public class AdMobAdvertisementPlayer : AdvertisementPlayer
    {
        public void SetTestMode(bool testMode) { }

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

    #endif
}