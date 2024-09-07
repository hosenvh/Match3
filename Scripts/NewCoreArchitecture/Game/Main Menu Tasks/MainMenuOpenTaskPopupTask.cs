using System;
using Match3.Presentation.MainMenu;
using UnityEngine;


namespace Match3.Game.TaskManagement
{
    public class MainMenuOpenTaskPopupTask : MainMenuTask
    {
        private global::Game gameManager = Base.gameManager;
        private TaskPopupMainMenuController taskPopupMainMenuController;
        
        public MainMenuOpenTaskPopupTask(TaskPopupMainMenuController taskPopupMainMenuController)
        {
            this.taskPopupMainMenuController = taskPopupMainMenuController;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            taskPopupMainMenuController.OpenTaskPopup(onComplete, onAbort, onAbort);
        }

        protected override bool IsConditionSatisfied()
        {
            return gameManager.profiler.LastUnlockedLevel > 0 && Base.gameManager.profiler.LastUnlockedLevel < 4 &&
                   Base.gameManager.profiler.StarCount > 0;
        }

        
    }
}