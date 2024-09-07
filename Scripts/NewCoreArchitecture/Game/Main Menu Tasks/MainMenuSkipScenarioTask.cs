using System;
using Match3.Presentation.SkipScenario;


namespace Match3.Game.TaskManagement
{

    public class MainMenuSkipScenarioTask : MainMenuTask
    {
        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            Base.gameManager.OpenPopup<Popup_SkipScenario>().Setup(onComplete);
        }

        protected override bool IsConditionSatisfied()
        {
            return Base.gameManager.skipScenarioController.IsProperTimeToSkipScenario();
        }
    }

}