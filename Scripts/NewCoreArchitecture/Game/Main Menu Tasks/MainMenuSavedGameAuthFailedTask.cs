using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Game.TaskManagement
{

    public class MainMenuSavedGameAuthFailedTask : MainMenuTask
    {
        
        private CloudSaveMessenger cloudSaveMessenger;
        
        public MainMenuSavedGameAuthFailedTask(CloudSaveMessenger cloudSaveMessenger)
        {
            this.cloudSaveMessenger = cloudSaveMessenger;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            cloudSaveMessenger.ShowAuthenticationFailedMessage(onAbort);
        }

        protected override bool IsConditionSatisfied()
        {
            return cloudSaveMessenger.IsAuthenticationFailedMessageExists();
        }
    }

}