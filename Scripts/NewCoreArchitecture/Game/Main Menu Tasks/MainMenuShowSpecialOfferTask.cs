using System;
using UnityEngine;



namespace Match3.Game.TaskManagement
{

    public class MainMenuShowSpecialOfferTask : MainMenuTask
    {
        private MainMenuDynamicSpecialOfferController specialOfferController;
        
        public MainMenuShowSpecialOfferTask(MainMenuDynamicSpecialOfferController dynamicSpecialOfferMenuController)
        {
            specialOfferController = dynamicSpecialOfferMenuController;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            specialOfferController.dynamicSpecialOfferMenuUi.OpenSpecialOfferPopup(onAbort, onComplete);
            MainMenuDynamicSpecialOfferController.openSpecialOfferTime = Time.time;
            MainMenuDynamicSpecialOfferController.dynamicOfferShowed = true;
        }

        protected override bool IsConditionSatisfied()
        {
            return specialOfferController.CanShowDynamicSpecialOffer();
        }
    }

}