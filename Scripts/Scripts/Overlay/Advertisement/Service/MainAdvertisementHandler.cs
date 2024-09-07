using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Players.Base;
using Match3.Overlay.Advertisement.Service.ChoosingPolicy;
using Match3.Overlay.Advertisement.Service.ChoosingPolicy.Base;
using Match3.Overlay.Advertisement.Service.Handlers;
using Match3.Overlay.Advertisement.Service.Session;
using Match3.Utility.GolmoradLogging;


namespace Match3.Overlay.Advertisement.Service
{
    public interface AdvertisementHandler
    {
        void RequestAndShowRewarded(Action onCompletelyShown, Action onSkipped, Action<AdFailReason> onFailed);
        void RequestAndShowInterstitial(Action onShown, Action<AdFailReason> onFailed);
        AdvertisementPlayerBase GetLastUsedAdsPlayer();
        AdvertisementPlayersHandler GetAdvertisementPlayersHandler();
    }

    public class MainAdvertisementHandler : AdvertisementHandler
    {
        private readonly AdvertisementPlayersHandler playersHandler = new AdvertisementPlayersHandler();

        private readonly RewardedAdvertisementHandler rewardedAdvertisementHandler = new RewardedAdvertisementHandler();
        private readonly InterstitialAdvertisementHandler interstitialAdvertisementHandler = new InterstitialAdvertisementHandler();
        private readonly AdvertisementSession advertisementSession = new AdvertisementSession();

        private AdvertisementPlayerChoosingPolicy currentPlayerChoosingPolicy;
        private AdvertisementPlayerBase lastUsedAdvertisementPlayer;


        public MainAdvertisementHandler()
        {
            playersHandler.AddEnabledPlayersBasedOnPlayersData();
            playersHandler.InitializeEnabledPlayersWhichNeedFromStartInitialization();

            UpdateCurrentPolicyBasedOnServerConfig();
            ServiceLocator.Find<ServerConfigManager>().onServerConfigUpdated += data => UpdateCurrentPolicyBasedOnServerConfig();

            rewardedAdvertisementHandler.SetAdvertisementSessionBeingOpenChecker(IsAdvertisementSessionOpen);
            interstitialAdvertisementHandler.SetAdvertisementSessionBeingOpenChecker(IsAdvertisementSessionOpen);
        }

        private bool IsAdvertisementSessionOpen() => advertisementSession.IsOpen;

        private void UpdateCurrentPolicyBasedOnServerConfig()
        {
            var serverConfigManager = ServiceLocator.Find<ServerConfigManager>();
            Type policyType = serverConfigManager.data.config.advertisementPlayersDataContainer_V3.ChoosingPolicyType;

            try
            {
                currentPlayerChoosingPolicy = Activator.CreateInstance(policyType, playersHandler) as AdvertisementPlayerChoosingPolicy;
            }
            catch (Exception exception)
            {
                // Todo: Currently if we can't find/create the advertisementChoosingPolicy, we do fallback to priorityBased, but that's not ideal, maybe to have a list of choosingPolicies on the server side would be better idea
                currentPlayerChoosingPolicy = new PriorityBasedAdvertisementPlayerPolicy(playersHandler);
                DebugPro.LogWarning<AdvertisementLogTag>(message:$"Creating Advertisement choosing policy failed, falling back to  {nameof(PriorityBasedAdvertisementPlayerPolicy)}. Exception: {exception}");
            }
        }

        public void RequestAndShowRewarded(Action onCompletelyShown, Action onSkipped, Action<AdFailReason> onFailed)
        {
            TryRequestAndShowAdvertisement(
                advertisementHandler: rewardedAdvertisementHandler,
                onShown: shownResult =>
                {
                    if (shownResult == AdShownResult.Completed)
                        onCompletelyShown.Invoke();
                    else
                        onSkipped.Invoke();
                },
                onFailed);
        }

        public void RequestAndShowInterstitial(Action onShown, Action<AdFailReason> onFailed)
        {
            TryRequestAndShowAdvertisement(
                advertisementHandler: interstitialAdvertisementHandler,
                onShown: result => { onShown.Invoke(); },
                onFailed);
        }

        private void TryRequestAndShowAdvertisement(Handlers.Base.AdvertisementHandler advertisementHandler, Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            if (IsAdvertisementSessionOpen())
                onFailed.Invoke(AdFailReason.SessionIsAlreadyOpen);
            else
                StartRequestAndShowAdvertisement(advertisementHandler, onShown, onFailed);
        }

        private void StartRequestAndShowAdvertisement(Handlers.Base.AdvertisementHandler advertisementHandler, Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            PrepareSession();
            currentPlayerChoosingPolicy.Resort();

            RequestAndShowAdvertisement(advertisementHandler, onShown: advertisementSession.CloseAsShown, onFailed: advertisementSession.CloseWithFailure);

            void PrepareSession()
            {
                advertisementSession.Setup(onShown, onFailed, advertisementHandler.ShouldShowSessionPresentation());
                advertisementSession.Open();
                ScheduleToCloseSessionOnTimeout();

                void ScheduleToCloseSessionOnTimeout()
                {
                    ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
                    ServiceLocator.Find<UnityTimeScheduler>().Schedule(
                        delay: advertisementHandler.GetTimeoutDuration(),
                        callback: () => { advertisementSession.CloseWithFailure(AdFailReason.TimeOut); },
                        owner: this);
                }
            }
        }

        private void RequestAndShowAdvertisement(Handlers.Base.AdvertisementHandler advertisementHandler, Action<AdShownResult> onShown, Action<AdFailReason> onFailed)
        {
            AdFailReason lastFailedReason = AdFailReason.None;

            RequestAndShowAdvertisementRecursively();

            void RequestAndShowAdvertisementRecursively()
            {
                if (currentPlayerChoosingPolicy.HasAnyPlayerLeft() && IsAdvertisementSessionOpen())
                {
                    var adsPlayer = currentPlayerChoosingPolicy.GetNextPlayerAndAdvance();
                    lastUsedAdvertisementPlayer = adsPlayer;

                    if (AdvertisementFraudHandler.IsFraud(adsPlayer))
                    {
                        onFailed.Invoke(AdFailReason.InvalidAdvertisementPlayer);
                        return;
                    }

                    playersHandler.InitializeAdvertisementPlayer(
                        adsPlayer,
                        onComplete: () =>
                        {
                            advertisementHandler.SetAdvertisementPlayer(adsPlayer);
                            advertisementHandler.RequestAndShow(
                                onShown:result =>
                                {
                                    currentPlayerChoosingPolicy.HandleAdvertisementPlayerUsedSuccessfully(adsPlayer);
                                    onShown.Invoke(result);
                                },
                                onFailed: failReason =>
                                {
                                    lastFailedReason = failReason;
                                    RequestAndShowAdvertisementRecursively();
                                });
                        },
                        onFailed: RequestAndShowAdvertisementRecursively
                    );
                }
                else
                    onFailed.Invoke(lastFailedReason);
            }
        }

        public AdvertisementPlayerBase GetLastUsedAdsPlayer()
        {
            return lastUsedAdvertisementPlayer;
        }

        public AdvertisementPlayersHandler GetAdvertisementPlayersHandler()
        {
            return playersHandler;
        }
    }
}