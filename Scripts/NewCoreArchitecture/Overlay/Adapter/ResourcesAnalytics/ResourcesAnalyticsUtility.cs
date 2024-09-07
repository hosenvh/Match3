using Match3.LiveOps.DogTraining.Game;
using Match3.Presentation.Gameplay;
using Match3.Presentation.NeighborhoodChallenge;
using UnityEngine;

namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public static class ResourcesAnalyticsUtility
    {
        public static class LevelType
        {
            public const string Null = "";
            public const string Campaign = "Campaign";
            public const string Neighbourhood = "Neighbour";
            public const string DogTraining = "DogTraining";
        }

        public static string GetCurrentLevelType()
        {
            switch (Base.gameManager.CurrentState)
            {
                case NeighborhoodChallengeGameplayState _:
                    return LevelType.Neighbourhood;
                case CampaignGameplayState _:
                    return LevelType.Campaign;
                case DogTrainingGameplayState _:
                    return LevelType.DogTraining;
                case GameplayState _:
                    return Base.gameManager.CurrentState.name;
            }

            ResourcesAnalyticsLogger.LogError("Getting LevelType while we are not in any gameplay state.");
            return "ResultNone";
        }

        public static string ConvertPowerUpIndexToCurrencyType(int powerUpIndex)
        {
            switch (powerUpIndex)
            {
                case 0: return GameAnalyticsDataProvider.ResourcesCurrencyType.PowerUpHammer;
                case 1: return GameAnalyticsDataProvider.ResourcesCurrencyType.PowerUpBroom;
                case 2: return GameAnalyticsDataProvider.ResourcesCurrencyType.PowerUpHand;
                default: return "UndefinedPowerUp";
            }
        }

        public static int ConvertCurrencyTypeToPowerUpIndex(string resourceCurrencyType)
        {
            switch (resourceCurrencyType)
            {
                case GameAnalyticsDataProvider.ResourcesCurrencyType.PowerUpHammer: return 0;
                case GameAnalyticsDataProvider.ResourcesCurrencyType.PowerUpBroom: return 1;
                case GameAnalyticsDataProvider.ResourcesCurrencyType.PowerUpHand: return 2;
            }
            return -1;
        }

        public static string ConvertBoosterIndexToCurrencyType(int boosterIndex)
        {
            switch (boosterIndex)
            {
                case 0: return GameAnalyticsDataProvider.ResourcesCurrencyType.BoosterBomb;
                case 1: return GameAnalyticsDataProvider.ResourcesCurrencyType.BoosterRainbow;
                case 2: return GameAnalyticsDataProvider.ResourcesCurrencyType.BoosterTntRainbow;
                default: return "UndefinedBooster";
            }
        }

        public static int ConvertCurrencyTypeToBoosterIndex(string resourceCurrencyType)
        {
            switch (resourceCurrencyType)
            {
                case GameAnalyticsDataProvider.ResourcesCurrencyType.BoosterBomb: return 0;
                case GameAnalyticsDataProvider.ResourcesCurrencyType.BoosterRainbow: return 1;
                case GameAnalyticsDataProvider.ResourcesCurrencyType.BoosterTntRainbow: return 2;
            }
            return -1;
        }
    }
}