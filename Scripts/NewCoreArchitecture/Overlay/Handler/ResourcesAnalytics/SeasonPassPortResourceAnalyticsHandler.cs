using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.LiveOps.Foundation;
using Match3.LiveOps.SeasonPass.Game;
using Match3.LiveOps.SeasonPass.Game.Controller;
using Match3.LiveOps.SeasonPass.Game.Handlers.Base;
using Match3.LiveOps.SeasonPass.Game.Handlers.Steps;
using Match3.Overlay.Analytics.ResourcesAnalytics;
using Medrick.LiveOps.Foundation;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class SeasonPassPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        public SeasonPassPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(ResourcesItemType.SeasonPass, actions)
        {
        }

        protected override void Handle(GameEvent evt)
        {
            if (evt is SeasonPassRewardBundleApplyStartedEvent startedEvent)
                OpenPort(startedEvent.RewardBundle.RewardMainBundleId);
            else if (evt is SeasonPassRewardBundleApplyFinishedEvent finishedEvent)
                ClosePort(finishedEvent.RewardBundle.RewardMainBundleId);
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
        }
    }
}