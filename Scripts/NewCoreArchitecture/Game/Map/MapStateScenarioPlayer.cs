using System;
using UnityEngine;


namespace Match3.Game
{

    public class MapStateScenarioPlayer : MonoBehaviour
    {
        private ScenarioManager scenarioManager;
        private Popup_MainMenu mainMenu;

        public void Setup(ScenarioManager scenarioManager)
        {
            this.scenarioManager = scenarioManager;
        }

        public void PlayScenario(ScenarioConfig scenarioConfig, Action onFinish, bool shouldSaveChanges = true)
        {
            Base.gameManager.TryGetPopup(ref mainMenu);

            if (shouldSaveChanges)
                scenarioManager.SaveScenarioStates(scenarioConfig.scenarioItems);
            mainMenu.gameObject.SetActive(false);
            var touchDisablerPopup = Base.gameManager.OpenPopup<Popup_TouchDisabler>();

            scenarioManager.StartScenarios(scenarioConfig.scenarioItems, () =>
            {
                Base.gameManager.ClosePopup(touchDisablerPopup);
                mainMenu.gameObject.SetActive(true);
                onFinish();
            });
        }

    }

}