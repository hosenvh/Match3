using System;
using GameScripts.States;
using I2.Loc;
using Match3.Overlay.Advertisement.Placements;
using Match3.Foundation.Base.ServiceLocating;
using Match3.LevelInfoAds.RewardHandler;
using Match3.Overlay.Advertisement.Placements.Implementations;
using static Popup_ConfirmBox;
using Random = UnityEngine.Random;

namespace Match3.LevelInfoAds
{
    public class LevelInfoRewaredAdsHandler
    {
        public void TryPlayRewardedAds(Action onComplete)
        {
            if (IsAdsAvailable())
                ShowAds(onComplete);
            else
                onComplete();
        }

        private bool IsAdsAvailable()
        {
            return ServiceLocator.Find<AdvertisementPlacementsManager>().IsAvailable<LevelInfoAdPlacement>();
        }

        private void ShowAds(Action onComplete)
        {
            LevelInfoAdsRewardHandler selectedRewardHandler = SelectRewardHandler();

            var suggestionPopup = OpenAdsSuggestionPopup();
            SetupAdsSuggestionPopup(
                suggestionPopup,
                onConfirm: () => PlayAds(
                    onSuccessfullyShown: suggestionPopup.Close,
                    adPlacementArgument: new LevelInfoAdPlacementArgument(selectedRewardHandler, onRewardConfirmed: onComplete)),
                onNotConfirm: onComplete);
        }

        private Popup_AdsConfirmBox OpenAdsSuggestionPopup()
        {
            return Base.gameManager.OpenPopup<Popup_AdsConfirmBox>();
        }

        private void SetupAdsSuggestionPopup(Popup_AdsConfirmBox popup, Action onConfirm, Action onNotConfirm)
        {
            var texts = new ConfirmPopupTexts(
                message: ScriptLocalization.UI_LevelInfo.DoYouWantToWatchAdToGetBoosterOrPowerup,
                confirmText: ScriptLocalization.Message_Tasks.WatchVideo,
                cancelText: ScriptLocalization.UI_General.Cancel2);

            popup.Setup(
                texts,
                closeOnConfirm: false,
                onResult: confirm =>
                {
                    if (confirm)
                        onConfirm.Invoke();
                    else
                        onNotConfirm.Invoke();
                });
        }

        private void PlayAds(Action onSuccessfullyShown, LevelInfoAdPlacementArgument adPlacementArgument)
        {
            ServiceLocator.Find<AdvertisementPlacementsManager>().Play<LevelInfoAdPlacement>(
                argument: adPlacementArgument,
                onSuccess: onSuccessfullyShown,
                onFailure: delegate { });
        }

        private LevelInfoAdsRewardHandler SelectRewardHandler()
        {
            int random = Random.Range(0, 2);
            if (random == 0)
                return new LevelInfoAdsBoosterRewardHandler();
            else
                return new LevelInfoAdsPowerupRewardHandler();
        }
    }
}