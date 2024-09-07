using System;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Game.TaskManagement
{

    public class MainMenuFirstScenarioTask : MainMenuTask
    {
        private GameObject mainMenuGameObject;
        private global::Game gameManager;
        List<ScenarioItem> scenarioItems;
        ScenarioManager scenarioManager;
        
        public MainMenuFirstScenarioTask(GameObject mainMenuGameObject)
        {
            this.mainMenuGameObject = mainMenuGameObject;
            gameManager = Base.gameManager;
            scenarioManager = gameManager.scenarioManager;
        }

        protected override void InternalExecution(Action onComplete, Action onAbort)
        {
            StartFirstScenario();
            onAbort();
        }

        protected override bool IsConditionSatisfied()
        {
            return !gameManager.taskManager.IsDoneFirstTaskOfDay();
        }

        private void StartFirstScenario()
        {
            mainMenuGameObject.SetActive(false);
            TaskConfig firstTaskConfigOfDay = null;
            foreach (var item in gameManager.taskManager.CurrentTasksList)
                firstTaskConfigOfDay = item;

            if (firstTaskConfigOfDay)
            {
                gameManager.taskManager.SetTaskDone(firstTaskConfigOfDay);
                scenarioItems = gameManager.taskManager.GetScenarioItems(firstTaskConfigOfDay);
                scenarioManager.SaveScenarioStates(scenarioItems);

                gameManager.OpenPopup<Popup_TouchDisabler>();
                scenarioManager.StartScenarios(scenarioItems, () =>
                {
                    gameManager.ClosePopup();
                    mainMenuGameObject.SetActive(true);
                    gameManager.tutorialManager.CheckThenShowTutorial(0, 0, null);
                });
            }
            else
                Debug.LogError("some thing is wrong!");
        }
        
    }

}