using System;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;


namespace Match3.Overlay.Advertisement.Players.Implementations
{
    public class MockAdvertisementPlayer : AdvertisementPlayer
    {

        protected override void InternalInitialize(string appId, Action onComplete, Action onFailed)
        {
            onFailed.Invoke();
        }

        public override void RequestRewarded(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            onFailed.Invoke(AdFailReason.InvalidAdvertisementPlayer);
        }

        public override void ShowRewarded(Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            onFailed.Invoke(AdFailReason.InvalidAdvertisementPlayer);
        }

        public override void ShowInterstitial(Action onShown, Action<AdFailReason> onFailed)
        {
            onFailed.Invoke(AdFailReason.InvalidAdvertisementPlayer);
        }

        public override void RequestInterstitial(Action onRequestFilled, Action<AdFailReason> onFailed)
        {
            onFailed.Invoke(AdFailReason.InvalidAdvertisementPlayer);
        }

        public override bool IsFraudSensitive()
        {
            return true;
        }
    }
}