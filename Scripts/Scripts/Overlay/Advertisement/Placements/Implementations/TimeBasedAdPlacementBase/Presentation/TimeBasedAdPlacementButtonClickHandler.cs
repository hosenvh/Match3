using System;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.Utility;
using Match3.Presentation.HUD;
using SeganX;
using UnityEngine;
using UnityEngine.UI;
using static Base;


namespace Match3.Overlay.Advertisement.Placements.Implementations.TimeBasedAdPlacementBase.Presentation
{
    public class TimeBasedAdPlacementButtonClickHandler<TAdPlacement> where TAdPlacement : TimeBasedAdPlacement
    {
        private readonly HudPresentationController hudPresentationController;
        private readonly ITimeManager timeManager;
        private readonly AdvertisementPlacementsManager advertisementPlacementsManager;
        private readonly Action onAdsShownSuccessfully;

        private TAdPlacement AdPlacement => ServiceLocator.Find<AdvertisementPlacementsManager>().Find<TAdPlacement>();

        public TimeBasedAdPlacementButtonClickHandler(Button unityButton, HudPresentationController hudPresentationController, Action onAdsShownSuccessfully)
        {
            this.hudPresentationController = hudPresentationController;
            this.onAdsShownSuccessfully = onAdsShownSuccessfully;
            this.timeManager = ServiceLocator.Find<ITimeManager>();
            this.advertisementPlacementsManager = ServiceLocator.Find<AdvertisementPlacementsManager>();

            unityButton.onClick.AddListener(HandleAdsShowRequested);
        }

        private void HandleAdsShowRequested()
        {
            if (Utilities.IsConnectedToInternet())
                HandleAdsShowRequestedWhileInternetIsConnect();
            else
                ShowSingleButtonConfirmPopup(message: ScriptLocalization.Message_Network.AskForInternet);
        }

        private void HandleAdsShowRequestedWhileInternetIsConnect()
        {
            GameState waitingPopup = ShowWaitingPopup();
            TryToUpdateTimeWithServer(
                onComplete: () =>
                {
                    waitingPopup.Close();
                    HandleAdsShowRequestedWhileServerTimeUpdateIsTried();
                });

            void TryToUpdateTimeWithServer(Action onComplete)
            {
                timeManager.TrySyncTimeWithServer(onComplete);
            }

            GameState ShowWaitingPopup()
            {
                return gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);
            }
        }

        private void HandleAdsShowRequestedWhileServerTimeUpdateIsTried()
        {
            if (IsAdvertisementAvailable())
                ShowAds();
            else
                HandleAdsShowRequestedWhileAdIsNotAvailable();

            bool IsAdvertisementAvailable() => advertisementPlacementsManager.IsAvailable<TAdPlacement>();
        }

        private void ShowAds()
        {
            if (ShouldShowConfirmation())
                ShowPopupForConfirmation(onConfirm: RequestAndShowAd);
            else
                RequestAndShowAd();

            bool ShouldShowConfirmation() => ServiceLocator.Find<MarketManager>().ShowVideoAdConfirmBox;

            void ShowPopupForConfirmation(Action onConfirm)
            {
                gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                    messageString: ScriptLocalization.Message_Advertisement.AskToWatchVideo,
                    confirmString: ScriptLocalization.UI_General.Yes,
                    cancelString: ScriptLocalization.UI_General.No, closeOnConfirm: true,
                    onResult: accept =>
                    {
                        if (accept)
                            onConfirm.Invoke();
                    });
            }

            void RequestAndShowAd()
            {
                advertisementPlacementsManager.Play<TAdPlacement>(
                    new EmptyArgument(),
                    onSuccess: HandleAdsShownSuccessfully,
                    onFailure: delegate { });

                void HandleAdsShownSuccessfully()
                {
                    onAdsShownSuccessfully.Invoke();
                    PresentAdsReward();
                }

                void PresentAdsReward()
                {
                    gameManager.OpenPopup<Popup_ClaimReward>()
                               .Setup(AdPlacement.Rewards)
                               .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                               .SetOnComplete(() => { ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), this); })
                               .SetAdditionalDelayBeforeClosing(1)
                               .StartPresentingRewards();
                }
            }
        }

        private void HandleAdsShowRequestedWhileAdIsNotAvailable()
        {
            if (IsPlacementAvailable())
                ShowPopupForPlacementIsAvailableButAdvertisementIsNotAvailable();
            else
                HandleAdsShowRequestedWhilePlacementIsNotAvailable();

            bool IsPlacementAvailable() => AdPlacement.IsAvailable();

            void ShowPopupForPlacementIsAvailableButAdvertisementIsNotAvailable()
            {
                ShowSingleButtonConfirmPopup(message: ScriptLocalization.Message_Advertisement.NoAvailableVideo);
            }
        }

        private void HandleAdsShowRequestedWhilePlacementIsNotAvailable()
        {
            if (AreConditionsElseThanTimeSatisfied())
                ShowPopupForRemainingTime();
            else
                ShowPopupForAdPlacementInNotReady();

            bool AreConditionsElseThanTimeSatisfied() => AdPlacement.AreConditionsElseThanTimeSatisfied();

            void ShowPopupForAdPlacementInNotReady()
            {
                ShowSingleButtonConfirmPopup(message: ScriptLocalization.Message_MainMenuAd.AdPlacementNonTimeBasedConditionsAreNotSatisfied);
            }
        }

        private void ShowPopupForRemainingTime()
        {
            int remainingTimeInSeconds = Mathf.CeilToInt((float) AdPlacement.GetRemainingTimeTillNextPossibleAvailability().TotalSeconds);
            string message = CreateMessageBasedOnRemainingTime();
            ShowSingleButtonConfirmPopup(message);

            string CreateMessageBasedOnRemainingTime()
            {
                float hours = remainingTimeInSeconds / 3600f;
                return hours > 1 ? string.Format(ScriptLocalization.Message_MainMenuAd.HoursLeftToAvailability, Mathf.RoundToInt(hours)) : string.Format(ScriptLocalization.Message_MainMenuAd.MinutesLeftToAvailability, Mathf.Max(1, Mathf.RoundToInt(remainingTimeInSeconds / 60f)));
            }
        }

        private void ShowSingleButtonConfirmPopup(string message)
        {
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                message,
                confirmString: ScriptLocalization.UI_General.Ok,
                cancelString: null,
                closeOnConfirm: true);
        }
    }
}