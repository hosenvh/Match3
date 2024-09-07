using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Service.Session;
using Match3.Utility.GolmoradLogging;
using SeganX;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Service.Handlers.Base
{
    public abstract class AdvertisementHandler
    {
        protected AdvertisementPlayerBase advertisementPlayer;
        private Func<bool> isAdvertisementSessionOpen;

        public abstract float GetTimeoutDuration();

        public void SetAdvertisementSessionBeingOpenChecker(Func<bool> isAdvertisementSessionOpen)
        {
            this.isAdvertisementSessionOpen = isAdvertisementSessionOpen;
        }

        public void SetAdvertisementPlayer(AdvertisementPlayerBase advertisementPlayer)
        {
            this.advertisementPlayer = advertisementPlayer;
        }

        public void RequestAndShow(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            #if UNITY_EDITOR
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(
                delay: 1,
                callback: () => onShown.Invoke(AdShownResult.Completed),
                owner: this);
            return;
            #endif
            #pragma warning disable 0162

            RequestAd(
                onRequestFilled: () => { ShowAd(onShown, onFailed); },
                onFailed);

            #pragma warning restore 0162
        }

        private void RequestAd(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            if (isAdvertisementSessionOpen.Invoke() == false)
                onFailed.Invoke(AdFailReason.SessionIsNotOpen);
            else if (Utilities.IsConnectedToInternet() == false)
                onFailed.Invoke(AdFailReason.NoConnection);
            else
            {
                DebugPro.LogInfo<AdvertisementLogTag>($"Handler | Requesting From: {advertisementPlayer.GetType().Name}");
                try
                {
                    RequestAdInternal(onRequestFilled, onFailed);
                }
                catch (Exception exception)
                {
                    DebugPro.LogError<AdvertisementLogTag>(message: $"Request Ads Failed {exception}");
                    onFailed.Invoke(AdFailReason.UnknownError);
                }
            }
        }

        protected abstract void RequestAdInternal(Action onRequestFilled, Action<AdFailReason> onFailed);

        private void ShowAd(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            if (isAdvertisementSessionOpen.Invoke() == false)
                onFailed.Invoke(AdFailReason.SessionIsNotOpen);
            else
            {
                DebugPro.LogInfo<AdvertisementLogTag>($"Handler | Showing From: {advertisementPlayer.GetType().Name}");
                try
                {
                    ShowAdInternal(onShown, onFailed);
                }
                catch (Exception exception)
                {
                    DebugPro.LogError<AdvertisementLogTag>(message: $"Show Ads Failed {exception}");
                    onFailed.Invoke(AdFailReason.UnknownError);
                }
            }
        }

        protected abstract void ShowAdInternal(Action<AdShownResult> onShown, Action<AdFailReason> onFailed);

        public abstract bool ShouldShowSessionPresentation();
    }
}