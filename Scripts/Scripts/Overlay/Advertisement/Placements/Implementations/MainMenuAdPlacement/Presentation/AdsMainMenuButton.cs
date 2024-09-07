using Match3.Foundation.Base.ServiceLocating;
using Match3.Overlay.Advertisement.Placements.Implementations.TimeBasedAdPlacementBase.Presentation;
using Match3.Presentation;
using Match3.Presentation.HUD;
using Match3.Presentation.MainMenu;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Overlay.Advertisement.Placements.Implementations.MainMenuAdPlacement.Presentation
{
    public class AdsMainMenuButton : MainMenuButton
    {
        [SerializeField] private Button unityButton;
        [SerializeField] private RemainedTimePresenter remainedTimePresenter;
        [SerializeField] private GameObject visualForIsReadyMode;
        [SerializeField] private HudPresentationController hudPresentationController;

        private TimeBasedAdPlacementButtonClickHandler<MainMenuAdPlacement> clickHandler;
        private AdsMainMenuButtonDisplayHandler displayHandler;

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
            return MainMenuButtonsSettings.Ads;
        }

        private void Awake()
        {
            Initialize();

            void Initialize()
            {
                clickHandler = new TimeBasedAdPlacementButtonClickHandler<MainMenuAdPlacement>(unityButton, hudPresentationController, onAdsShownSuccessfully: delegate {  });
                displayHandler = new AdsMainMenuButtonDisplayHandler(visualForIsReadyMode, remainedTimePresenter);

                displayHandler.UpdateDisplay();
                StartCoroutine(displayHandler.UpdateDisplayOnRoutine());
            }
        }
    }
}