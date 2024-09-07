using System;
using I2.Loc;
using Match3.Game.TaskManagement;
using static Base;


namespace Match3.Game.SkipScenario
{
    public class MainMenuSkipScenarioFinishedTask : MainMenuTask
    {
        private readonly string successfullySkipScenarioMessage;
        private bool isTaskExecuted;

        public MainMenuSkipScenarioFinishedTask(string successfullySkipScenarioMessage)
        {
            this.successfullySkipScenarioMessage = successfullySkipScenarioMessage;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            isTaskExecuted = true;
            gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                successfullySkipScenarioMessage,
                confirmString: ScriptLocalization.UI_General.Nice,
                cancelString: "",
                closeOnConfirm: true,
                onResult: result =>
                {
                    onComplete.Invoke();

                });
        }

        protected override bool IsConditionSatisfied()
        {
            return isTaskExecuted == false;
        }
    }
}