using Match3.Foundation.Base.EventManagement;
using Match3.Overlay.Analytics.BoardPreview;

namespace Match3.Overlay.Analytics
{
    public class BoardPreviewAnalyticsAdapter : AnalyticsAdapter
    {
        public BoardPreviewAnalyticsAdapter(ServerConfigManager configManager)
        {
            if (!IsBoardPreviewActivate(configManager))   
                return;

            RegisterHandler(new LoseLevelShownAnalyticsHandler());
            RegisterHandler(new PreviewGameBoardClickedAnalyticsHandler());
            RegisterHandler(new ResumeLevelWithExtraMoveAnalyticsHandler());
        }

        public override void OnEvent(GameEvent evt, object sender)
        {
            base.OnEvent(evt, sender);
        }
        
        private bool IsBoardPreviewActivate(ServerConfigManager configManager)
        {
            return GetGameOverServerConfigData().IsBoardPreviewActivate;

            GameOverServerConfigData GetGameOverServerConfigData()
            {
                var gameOverServerConfigData = configManager.data.config.gameOverServerConfigData;
                if (gameOverServerConfigData == null)
                    gameOverServerConfigData = new GameOverServerConfigData();

                return gameOverServerConfigData;
            }
        }
    }
}