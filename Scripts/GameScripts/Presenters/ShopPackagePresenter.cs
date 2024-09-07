using I2.Loc;
using UnityEngine;
using SeganX;
using UnityEngine.UI;
using Match3.Foundation.Base.MoneyHandling;
using System;
using Match3.Game.ShopManagement;
using Match3.Presentation.TextAdapting;

namespace Match3.Presentation.ShopManagement
{
    public abstract class ShopPackagePresenter<T> : MonoBehaviour, AnimatatedPackage where T : HardCurrencyPackage
    {
        private static readonly int Appear = Animator.StringToHash("Appear");
        private static readonly int Hide = Animator.StringToHash("Hide");


        [SerializeField] GameObject[] iconGameObjects = default;
        [SerializeField] protected LocalText titleText = null;
        [SerializeField] private LocalText priceText = null, tagText = null;
        [SerializeField] GameObject tagTextParentGameObject = default;
        [SerializeField] private Button purchaseButton = default;
        [SerializeField] private Animator animator;

        [SerializeField] TextAdapter previousPriceText = default;
        [SerializeField] GameObject previousPriceContainer = default;

        T package;
        Action<T> clickAction;

        public void Setup(T package, Action<T> clickAction)
        {
            this.package = package;
            this.clickAction = clickAction;
            SetupTag(package.Tag());
            SetupPrice(package.IsFree(), package.Price());
            SetIcon(package.IconIndex());
            SetupDiscount(package.DiscountInfo());
            SetupTitle(package.Title());
                
            InternalSetup(package);
        }

        protected abstract void InternalSetup(T package);

        public void OnClick()
        {
            clickAction(package);
        }

        public virtual void SetPurchasedBefore()
        {
            DisableDiscountPrice();
            purchaseButton.interactable = false;
            priceText.SetText(ScriptLocalization.Message_General.YouGotItBefore);
        }

        private void SetupPrice(bool isFree, Money price)
        {
            var priceLabel = isFree ? ScriptLocalization.UI_General.Free.ToString() : price.FormatMoneyToString();
            priceText.SetText(priceLabel);
        }

        private void SetIcon(int index)
        {
            foreach (var icon in iconGameObjects)
                icon.SetActive(false);

            if (iconGameObjects.Length > index)
                iconGameObjects[index].SetActive(true);
        }

        private void SetupDiscount(DiscountInfo discountInfo)
        {
            if (discountInfo.HasDiscount())
                previousPriceText.SetText(discountInfo.BeforeDiscountPrice().FormatMoneyToString());
            else
                DisableDiscountPrice();
        }

        private void DisableDiscountPrice()
        {
            previousPriceContainer.SetActive(false);

            // NOTE: This is an ad hoc solution to align the price.
            var localPos = priceText.transform.localPosition;
            priceText.transform.localPosition = new Vector3(localPos.x, 0, localPos.y);
        }

        private void SetupTitle(string title)
        {
            if (titleText != null)
                titleText.SetText(title);
        }

        private void SetupTag(string tag)
        {
            if (tagTextParentGameObject)
            {
                if (!string.IsNullOrEmpty(tag))
                    tagText.SetText(tag.Replace("\\n", "\n"));
                else
                    tagTextParentGameObject.SetActive(false);
            }
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
    }
}
