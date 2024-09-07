using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay.SubSystems.LevelContinuing;

namespace Match3.Overlay.Analytics.BoardPreview
{
    public class ResumeLevelWithExtraMoveAnalyticsHandler : BoardPreviewAnalyticsHandler
    {
        protected override void Handle(GameEvent evt)
        {
            if (IsContinuingGameWithExtraMove())
                AnalyticsManager.SendEvent(new AnalyticsData_BoardPreview_ResumeLevelWithExtraMove());

            bool IsContinuingGameWithExtraMove() => (
                evt is LevelResumeWithExtraMoveEvent ||
                evt is LevelResumeWithAdsExtraMoveEvent
            );
        }
    }
}
