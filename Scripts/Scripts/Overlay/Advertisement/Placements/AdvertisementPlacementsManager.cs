using System;
using System.Collections.Generic;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.Utility;
using Match3.Overlay.Advertisement.Base;
using Match3.Overlay.Advertisement.Overlay;
using Match3.Overlay.Advertisement.Placements.Base;
using Match3.Overlay.Advertisement.Service;
using Match3.Utility.GolmoradLogging;


namespace Match3.Overlay.Advertisement.Placements
{
    public interface AdvertisementPlacementsAnalyticsPort
    {
        void HandleRequestStart(AdvertisementPlacement adPlacement);
        void HandleAdRequestOrShowFailure(AdvertisementPlacement adPlacement, AdFailReason requestResult);
        void HandleAdCompletelyShownResult(AdvertisementPlacement adPlacement);
        void HandleAdSkippedResult(AdvertisementPlacement adPlacement);
    }


    // TODO: Make a better abstraction handling paying and non paying users.
    // TODO: Try find a better way to handle paying users and non paying users
    // TODO: Move out the disability responsibility
    public class AdvertisementPlacementsManager : Foundation.Base.ServiceLocating.Service, EventListener
    {
        struct RequestData
        {
            public readonly AdvertisementPlacement adPlacement;
            public readonly Argument argument;
            public readonly Action onSuccess;
            public readonly Action onFailure;

            public RequestData(AdvertisementPlacement adPlacement, Argument argument, Action onSuccess, Action onFailure)
            {
                this.adPlacement = adPlacement;
                this.argument = argument;
                this.onSuccess = onSuccess;
                this.onFailure = onFailure;
            }
        }

        private readonly Dictionary<Type, AdvertisementPlacement> nonPayingUserPlacements = new Dictionary<Type, AdvertisementPlacement>();
        private readonly Dictionary<Type, AdvertisementPlacement> payingUserPlacements = new Dictionary<Type, AdvertisementPlacement>();

        private AdvertisementHandler advertisementHandler;
        private AdvertisementPlacementsAnalyticsPort analyticsPort;

        public AdvertisementPlacementsManager(EventManager eventManager, ConfigurationManager configurationManager)
        {
            eventManager.Register(this);
        }

        public void Initialize()
        {
            this.advertisementHandler = new MainAdvertisementHandler();
            this.analyticsPort = new MainAdvertisementPlacementsAnalyticsPort(advertisementHandler);
        }

        public bool IsAvailable<T>() where T : AdvertisementPlacement
        {
            var placement = Find<T>();
            if (placement != null)
                return placement.IsAvailable();
            else
            {
                DebugPro.LogError<AdvertisementLogTag>($"Placement {typeof(T)} is not defined.");
                return false;
            }
        }

        public void Play<T>(Argument argument, Action onSuccess, Action onFailure) where T : AdvertisementPlacement
        {
            var placement = Find<T>();

            var requestData = new RequestData(placement, argument, onSuccess, onFailure);

            PlayAdvertisementFor(placement, requestData);
            analyticsPort.HandleRequestStart(placement);
        }

        private void PlayAdvertisementFor(AdvertisementPlacement placement, RequestData requestData)
        {
            switch (placement.AdvertisementType())
            {
                case AdvertisementPlacementType.Rewarded:
                    advertisementHandler.RequestAndShowRewarded(
                        onCompletelyShown: () => HandleAdCompletelyShownResult(requestData),
                        onSkipped: () => HandleAdSkippedResult(requestData),
                        onFailed: requestResult => HandleRequestFailure(requestData, requestResult));
                    break;
                case AdvertisementPlacementType.Interstitial:
                    advertisementHandler.RequestAndShowInterstitial(
                        onShown: () => HandleAdCompletelyShownResult(requestData),
                        onFailed: (requestResult) => HandleRequestFailure(requestData, requestResult));
                    break;
            }
        }

        private void HandleAdCompletelyShownResult(RequestData requestData)
        {
            requestData.adPlacement.Execute(requestData.argument);
            requestData.onSuccess.Invoke();
            analyticsPort.HandleAdCompletelyShownResult(requestData.adPlacement);
        }

        private void HandleAdSkippedResult(RequestData requestData)
        {
            requestData.onFailure.Invoke();
            analyticsPort.HandleAdSkippedResult(requestData.adPlacement);
        }

        private void HandleRequestFailure(RequestData requestData, AdFailReason requestResult)
        {
            requestData.onFailure.Invoke();
            analyticsPort.HandleAdRequestOrShowFailure(requestData.adPlacement, requestResult);
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (IsPayingUser())
            {
                foreach (var placement in payingUserPlacements.Values)
                    placement.UpdateInternalSateBasedOn(evt);
            }
            else
            {
                foreach (var placement in nonPayingUserPlacements.Values)
                    placement.UpdateInternalSateBasedOn(evt);
            }
        }

        public void AddPlacementForNonPayingUser(AdvertisementPlacement advertismentPlacement)
        {
            nonPayingUserPlacements[advertismentPlacement.GetType()] = advertismentPlacement;
        }

        public void AddPlacementForPayingUsers(AdvertisementPlacement advertismentPlacement)
        {
            payingUserPlacements[advertismentPlacement.GetType()] = advertismentPlacement;
        }

        // NOTE: This only works with the exact type, the data structure must change if this does not apply.
        public T Find<T>() where T : AdvertisementPlacement
        {
            AdvertisementPlacement placement;

            if (IsPayingUser())
                payingUserPlacements.TryGetValue(typeof(T), out placement);
            else
                nonPayingUserPlacements.TryGetValue(typeof(T), out placement);

            return (T) placement;
        }

        private bool IsPayingUser()
        {
            var gameManager = global::Base.gameManager;
            return gameManager != null ? gameManager.profiler.PurchaseCount > 0 : false;
        }

        public AdvertisementHandler GetAdvertisementHandler()
        {
            return advertisementHandler;
        }
    }
}