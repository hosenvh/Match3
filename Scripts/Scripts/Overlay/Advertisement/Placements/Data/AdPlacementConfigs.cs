using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Data;
using Match3.Game;
using Match3.LuckySpinner.AdsBased.Game;
using Match3.Overlay.Advertisement.Placements.Base;
using Match3.Overlay.Advertisement.Placements.Implementations;
using Match3.Overlay.Advertisement.Placements.Implementations.MainMenuAdPlacement;
using Match3.Overlay.Advertisement.Placements.Implementations.TaskPopupAdPlacement;


namespace Match3.Overlay.Advertisement.Placements.Data
{
    public partial class AdvertisementPlacementsConfigurer
    {
        [Serializable]
        abstract class AdPlacementConfig
        {
            public int maxPlaysInDay = 3;
            public int availabilityLevel = 0;

            public abstract AdvertisementPlacement CreateAdPlacement();
        }

        [Serializable]
        class LifePopupConfig : AdPlacementConfig
        {
            public int livesToGiveCount = 3;
            public int totalNeededZeroLives = 3;

            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new LifePopupAdPlacement(livesToGiveCount, totalNeededZeroLives, availabilityLevel, maxPlaysInDay);
            }
        }

        [Serializable]
        class TicketsPopupConfig : AdPlacementConfig
        {
            public int ticketsToGiveCount = 3;
            public int totalNeededZeroTickets = 1;

            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new TicketsPopupAdPlacement(ticketsToGiveCount, totalNeededZeroTickets, availabilityLevel, maxPlaysInDay);
            }
        }
        
        [Serializable]
        class LevelInfoConfig : AdPlacementConfig
        {
            public int totalNeededContinueLoses=3;
            
            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new LevelInfoAdPlacement(totalNeededContinueLoses,availabilityLevel, maxPlaysInDay, AdvertisementPlacementType.Rewarded);
            }
        }

        [Serializable]
        class LuckySpinnerConfig : AdPlacementConfig
        {
            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new LuckySpinnerAdvertisementPlacement(availabilityLevel, maxPlaysInDay, AdvertisementPlacementType.Rewarded);
            }
        }

        [Serializable]
        class ContinueWithExtraMoveConfig : AdPlacementConfig
        {
            public int extraMoves = 2;
            public int totalNeededLosses = 5;

            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new ContinuingWithExtraMovesAdPlacement(extraMoves, totalNeededLosses, availabilityLevel, maxPlaysInDay);
            }
        }

        [Serializable]
        class DoubleLevelRewardConfig : AdPlacementConfig
        {
            public int totalNeededWins = 4;

            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new DoublingLevelCoinRewardAdPlacement(totalNeededWins, availabilityLevel, maxPlaysInDay);
            }
        }

        [Serializable]
        class MapEnteringInterstitialConfig : AdPlacementConfig
        {
            public int totalNeededTransitions = 5;

            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new MapEnteringInterstitialAdPlacement(totalNeededTransitions, availabilityLevel, maxPlaysInDay);
            }
        }

        [Serializable]
        class MainMenuAdConfig : AdPlacementConfig
        {
            public int availabilityGapDuration = 7200;
            public List<SelectableReward> rewards;

            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new MainMenuAdPlacement(availabilityGapDuration, rewards == null ? new Reward[] {new CoinReward(count: 300)} : rewards.Select(reward => reward.GetReward()).ToArray(), availabilityLevel, maxPlaysInDay);
            }
        }

        [Serializable]
        class TaskPopupAdConfig : AdPlacementConfig
        {
            public int availabilityGapDuration = 7200;
            public List<SelectableReward> rewards;

            public override AdvertisementPlacement CreateAdPlacement()
            {
                return new TaskPopupAdPlacement(availabilityGapDuration, rewards == null ? new Reward[] {new CoinReward(count: 300)} : rewards.Select(reward => reward.GetReward()).ToArray(), availabilityLevel, maxPlaysInDay);
            }
        }
    }
}