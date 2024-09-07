using Match3.Game;
using Match3.Presentation.Rewards;
using UnityEngine;

namespace GameScripts.States
{
    public class Popup_RewardConfirmBox : Popup_ConfirmBox
    {
        [SerializeField] private Transform rewardContainer;

        public Popup_RewardConfirmBox(Transform rewardContainer)
        {
            this.rewardContainer = rewardContainer;
        }

        public Popup_ConfirmBox Setup(Reward reward, ConfirmPopupTexts texts, bool closeOnConfirm, System.Action<bool> onResult)
        {
            var rewardPresentation = new RewardPresentationFactory().GenerateRewardPresentation(RewardPresentationType.BottomValue, reward, rewardContainer);
            rewardPresentation.HideCount();
            return Setup(texts, closeOnConfirm, onResult);
        }
    }
}