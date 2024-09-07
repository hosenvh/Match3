using System;

namespace Match3.Game.TaskManagement
{
    public class MainMenuCloudDeviceConflictTask : MainMenuTask
    {
        private const int TASK_EXECUTION_LIMIT = 3;

        private CloudSaveMessenger cloudSaveMessenger;
        private static int numberOfTaskExecuted;

        public MainMenuCloudDeviceConflictTask(CloudSaveMessenger cloudSaveMessenger)
        {
            this.cloudSaveMessenger = cloudSaveMessenger;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            cloudSaveMessenger.ShowDeviceConflictMessage(onAbort);
            numberOfTaskExecuted += 1;
        }

        protected override bool IsConditionSatisfied()
        {
            return numberOfTaskExecuted < TASK_EXECUTION_LIMIT && cloudSaveMessenger.HasDeviceConflict();
        }
    }

}