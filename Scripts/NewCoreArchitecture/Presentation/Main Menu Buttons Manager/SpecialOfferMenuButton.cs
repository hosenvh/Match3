using UnityEngine;


namespace Match3.Presentation.MainMenu
{
    public class SpecialOfferMenuButton : MainMenuButton
    {
        public override Transform Create(Transform buttonParent, Transform buttonControllerParent)
        {
            button.SetParent(buttonParent);
            return button;
        }

        public override bool CreationCondition()
        {
            return true;
        }

        public override MainMenuButtonSetting GetSetting()
        {
            return MainMenuButtonsSettings.SpecialOffer;
        }
    }
}