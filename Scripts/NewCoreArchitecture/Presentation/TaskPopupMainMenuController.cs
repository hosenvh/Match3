using System;
using System.Collections.Generic;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;
using UnityEngine.Events;


namespace Match3.Presentation.MainMenu
{
    public class TaskPopupMainMenuController : MonoBehaviour
    {
        public Notifier notifier;
        public UnityEvent onNoStar;

        string hasUnseenNewTaskString = "HasUnseenNewTask";

        public bool HasUnseenNewTask
        {
            get { return PlayerPrefs.GetInt(hasUnseenNewTaskString, 1) == 1; }
            private set { PlayerPrefs.SetInt(hasUnseenNewTaskString, value ? 1 : 0); }
        }

        private ScenarioManager scenarioManager;
        private List<ScenarioItem> scenarioItems;
        private global::Game gameManager;

        
        
        private void Awake()
        {
            gameManager = Base.gameManager;
            scenarioManager = gameManager.scenarioManager;
            notifier.SetNotify(HasUnseenNewTask, this);
        }

        public Popup_Tasks OpenTaskPopup()
        {
            return OpenTaskPopup(delegate { }, delegate { }, delegate { });
        }

        public Popup_Tasks OpenTaskPopup(Action onCanceled, Action onTaskStart, Action onNoStar)
        {
            gameManager.tutorialManager.CheckAndHideTutorial(0);
            gameManager.tutorialManager.CheckAndHideTutorial(81);
            gameManager.tutorialManager.CheckAndHideTutorial(82);
            notifier.SetNotify(false, this);
            HasUnseenNewTask = false;

            var taskPopup = gameManager.OpenPopup<Popup_Tasks>();
            taskPopup.OnCloseEvent += onCanceled;
            taskPopup.Setup(
                (taskConfig) =>
                {
                    taskPopup.OnCloseEvent -= onCanceled;
                    
                    gameManager.taskManager.SetTaskDone(taskConfig);
                    scenarioItems = gameManager.taskManager.GetScenarioItems(taskConfig);
                    scenarioManager.SaveScenarioStates(scenarioItems);
                    HasUnseenNewTask = true;
                }, (taskConfig) =>
                {
                    ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), this);
                    notifier.SetNotify(true, this);
                    gameObject.SetActive(false);

                    gameManager.ClosePopup();
                    gameManager.OpenPopup<Popup_TouchDisabler>();
                    scenarioManager.StartScenarios(scenarioItems, () =>
                    {
                        gameManager.ClosePopup();
                        gameObject.SetActive(true);

                        switch (taskConfig.id)
                        {
                            case 1:
                                gameManager.tutorialManager.CheckThenShowTutorial(4, 0, null);
                                break;
                            case 2:
                                gameManager.tutorialManager.CheckThenShowTutorial(72, 0, null);
                                break;
                            case 3:
                                gameManager.tutorialManager.CheckThenShowTutorial(73, 0, null);
                                break;
                            case 5:
                                gameManager.tutorialManager.CheckThenShowTutorial(22, 0, null);
                                break;
                            case 19:
                                AnalyticsManager.SendEvent(new AnalyticsData_DayOne_End());
                                break;
                            default:
                                break;
                        }
                    });
                },
                () =>
                {
                    onNoStar();
                    this.onNoStar.Invoke();
                });
            switch (gameManager.profiler.LastUnlockedLevel)
            {
                case 0:
                    gameManager.tutorialManager.CheckThenShowTutorial(1, 0, null);
                    break;
                case 1:
                    gameManager.tutorialManager.CheckThenShowTutorial(81, 0, null);
                    break;
                case 2:
                    gameManager.tutorialManager.CheckThenShowTutorial(82, 0, null);
                    break;
            }

            return taskPopup;
        }


        public void OnTasksButtonClick()
        {
            gameManager.fxPresenter.PlayClickAudio();
            OpenTaskPopup();
        }
    }
}