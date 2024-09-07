using System.Collections;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation;
using SeganX;
using UnityEngine;


namespace Match3.Overlay.Advertisement.Placements.Implementations.MainMenuAdPlacement.Presentation
{
    public class AdsMainMenuButtonDisplayHandler
    {
        private readonly GameObject visualForIsReady;
        private readonly RemainedTimePresenter remainedTimePresenter;
        private readonly AdvertisementPlacementsManager advertisementPlacementsManager;

        private MainMenuAdPlacement AdPlacement => ServiceLocator.Find<AdvertisementPlacementsManager>().Find<MainMenuAdPlacement>();

        public AdsMainMenuButtonDisplayHandler(GameObject visualForIsReady, RemainedTimePresenter remainedTimePresenter)
        {
            this.visualForIsReady = visualForIsReady;
            this.remainedTimePresenter = remainedTimePresenter;
            this.advertisementPlacementsManager = ServiceLocator.Find<AdvertisementPlacementsManager>();
        }

        public IEnumerator UpdateDisplayOnRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(1);
            while (true)
            {
                UpdateDisplay();
                yield return wait;
            }
        }

        public void UpdateDisplay()
        {
            if (IsInternetConnected())
                UpdateDisplayWhileInternetIsConnected();
            else
                GoToModeForNoInternet();

            bool IsInternetConnected() => Utilities.IsConnectedToInternet();

            void GoToModeForNoInternet()
            {
                SetupDisplayForNotAvailableMode();
            }
        }

        private void UpdateDisplayWhileInternetIsConnected()
        {
            if (IsAdvertisementAvailable())
                GotoModeForReady();
            else
                UpdateDisplayWhileAdIsNotAvailable();

            bool IsAdvertisementAvailable() => advertisementPlacementsManager.IsAvailable<MainMenuAdPlacement>();

            void GotoModeForReady()
            {
                SetupDisplayForReadyMode();
            }
        }

        private void UpdateDisplayWhileAdIsNotAvailable()
        {
            if (IsPlacementAvailable())
                GoToModeForPlacementIsAvailableButAdvertisementIsNotAvailableMode();
            else
                UpdateDisplayWhilePlacementIsNotAvailable();

            bool IsPlacementAvailable() => AdPlacement.IsAvailable();

            void GoToModeForPlacementIsAvailableButAdvertisementIsNotAvailableMode()
            {
                SetupDisplayForNotAvailableMode();
            }
        }

        private void UpdateDisplayWhilePlacementIsNotAvailable()
        {
            if (AreConditionsElseThanTimeSatisfied())
                GotoTimerMode();
            else
                GotoAdPlacementIsNotReadyMode();

            bool AreConditionsElseThanTimeSatisfied() => AdPlacement.AreConditionsElseThanTimeSatisfied();

            void GotoTimerMode()
            {
                SetupDisplayForTimerMode();
            }

            void GotoAdPlacementIsNotReadyMode()
            {
                SetupDisplayForNotAvailableMode();
            }
        }

        private void SetupDisplayForNotAvailableMode()
        {
            visualForIsReady.gameObject.SetActive(false);
            remainedTimePresenter.ForceStop();
        }

        private void SetupDisplayForReadyMode()
        {
            visualForIsReady.gameObject.SetActive(true);
            remainedTimePresenter.ForceStop();
        }

        private void SetupDisplayForTimerMode()
        {
            if (remainedTimePresenter.IsTimerStarted)
                return;
            visualForIsReady.gameObject.SetActive(false);
            remainedTimePresenter.StartTimer( getRemainingTimeFunc: () => AdPlacement.GetRemainingTimeTillNextPossibleAvailability());
        }
    }
}