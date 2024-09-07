using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.HUD;


namespace Match3.Game.TaskManagement
{

    public class MainMenuNotificationGiftTask : MainMenuTask
    {
        private HudPresentationController hudPresentationController;
        
        private Reward[] notificationGifts;
        private EventManager eventManager = ServiceLocator.Find<EventManager>();

        
        public MainMenuNotificationGiftTask(HudPresentationController hudPresentationController)
        {
            this.hudPresentationController = hudPresentationController;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            FirebaseNotificationGiftHelper.ApplyGifts(notificationGifts, hudPresentationController, eventManager,
                onComplete);
        }

        protected override bool IsConditionSatisfied()
        {
            return FirebaseNotificationGiftHelper.HasGift(out notificationGifts);
        }
    }

}