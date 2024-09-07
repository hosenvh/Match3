using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.HUD;


namespace Match3.Game.TaskManagement
{

    public class MainMenuUpdateWelcomeTask : MainMenuTask
    {
        private HudPresentationController hudPresentationController;
        private EventManager eventManager = ServiceLocator.Find<EventManager>();

        public MainMenuUpdateWelcomeTask(HudPresentationController hudPresentationController)
        {
            this.hudPresentationController = hudPresentationController;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            Base.gameManager.updateWelcomeController.OpenUpdateWelcomePopup(hudPresentationController,
                eventManager, onComplete);
        }

        protected override bool IsConditionSatisfied()
        {
            return Base.gameManager.updateWelcomeController.ShouldOpenUpdateWelcome();
        }
    }

}