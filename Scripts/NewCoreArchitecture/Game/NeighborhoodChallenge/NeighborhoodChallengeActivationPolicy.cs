using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using System;

namespace Match3.Game.NeighborhoodChallenge
{
    public class NeighborhoodChallengeActivationPolicy : EventListener
    {
        bool isEnabled;
        TaskConfig activationTaskConfig;
        int activationLevelIndex;

        Action activationAction;

        public NeighborhoodChallengeActivationPolicy(Action activationAction)
        {
            this.activationAction = activationAction;

            ServiceLocator.Find<ConfigurationManager>().Configure(this);

            ServiceLocator.Find<EventManager>().Register(this);

            CheckForActivation();

        }

        private void CheckForActivation()
        {
            if (ShouldActivate())
            {
                activationAction.Invoke();
                ServiceLocator.Find<EventManager>().UnRegister(this);
            }
        }

        private bool ShouldActivate()
        {
            return isEnabled
                && Base.gameManager.taskManager.IsTaskDone(activationTaskConfig)
                && Base.gameManager.profiler.LastUnlockedLevel >= activationLevelIndex;
        }

        public void SetActivationInfo(TaskConfig taskConfig, int levelIndex)
        {
            this.activationTaskConfig = taskConfig;
            this.activationLevelIndex = levelIndex;
        }

        public void SetIsEnabled(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        public void OnEvent(GameEvent evt, object sender)
        {
            if (evt is TaskDoneEvent)
                CheckForActivation();
        }

        public bool IsEnabled()
        {
            return isEnabled;
        }
    }
}