using UnityEngine;
using UnityEngine.UI;
using SeganX;


namespace Match3.Presentation
{
    public class Popup_GameMainLoopTutorial : GameState
    {
        [SerializeField] private Button continueButton;

        private void Awake()
        {
            SetupContinueButton();
        }

        private void SetupContinueButton()
        {
            continueButton.onClick.AddListener(Close);
        }
    }
}