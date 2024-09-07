using System;
using Match3.Game;
using Match3.Game.Gameplay;


namespace Match3.Presentation.Gameplay
{
    public class CampaignOpenWinPopupTask : LevelEndingTask
    {
        private GameplayController gameplayController;
        private int score;
        
        public CampaignOpenWinPopupTask(GameplayController gameplayController, int score)
        {
            this.gameplayController = gameplayController;
            this.score = score;
        }

        protected override void ExecuteLevelEndingTask(Action onComplete)
        {
            Base.gameManager.OpenPopup<Popup_Win>().Setup(
                gameplayController, 
                score, 
                onExit: onComplete);
        }
    }
}