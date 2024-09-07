using SeganX;
using Match3.Game.ShopManagement;
using System;
using I2.Loc;
using Match3.Foundation.Base.MoneyHandling;

namespace Match3.Presentation.ShopManagement
{
    public partial class ShopPopupCreator
    {
        public enum FailureResult
        {
            PurchaseDisabled,
            PriceQueryFailure,
            Timeout,
        }


        private const float TIMEOUT_DELAY = 20f;

        GolmoradShopCenter shopCenter;
        IMarketManager marketManager;
        GameManager gameManager;

        public ShopPopupCreator(GolmoradShopCenter shopCenter, IMarketManager marketManager, GameManager gameManager)
        {
            this.shopCenter = shopCenter;
            this.marketManager = marketManager;
            this.gameManager = gameManager;
        }

        public void TryOpenShop<T>(string page, Action<T> onOpen, Action<FailureResult> onFailure) where T : GameState
        {
            KeepAliveHelper.SetCurrentPage(page);

            var waitPopup = Base.gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);

            ValidateShopOpening(
                onSuccess: () => { waitPopup.Close(); onOpen.Invoke(gameManager.OpenPopup<T>()); },
                onFailure: (result) => { waitPopup.Close(); onFailure.Invoke(result); });
        }

        public void TryOpenShop<T>(string page, Action<T> onOpen) where T : GameState
        {
            TryOpenShop<T>(page, onOpen, HandleFailureResult);
        }

        public void TryOpenShopByPath<T>(string relativePath, string page, Action<T> onOpen) where T : GameState
        {
            TryOpenShopByPath<T>(relativePath, page, onOpen, HandleFailureResult);
        }

        public void TryOpenShopByPath<T>(string relativePath, string page, Action<T> onOpen, Action<FailureResult> onFailure) where T : GameState
        {
            KeepAliveHelper.SetCurrentPage(page);

            var waitPopup = Base.gameManager.OpenPopup<Popup_WaitBox>().Setup(ScriptLocalization.Message_General.Wait);

            ValidateShopOpening(
                onSuccess: () => { waitPopup.Close(); onOpen.Invoke(gameManager.OpenPopupByPath<T>(relativePath)); },
                onFailure: (result) => { waitPopup.Close(); onFailure.Invoke(result); });
        }

        public void HandleFailureResult(FailureResult failureResult)
        {
            if (marketManager is NoCafebazaarFallbackMarket noCafebazaarFallbackMarket)
            {
                noCafebazaarFallbackMarket.ShowErrorMessage();
            }
            else
            {
                switch (failureResult)
                {
                    case FailureResult.PriceQueryFailure:
                    case FailureResult.Timeout:
                        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(ScriptLocalization.Message_Shop.InternetRequirment, ScriptLocalization.UI_General.Ok, null, true, null);
                        break;
                }
            }
        }

        public void ValidateShopOpening(Action onSuccess, Action<FailureResult> onFailure)
        {
            var session = new Session(onSuccess, onFailure);

            if (PurchasePossibilityChecker.IsPurchasePossible())
            {
                shopCenter.QueryPrices(
                    onSuccess: session.TryFinishWithSuccess,
                    onFailure: () => session.TryFinishWithFailure(FailureResult.PriceQueryFailure));
            }
            else
                session.TryFinishWithFailure(FailureResult.PurchaseDisabled);

            gameManager.DelayCall(TIMEOUT_DELAY, () => session.TryFinishWithFailure(FailureResult.Timeout));
        }


        public bool IsValidToOpen(HardCurrencyPackage package)
        {
            return PurchasePossibilityChecker.IsPurchasePossible() && package.Price().IsValid();
        }

        public GolmoradShopCenter ShopCenter()
        {
            return shopCenter;
        }

    }
}