using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Presentation.Gameplay;
using System;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Overlay.Analytics.LevelEntries;
using Match3.Utility.GolmoradLogging;
using UnityEngine;


namespace Match3.Overlay.Analytics
{
    public class GameplayAnalyticsAdapter : AnalyticsAdapter
    {
        public GameplayAnalyticsAdapter()
        {
            RegisterHandler(new GlobalLevelEntriesAnalyticsHandler());
            RegisterHandler(new ChallengeLevelSpecificLevelEntriesAnalyticsHandler());
            RegisterHandler(new ReferralAnalyticsHandler());
            RegisterHandler(new GameplayUserInteractionsAnalyticsHandler());
            RegisterHandler(new GameplayItemGenerationAndActivationAnalyticsHandler());
            RegisterHandler(new GameplayProgressAnalyticsHandler());
            RegisterHandler(new MiscellaneousGameplayAnalyticsHandler());
            RegisterHandler(new GameplayPurchaseAnalyticsHandler());
            RegisterHandler(new NeighbourHoodAnalyticsHandler());
            RegisterHandler(new GameplayWarningsAnalyticsHandler());
        }

        public override void OnEvent(GameEvent evt, object sender)
        {
            try
            {
                if (evt is LevelStartedEvent)
                {
                    var movementStopCondition = (Base.gameManager.CurrentState as GameplayState).gameplayController.GetSystem<LevelStoppingSystem>().FindStoppingCondition<MovementStopCondition>();
                    AnalyticsDataMaker.Setup(null, movementStopCondition.RemainingMovements);

                    AnalyticsManager.SetMatchId(DateTime.Now.ToFileTime());
                }
            }
            catch(Exception e)
            {
                DebugPro.LogException<AnalyticsLogTag>($"Error in GameplayAnalyticsAdapter:\n{e}");
            }
            base.OnEvent(evt, sender);
        }
    }
}