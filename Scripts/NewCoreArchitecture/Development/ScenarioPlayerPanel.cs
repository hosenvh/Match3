using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Match3.Development
{
    public class ScenarioPlayerPanel : MonoBehaviour
    {
        [Serializable]
        public class UnityStringEvent : UnityEvent<string>
        {

        }

        public GameObject[] visibilityTargets;

        public Text infoText;
        public UnityStringEvent updatePlayingSpeedString;
        public UnityStringEvent updateInteractionDelayString;

        ScenarioPlayer scenarioPlayer;

        public void Setup(ScenarioPlayer scenarioPlayer)
        {
            this.scenarioPlayer = scenarioPlayer;
            updatePlayingSpeedString.Invoke(scenarioPlayer.GetPlayingSpeed().ToString());
            updateInteractionDelayString.Invoke(scenarioPlayer.GetInteractionDelay().ToString());
        }

        public void StartPlaying()
        {
            scenarioPlayer.Start(UpdateInfo);
        }

        private void UpdateInfo(TaskConfig taskConfig)
        {
            if(this.gameObject != null)
                infoText.text = $"Current Task: {taskConfig.id}";
        }

        public void StopPlaying()
        {
            scenarioPlayer.Stop();
        }

        public void Exit()
        {
            scenarioPlayer.Stop();
            Destroy(this.gameObject);
        }

        public void SetPlayingSpeed(float speed)
        {
            scenarioPlayer.SetPlayingSpeed(speed);
            updatePlayingSpeedString.Invoke(speed.ToString());
        }

        public void SetInteractionDelay(float delayInSeconds)
        {
            scenarioPlayer.SetInteractionDelay(delayInSeconds);
            updateInteractionDelayString.Invoke(delayInSeconds.ToString());
        }

        public void SetItemIndexToChoose(int index)
        {
            scenarioPlayer.SetItemIndexToChoose(index);
        }

        public void ToggleVisibility()
        {
            foreach (var gameObject in visibilityTargets)
                gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}