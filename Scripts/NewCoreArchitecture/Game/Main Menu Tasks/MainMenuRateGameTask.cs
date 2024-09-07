using System;



namespace Match3.Game.TaskManagement
{

    public class MainMenuRateGameTask : MainMenuTask
    {
        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            Base.gameManager.joySystem.CheckShowRateUs(onAbort, onComplete, onComplete);
        }

        protected override bool IsConditionSatisfied()
        {
            return true;
        }
    }

}