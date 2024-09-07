using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Presentation.TransitionEffects;
using UnityEngine;
using static Base;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Map", priority: 6)]
    public class MapDevOptions : DevelopmentOptionsDefinition
    {
        [DevOption(commandName: "Go To Scenario", color: DevOptionColor.Cyan, shouldAutoClose: true)]
        public static void GoToScenarioIndex(int scenario)
        {
            gameManager.scenarioManager.ResetUserStateForGetReadyToSetScenario();
            SkipScenario(scenario);
        }

        [DevOption(commandName: "Skip Scenarios", shouldAutoClose: true)]
        public static void SkipScenario(int skipScenario)
        {
            gameManager.scenarioManager.SkipScenario(skipScenario);
        }

        [DevOption(commandName: "Skip 1 Scenario", shouldAutoClose: true)]
        [ShortCut(KeyCode.LeftShift, KeyCode.S)]
        public static void SkipOneScenario()
        {
            SkipScenario(1);
        }

        [DevOption(commandName: "Go To Last Scenario")]
        public static void GoToLastPublishedScenario()
        {
            int lastScenarioIndex = GetLastScenarioIndex();
            GoToScenarioIndex(lastScenarioIndex);

            int GetLastScenarioIndex() => gameManager.taskManager.LastTaskId();
        }

        [DevOption(commandName: "Play Scenario", color: DevOptionColor.Cyan, shouldAutoClose: true)]
        public static void PlayScenario()
        {
            var panel = Object.Instantiate(Resources.Load<ScenarioPlayerPanel>("ScenarioPlayerPanel"));
            panel.Setup(new ScenarioPlayer());
        }

        [DevOption(commandName: "Set UserSelects Index")]
        public static void SetUserSelectsIndex(int userSelectIndex)
        {
            var mapItemCenter = gameManager.mapItemManager;

            foreach (var mapItemUserSelect in mapItemCenter.CurrentMapUserSelectItems)
            {
                var currentIndex = mapItemCenter.GetUserSelectItemSelectedIndex(mapItemUserSelect);
                if (currentIndex != -1)
                    mapItemCenter.SetUserSelectItemSelectedIndex(mapItemUserSelect, userSelectIndex);
            }
            ServiceLocator.Find<GameTransitionManager>().GoToLastMap<EmptyTransitionEffect>();
        }

        [DevOption(commandName: "Unlock Map State")]
        public static void UnlockMapItemState(int stateIndex)
        {
            gameManager.mapItemManager.GetStateItemFromCurrentMap(stateIndex).Save(1);
            ServiceLocator.Find<GameTransitionManager>().GoToLastMap<DarkInTransitionEffect>();
        }

        [DevOption(commandName: "Go To Mansion", color: DevOptionColor.Magenta)]
        public static void GoToMansion()
        {
            ServiceLocator.Find<GameTransitionManager>().GoToMap<DarkInTransitionEffect>("Golmorad_Mansion", false, delegate { });
        }

        [DevOption(commandName: "Go To Villa", color: DevOptionColor.Green)]
        public static void GoToVilla()
        {
            ServiceLocator.Find<GameTransitionManager>().GoToMap<DarkInTransitionEffect>("Golmorad_Villa", false, delegate { });
        }
    }
}