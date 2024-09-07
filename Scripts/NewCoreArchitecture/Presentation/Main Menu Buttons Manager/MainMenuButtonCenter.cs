using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Presentation.MainMenu
{

    public class MainMenuButtonCenter : MonoBehaviour
    {
        public Transform leftButtonsColumn;
        public Transform rightButtonsColumn;
        public Transform buttonsControllerParent;

        [Space(10)] public List<MainMenuButton> buttons;

        private MainMenuButtonBoard buttonBoard;



        private void Awake()
        {
            buttonBoard = ServiceLocator.Find<MainMenuButtonBoard>();
            RegisterButtons();
        }

        private void Start()
        {
            CreateButtons();
        }

        private void RegisterButtons()
        {
            foreach (var button in buttons)
                buttonBoard.RegisterButton(button.id, button.GetSetting(), button.CreationCondition, button.Create);
        }

        private void CreateButtons()
        {
            buttonBoard.CreateButtons(leftButtonsColumn, rightButtonsColumn, buttonsControllerParent);
        }
    }

}