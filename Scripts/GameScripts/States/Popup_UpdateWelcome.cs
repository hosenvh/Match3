using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Game;
using Match3.Game.UpdateWelcome;
using Match3.Presentation.HUD;
using Match3.Presentation.TextAdapting;
using SeganX;


namespace Match3.Presentation.UpdateWelcome
{
    public class Popup_UpdateWelcome : GameState
    {
        public TextAdapter changeLogText;
        public TextAdapter coinCountText;

        private EventManager eventManager;
        private HudPresentationController hudPresentationController;
        private UpdateWelcomeController updateWelcomeController;
        private int coinCount;

        private Action onFinishClaimReward;

        public Popup_UpdateWelcome Setup(HudPresentationController hudPresentationController, EventManager eventManager,
            UpdateWelcomeController updateWelcomeController, string changeLog, int coinCount)
        {
            this.eventManager = eventManager;
            this.hudPresentationController = hudPresentationController;
            this.updateWelcomeController = updateWelcomeController;
            this.coinCount = coinCount;
            changeLogText.SetText(changeLog);
            coinCountText.SetText(coinCount.ToString());
            return this;
        }

        public void SetOnFinishClaimReward(Action onFinish)
        {
            onFinishClaimReward = onFinish;
        }

        public void ClaimUpdateReward()
        {
            var coinReward = new CoinReward(coinCount);
            coinReward.Apply();
            var claimRewardPopup = gameManager.OpenPopup<Popup_ClaimReward>();
            claimRewardPopup.Setup(new Reward[] {coinReward})
                            .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                            .SetAdditionalDelayBeforeClosing(1)
                            .StartPresentingRewards();
            if (onFinishClaimReward != null)
                claimRewardPopup.OnCloseEvent += onFinishClaimReward;
            updateWelcomeController.UpdateLastCheckedVersion();
            Close();
        }

        public override void Back()
        {
            // Do Nothing
        }
    }
}