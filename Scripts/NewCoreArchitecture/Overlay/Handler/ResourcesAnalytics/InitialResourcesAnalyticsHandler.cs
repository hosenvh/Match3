using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Game.NeighborhoodChallenge;
using Match3.Overlay.Analytics.ResourcesAnalytics;
using SeganX;
using UnityEngine;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics
{
    public class InitialResourcesAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private static class UserInitialResourcesType
        {
            public const string FirstSession = "FirstSession";
            public const string OldUser = "OldUser";
        }

        private const string INITIAL_RESOURCES_HANDLED_KEY = "InitialResourcesHandledKey";
        private readonly UserProfileManager profileManager;
        private readonly GameProfiler userProfile;
        private readonly NeighborhoodChallengeManager neighborhoodChallengeManager;
        private readonly bool isFirstSession;
        private readonly Action<string, float> onResourcesDataIsReadyToBeSent;

        private bool IsInitialResourcesAlreadyHandled
        {
            get => profileManager.LoadData(INITIAL_RESOURCES_HANDLED_KEY, 0) == 1;
            set => profileManager.SaveData(INITIAL_RESOURCES_HANDLED_KEY, value ? 1 : 0);
        }

        public InitialResourcesAnalyticsHandler(UserProfileManager profileManager, GameProfiler userProfile, NeighborhoodChallengeManager neighborhoodChallengeManager, bool isFirstSession, ResourceAnalyticsHandlerActions actions, Action<string, float> onResourcesDataIsReadyToBeSent)
            : base(ResourcesItemType.Misc, actions)
        {
            this.profileManager = profileManager;
            this.userProfile = userProfile;
            this.neighborhoodChallengeManager = neighborhoodChallengeManager;
            this.isFirstSession = isFirstSession;
            this.onResourcesDataIsReadyToBeSent = onResourcesDataIsReadyToBeSent;

            TryToHandleInitialResources();
        }

        private void TryToHandleInitialResources()
        {
            if (!IsInitialResourcesAlreadyHandled)
                HandleInitialResources();
        }

        private void HandleInitialResources()
        {
            SendAllInitialResourcesAnalytics();
            FinishInitialResourcesHandling();
        }

        private void FinishInitialResourcesHandling()
        {
            IsInitialResourcesAlreadyHandled = true;
        }

        private void SendAllInitialResourcesAnalytics()
        {
            string userInitialResourcesType = isFirstSession ? UserInitialResourcesType.FirstSession : UserInitialResourcesType.OldUser;

            OpenPort(itemId: userInitialResourcesType);
            foreach (string resourcesCurrencyType in GetAllResourceCurrencyTypes())
                SendInitialResourceAnalyticsOf(resourcesCurrencyType);
            ClosePort(itemId: userInitialResourcesType);
        }

        private void SendInitialResourceAnalyticsOf(string resourceCurrencyType)
        {
            int amount = GetResourceCurrentAmount(resourcesCurrencyType: resourceCurrencyType);
            onResourcesDataIsReadyToBeSent.Invoke(resourceCurrencyType, amount);
        }

        private int GetResourceCurrentAmount(string resourcesCurrencyType)
        {
            switch (resourcesCurrencyType)
            {
                case ResourcesCurrencyType.Coin: return userProfile.CoinCount;
                case ResourcesCurrencyType.Star: return userProfile.StarCount;
                case ResourcesCurrencyType.Key: return userProfile.KeyCount;

                case ResourcesCurrencyType.Life: return Mathf.Max(userProfile.LifeCount, 5);
                case ResourcesCurrencyType.Ticket: return neighborhoodChallengeManager.Ticket().CurrentValue();

                case ResourcesCurrencyType.PowerUpHammer:
                case ResourcesCurrencyType.PowerUpBroom:
                case ResourcesCurrencyType.PowerUpHand:
                    return userProfile.GetPowerupCount(ResourcesAnalyticsUtility.ConvertCurrencyTypeToPowerUpIndex(resourcesCurrencyType));

                case ResourcesCurrencyType.BoosterBomb:
                case ResourcesCurrencyType.BoosterRainbow:
                case ResourcesCurrencyType.BoosterTntRainbow:
                    return userProfile.GetBoosterCount(ResourcesAnalyticsUtility.ConvertCurrencyTypeToBoosterIndex(resourcesCurrencyType));

                default:
                    ResourcesAnalyticsLogger.LogError(message: $"Resource Currency Type {resourcesCurrencyType} not found. Initial Analytics Resource Handler Bug");
                    break;
            }

            return 0;
        }

        private IEnumerable<string> GetAllResourceCurrencyTypes()
        {
            return new List<string>
            {
               ResourcesCurrencyType.Coin,
               ResourcesCurrencyType.Life,
               ResourcesCurrencyType.Ticket,
               ResourcesCurrencyType.Star,
               ResourcesCurrencyType.Key,

               ResourcesCurrencyType.PowerUpBroom,
               ResourcesCurrencyType.PowerUpHand,
               ResourcesCurrencyType.PowerUpHammer,

               ResourcesCurrencyType.BoosterBomb,
               ResourcesCurrencyType.BoosterRainbow,
               ResourcesCurrencyType.BoosterTntRainbow
            };
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
        }

        protected override void Handle(GameEvent evt)
        {
        }
    }
}