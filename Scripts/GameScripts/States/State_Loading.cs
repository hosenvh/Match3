using UnityEngine;
using UnityEngine.UI;
using SeganX;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Match3.Foundation.Base.TaskManagement;

namespace Match3
{
    public class State_Loading : GameState
    {
        // TODO: Try not using static.
        public static SequentialTaskChain taskChain;

        [SerializeField] Image loadingImage = null;
        [SerializeField] Text percentageText = null;
        [SerializeField] float progressSpeed = default;

        AsyncOperation asyncOperation = null;

        float currentProgress;

        bool isStarted = false;
        private bool isSceneChangedAllowed;

        IEnumerator Start()
        {
            currentProgress = 0;
            UpdateLoadingBar(currentProgress);

            yield return null;

            isStarted = true;
            asyncOperation = SceneManager.LoadSceneAsync("Game");
            asyncOperation.completed += (a) => { Debug.Log("Scene Management | Scene Loading COMPLETED!"); };
            asyncOperation.allowSceneActivation = false;
            taskChain.Execute(delegate { }, delegate { });
            loadingImage.fillAmount = 0;

        }
        
       
        private void Update() 
        {
            if (isStarted == false)
                return;

            UpdateProgress(Time.deltaTime);

            UpdateLoadingBar(currentProgress);

            if (IsCompleted())
                Invoke(nameof(AllowSceneToChanged), time: 1);
        }

        private void AllowSceneToChanged()
        {
            if(isSceneChangedAllowed)
                return;
            isSceneChangedAllowed = true;
            asyncOperation.allowSceneActivation = true;
        }

        private void UpdateProgress(float deltaTime)
        {
            currentProgress = Mathf.MoveTowards(currentProgress, GetActualProgress(), progressSpeed * deltaTime);
        }

        private void UpdateLoadingBar(float progress)
        {
            loadingImage.fillAmount = progress;
            percentageText.SetText(Math.Min((int)(100 * progress), 100).ToString() + " %");
        }

        private bool IsCompleted()
        {
            return currentProgress >= 1;
        }

        private void OnDestroy()
        {
            taskChain = null;
        }

        private float GetActualProgress()
        {
            var taskProgress = taskChain.Progress();
            var sceneLoadProgress = asyncOperation.progress / 0.9f;
            return Mathf.Min(taskProgress, sceneLoadProgress);
        }
    }
}