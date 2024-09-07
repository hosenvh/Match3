using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Service.Handlers.Base;


namespace Match3.Overlay.Advertisement.Service.Handlers
{
    public class InterstitialAdvertisementHandler : Base.AdvertisementHandler
    {
        private const float TIMEOUT_DURATION = 2f;

        public override float GetTimeoutDuration()
        {
            return TIMEOUT_DURATION;
        }

        protected override void RequestAdInternal(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            advertisementPlayer.RequestInterstitial(onRequestFilled, onFailed);
        }

        protected override void ShowAdInternal(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            advertisementPlayer.ShowInterstitial(onShown: () => onShown.Invoke(AdShownResult.Completed), onFailed);
        }

        public override bool ShouldShowSessionPresentation()
        {
            return false;
        }
    }
}