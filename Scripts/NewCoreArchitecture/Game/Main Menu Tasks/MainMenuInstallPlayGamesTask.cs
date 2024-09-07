using System;
using UnityEngine;



namespace Match3.Game.TaskManagement
{

    public class MainMenuInstallPlayGamesTask : MainMenuTask
    {

        private CloudSaveMessenger cloudSaveMessenger;
        
        public MainMenuInstallPlayGamesTask(CloudSaveMessenger cloudSaveMessenger)
        {
            this.cloudSaveMessenger = cloudSaveMessenger;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            cloudSaveMessenger.ShowInstallPlayGamesMessage(onAbort, onComplete);
        }

        protected override bool IsConditionSatisfied()
        {
            return cloudSaveMessenger.IsInstallPlayGamesMessageExists();
        }
    }

}