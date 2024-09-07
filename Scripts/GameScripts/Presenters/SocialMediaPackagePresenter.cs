using UnityEngine;
using System;
using Match3.Presentation.TextAdapting;

namespace Match3.Presentation.ShopManagement
{
    public class SocialMediaPackagePresenter : MonoBehaviour, AnimatatedPackage
    {
        private static readonly int Appear = Animator.StringToHash("Appear");
        private static readonly int Hide = Animator.StringToHash("Hide");

        [SerializeField] TextAdapter coinCountText = default;
        [SerializeField] GameObject coinObject = default;
        [SerializeField] private Animator animator;

        Action clickAction;

        public void Setup(int coinCount, Action clickAction)
        {
            coinCountText.SetText(coinCount.ToString());
            this.clickAction = clickAction;
        }

        public void OnClick()
        {
            clickAction();
        }

        public void PlayAppearAnimation()
        {
            animator.SetTrigger(Appear);
        }

        public void PlayHideAnimation()
        {
            animator.SetTrigger(Hide);
        }

        public void SetActive(bool value)
        {
            this.gameObject.SetActive(value);
        }

        public void DisableReward()
        {
            coinCountText.gameObject.SetActive(false);
            coinObject.SetActive(false);
        }
    }
}
