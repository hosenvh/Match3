using System;



namespace Match3.Game.TaskManagement
{

    public class MainMenuSavedGameConflictTask : MainMenuTask
    {
        private static bool hasShownCloudSaveSwitchConflictMessage;
        private CloudSaveMessenger cloudSaveMessenger;
        
        public MainMenuSavedGameConflictTask(CloudSaveMessenger cloudSaveMessenger)
        {
            this.cloudSaveMessenger = cloudSaveMessenger;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            cloudSaveMessenger.ShowConflictMessage(onAbort);
            hasShownCloudSaveSwitchConflictMessage = true;
        }

        protected override bool IsConditionSatisfied()
        {
            return !hasShownCloudSaveSwitchConflictMessage && cloudSaveMessenger.IsConflictMessageExists();
        }
    }

}