using Match3.Presentation.MainMenu;
using UnityEngine;


namespace Match3.Presentation.MainMenu
{
    // TODO: The current implementation of task popup handling is a shitty fucking mess and requires refactoring. Currently, most of its setup is being performed within a MonoBehaviour attached to the mainMenu popup, restricting opening this popup easily and from anywhere in the game.
    public class TaskPopupOpener
    {
        public Popup_Tasks TryOpenTaskPopup()
        {
            TaskPopupMainMenuController mainMenuTaskPopupController = GameObject.FindObjectOfType<TaskPopupMainMenuController>();
            if (mainMenuTaskPopupController)
                return mainMenuTaskPopupController.OpenTaskPopup();

            Debug.LogError("Trying To Open The Task Popup Whilst TaskPopupMainMenuController is not found, probably we are not in main menu");
            return null;
        }
    }
}