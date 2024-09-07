using Match3.Foundation.Base.EventManagement;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.SubSystems.LevelEnding;

namespace Match3.Overlay.Analytics.BoardPreview
{
    public class PreviewGameBoardClickedAnalyticsHandler : BoardPreviewAnalyticsHandler
    {
        private bool isAcceptToSendPreviewClickedEvent;

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case LevelEndedEvent levelEndedEvent:
                    if (levelEndedEvent.result == LevelResult.Lose)
                        isAcceptToSendPreviewClickedEvent = true;
                    break;
                case GameOverBoardPreviewClickedEvent boardPreviewClickedEvent:
                    if (isAcceptToSendPreviewClickedEvent)
                    {
                        isAcceptToSendPreviewClickedEvent = false;
                        AnalyticsManager.SendEvent(new AnalyticsData_BoardPreview_PreviewGameBoardClicked());
                    }
                    break;
            }
        }
    }
}
