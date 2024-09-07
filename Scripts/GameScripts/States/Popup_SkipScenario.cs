using System;
using I2.Loc;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.SkipScenario;
using SeganX;


namespace Match3.Presentation.SkipScenario
{
    
    public class Popup_SkipScenario : GameState
    {
        public LocalizedStringTerm successfullySkipScenarioMessage;

        private Action onCancel;
        
        public void Setup(Action onCancel)
        {
            this.onCancel = onCancel;
        }
        
        public void SkipScenarios()
        {
            gameManager.ClosePopup(true);
            var popupWaitBox = gameManager.OpenPopup<Popup_WaitBox>();
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(0.01f, () =>
            {
                gameManager.ClosePopup(popupWaitBox);
                gameManager.skipScenarioController.SkipScenarioWithStackedStars(() =>
                {
                    ServiceLocator.Find<MainMenuTaskChainService>().AddTask(new MainMenuSkipScenarioFinishedTask(successfullySkipScenarioMessage), priority: MainMenuTasksPriorities.SkipScenarioFinishedTask, "SkipScenarioFinishedTask");
                });
            }, this);
        }

        public void RejectSkipOffer()
        {
            onCancel();
            gameManager.skipScenarioController.RejectSkipOffer();
            Back();
        }
        
        
    }
    
}

