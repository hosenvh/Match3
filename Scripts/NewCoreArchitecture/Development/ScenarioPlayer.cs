using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using SeganX;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Match3.Development
{
    public class ScenarioPlayer
    {
        TaskManager taskManager;
        ScenarioManager scenarioManager;

        UnityTimeScheduler unityTimeScheduler;
        bool isPlaying = false;
        WaitForSecondsRealtime interactionDelay = new WaitForSecondsRealtime(1.5f);
        int itemIndexToChoose = 0;

        public void Start(Action<TaskConfig> onTaskStartAction)
        {
            if (isPlaying)
                return;

            Application.runInBackground = true;
            var gameManager = global::Base.gameManager;

            if (gameManager.mapManager.IsInMap() == false)
            {
                Debug.LogError("You are not in Map.");
                return;
            }

            taskManager = gameManager.taskManager;
            scenarioManager = gameManager.scenarioManager;
            unityTimeScheduler = ServiceLocator.Find<UnityTimeScheduler>();

            isPlaying = true;

            unityTimeScheduler.StartCoroutine(ProcessInteractions());
            StartDoingATask(onTaskStartAction, gameManager);
        }

        public void Stop()
        {
            UnityEngine.Time.timeScale = 1;
            isPlaying = false;
        }


        public void SetPlayingSpeed(float speed)
        {
            UnityEngine.Time.timeScale = speed;
        }

        public void SetInteractionDelay(float delayInSeconds)
        {
            this.interactionDelay.waitTime = delayInSeconds;
        }

        public float GetPlayingSpeed()
        {
            return UnityEngine.Time.timeScale;
        }

        public float GetInteractionDelay()
        {
            return interactionDelay.waitTime;
        }

        public int ItemIndexToChoose()
        {
            return itemIndexToChoose;
        }

        public void SetItemIndexToChoose(int index)
        {
            itemIndexToChoose = index;
        }

        private void StartDoingATask(Action<TaskConfig> onTaskStartAction, GameManager gameManager)
        {
            if (!isPlaying)
                return;

            var taskConfigsList = taskManager.CurrentTasksList.ToList();

            if (taskConfigsList.Count == 0 || IsLastTask(taskConfigsList[0]))
            {
                Stop();
                return;
            }

            var taskConfig = taskConfigsList[0];
            // NOTE: It is really important to call SetTaskDone before GetScenarioItems
            taskManager.SetTaskDone(taskConfig);
            var scenarioItems = taskManager.GetScenarioItems(taskConfig);
            scenarioManager.SaveScenarioStates(scenarioItems);

            var mainMenu = gameManager.GetPopup<Popup_MainMenu>();
            var touchDisabler = gameManager.OpenPopup<Popup_TouchDisabler>();

            mainMenu.gameObject.SetActive(false);
            Debug.Log("Starting task" + taskConfig.id);
            onTaskStartAction.Invoke(taskConfig);
            scenarioManager.StartScenarios(
                scenarioItems, 
                onFinish: () =>
                {
                    touchDisabler.Close();
                    mainMenu.gameObject.SetActive(true);
                    StartDoingATask(onTaskStartAction, gameManager);
                });

        }

        private bool IsLastTask(TaskConfig taskConfig)
        {
            return taskManager.GetDayConfig(taskManager.TotalDays() -1).lastTask == taskConfig;
        }

        System.Collections.IEnumerator ProcessInteractions()
        {
            yield return interactionDelay;
            while (isPlaying)
            {
                ProcessOneInteraction();
                yield return interactionDelay;
            }
        }

        void ProcessOneInteraction()
        {
            Popup_MapItemSelector selector = GameObject.FindObjectOfType<Popup_MapItemSelector>();
            if (IsValid(selector) && selector.IsUserInteractionPossible)
            {
                selector.OnSelectableItemButtonClicked(itemIndexToChoose);
                selector.OnConfirmClick(false);
                return;
            }

            var dialogue = GameObject.FindObjectOfType<Popup_DialogueBox>();
            if (IsValid(dialogue))
            {
                dialogue.OnButtonClick();
                return;
            }

            var socialAlbumPost = GameObject.FindObjectOfType<Popup_SocialAlbumPicturePresenter>();
            if (IsValid(socialAlbumPost))
            {
                socialAlbumPost.Back();
                return;
            }
        }

        private bool IsValid(GameState popup)
        {
            return popup != null && GameManager.gameManager.HasPopup(popup);
        }
    }
}