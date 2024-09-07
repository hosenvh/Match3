using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Development.Base;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Base.TaskManagement;
using Match3.Game.TaskManagement;
using UnityEngine;
using static Base;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Tutorial & Intros", priority: 0)]
    public class TutorialsAndIntrosDevOption : DevelopmentOptionsDefinition
    {
        private static MainMenuTaskChainService MainMenuTaskChain => ServiceLocator.Find<MainMenuTaskChainService>();

        [DevOption(commandName: "Leave Me Alone!", color: DevOptionColor.Cyan, shouldAutoClose: true)]
        public static void DisableInterrupters()
        {
            CloseMainMenuPopups();
            DisableTutorial();
            DisableIntros();
        }

        private static void CloseMainMenuPopups()
        {
            if (IsOnMainMenu())
                while (ShouldCloseCurrentPopup())
                    CloseCurrentPopup();

            bool IsOnMainMenu() => gameManager.GetPopup<Popup_MainMenu>() != null;
            bool ShouldCloseCurrentPopup() => gameManager.CurrentPopup.GetType() != typeof(Popup_MainMenu);
            void CloseCurrentPopup() => gameManager.ClosePopup();
        }

        [DevOption(commandName: "Fast Start", color: DevOptionColor.Cyan, shouldAutoClose: true)]
        public static void PrepareForFastStart()
        {
            DisableInterrupters();
            MaximizeResources();
            SkipOneLevel();

            void MaximizeResources() => GameResourcesDevOptions.MaximizeResources();
            void SkipOneLevel() => MapDevOptions.SkipOneScenario();
        }

        [DevOption(commandName: "Disable Tutorial")]
        public static void DisableTutorial()
        {
            for (int i = 0; i < gameManager.tutorialManager.tutorialConfigs.Count; i++)
                PlayerPrefs.SetInt(gameManager.tutorialManager.TutorialShowedString(i), 1);
        }

        [DevOption(commandName: "Reset All Tutorials")]
        public static void ResetTutorials()
        {
            for (int i = 0; i < gameManager.tutorialManager.tutorialConfigs.Count; i++)
                ResetTutorial(i);
        }

        [DevOption(commandName: "Reset Tutorial")]
        public static void ResetTutorial(int index)
        {
            PlayerPrefs.DeleteKey(gameManager.tutorialManager.TutorialShowedString(index));
        }

        [DevOption(commandName: "Disable Intros")]
        public static void DisableIntros()
        {
            List<Task> tasksToRemove = GetIntroPopupOpeningMainMenuTasks();
            tasksToRemove.DoForEachElement(todo: RemoveMainMenuTask);

            List<Task> GetIntroPopupOpeningMainMenuTasks() => MainMenuTaskChain.FindTasksOfType<IntroPopupOpenerMainMenuTask>();
            void RemoveMainMenuTask(Task task) => MainMenuTaskChain.RemoveTask(task);
        }
    }
}
