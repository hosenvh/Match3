using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using Match3.Foundation.Base.MoneyHandling;

namespace Match3
{
    // TODO: This must be change to a generic fallback for other markets too.
    public class NoCafebazaarFallbackMarket : MarketManager
    {
        public override bool ShouldCheckPurchasePossibility()
        {
            return false;
        }

        public override string GetMarketName()
        {
            return this.GetType().Name;
        }

        protected override void InternalInit()
        {
            SetInitialized();
        }

        public override void QueryPrices(ICollection<string> skus, Action<string, Money> priceQueryCallback, Action onSuccess, Action onFailure)
        {
            onFailure.Invoke();
        }

        protected override void InternalPurchase(string sku)
        {
            FinishSessionAsFailure(PurchaseFailureState.PurchaseFailed);
        }

        public void ShowErrorMessage()
        {
            if (Base.gameManager == null)
                return;

            Base.gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
                ScriptLocalization.Message_Purchase.NeedBazaarToPurchase, 
                ScriptLocalization.UI_General.Download, 
                ScriptLocalization.UI_General.Later, 
                true,
                (confirm) => { if (confirm) OpenCafebazaarAndQuit(); });
        }

        private void OpenCafebazaarAndQuit()
        {
            Application.OpenURL("https://cafebazaar.ir/");

            // NOTE: It seems quiting right after [returning from] OpenURL doesn't work properly. Setting a delay seems to fix the problem 
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(0.2f, Application.Quit, this);
        }
    }
}