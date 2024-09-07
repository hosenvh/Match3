using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Service.Handlers.Base;


namespace Match3.Overlay.Advertisement.Service.Handlers
{
    public class RewardedAdvertisementHandler : Base.AdvertisementHandler
    {
        private const float TIMEOUT_DURATION = 10f;

        public override float GetTimeoutDuration()
        {
            return TIMEOUT_DURATION;
        }

        protected override void RequestAdInternal(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            advertisementPlayer.RequestRewarded(onRequestFilled, onFailed);
        }

        protected override void ShowAdInternal(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            advertisementPlayer.ShowRewarded(onShown, onFailed);
        }

        public override bool ShouldShowSessionPresentation()
        {
            return true;
        }
    }
}