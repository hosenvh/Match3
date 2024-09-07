using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.HUD;



namespace Match3.Game.TaskManagement
{

    public class MainMenuDifficultyRewardTask : MainMenuTask
    {
        
        private HudPresentationController hudPresentationController;
        private EventManager eventManager = ServiceLocator.Find<EventManager>();

        public MainMenuDifficultyRewardTask(HudPresentationController hudPresentationController)
        {
            this.hudPresentationController = hudPresentationController;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            DynamicDifficultyMainMenuPopup.GiveDynamicDifficultyRewardWithPopup(Base.gameManager,
                hudPresentationController, eventManager, onComplete);
        }

        protected override bool IsConditionSatisfied()
        {
            return DynamicDifficultyMainMenuPopup.IsDynamicDifficultyInfiniteLifeRewardAvailable();
        }
    }

}