using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Overlay.Analytics.BoardPreview
{
    public class LoseLevelShownAnalyticsHandler : BoardPreviewAnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            if (IsGameOver())
                AnalyticsManager.SendEvent(new AnalyticsData_BoardPreview_LoseLevelShown());

            bool IsGameOver() => (
                evt is LevelEndedEvent levelEndedEvent &&
                levelEndedEvent.result == LevelResult.Lose);
        }
    }
}
