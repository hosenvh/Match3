using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Overlay.Advertisement.Base;
using Match3.Utility.GolmoradLogging;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Service.Session
{
    public class AdvertisementSession
    {
        public class AdvertisementSessionOpenEvent : GameEvent
        {
        }

        public class AdvertisementSessionCloseEvent : GameEvent
        {
        }

        public bool IsOpen { get; private set; }

        private Action<AdShownResult> onShown;
        private Action<AdFailReason> onFailed;


        private readonly AdvertisementSessionPresentation presentation = new AdvertisementSessionPresentation();

        public void Setup(Action<AdShownResult> onShown, Action<AdFailReason> onFailed, bool shouldShowSessionPresentation)
        {
            this.onShown = onShown;
            this.onFailed = onFailed;
            presentation.Setup(shouldShowSessionPresentation);
        }

        public void Open()
        {
            DebugPro.LogInfo<AdvertisementLogTag>("Session | Opening");
            IsOpen = true;
            presentation.OpenWaitingPopup();
            ServiceLocator.Find<EventManager>().Propagate(new AdvertisementSessionOpenEvent(), this);
        }

        public void CloseAsShown(AdShownResult shownResult)
        {
            DebugPro.LogInfo<AdvertisementLogTag>("Session | Close As Shown");
            CloseSession(toCallOnClosing: () =>
            {
                onShown.Invoke(shownResult);
                if (shownResult == AdShownResult.Skipped)
                    presentation.ShowPopupForRewardedSkipped();
            });
        }

        public void CloseWithFailure(AdFailReason reason)
        {
            if (reason != AdFailReason.NoConnection && reason != AdFailReason.TimeOut)
                DebugPro.LogWarning<AdvertisementLogTag>($"Session | Close With Failure | Reason: {reason}");
            CloseSession(toCallOnClosing: () =>
            {
                onFailed.Invoke(reason);
                presentation.ShowFailurePopup(reason);
            });
        }

        private void CloseSession(Action toCallOnClosing)
        {
            if (IsOpen == false)
                return;

            IsOpen = false;
            presentation.CloseWaitingPopup();
            toCallOnClosing.Invoke();
            ServiceLocator.Find<EventManager>().Propagate(new AdvertisementSessionCloseEvent(), this);
        }
    }
}