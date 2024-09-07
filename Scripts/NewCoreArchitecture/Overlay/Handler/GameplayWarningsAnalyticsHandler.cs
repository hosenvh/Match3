using Match3.Foundation.Base.EventManagement;
using Match3.LiveOps.DogTraining.Presentation.RewardLossWarningsPopups;
using Match3.LiveOps.SeasonPass.Presentation.Popups.RewardLossWarnings;


namespace Match3.Overlay.Analytics
{

    public class GameplayWarningsAnalyticsHandler : GameplayAnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            if (evt is DogTrainingLevelExitConfirmEvent dogTrainingExitConfirmEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_Warning_LevelExit(WarningSource.DogTraining, dogTrainingExitConfirmEvent.Result));
            else if (evt is DogTrainingLevelGameOverPopupResultEvent dogTrainingGameOverPopupResultEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_Warning_GameOver(WarningSource.DogTraining, dogTrainingGameOverPopupResultEvent.Result));

            else if (evt is SeasonPassLevelExitConfirmEvent seasonPassLevelExitConfirmEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_Warning_LevelExit(WarningSource.SeasonPass, seasonPassLevelExitConfirmEvent.Result));
            else if (evt is SeasonPassGameOverPopupResultEvent seasonPassGameOverPopupResultEvent)
                AnalyticsManager.SendEvent(new AnalyticsData_Warning_GameOver(WarningSource.SeasonPass, seasonPassGameOverPopupResultEvent.Result));
        }
    }
}