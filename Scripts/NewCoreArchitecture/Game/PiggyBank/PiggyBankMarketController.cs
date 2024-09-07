using System;
using System.Linq;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.ShopManagement;
using Medrick.Foundation.ShopManagement.Core;
using UnityEngine;

namespace Match3.Game.PiggyBank
{
    public class PiggyBankPurchaseResultGivingStartedEvent : PurchaseResultGivingEvent
    {
        public PiggyBankPurchaseResultGivingStartedEvent(PiggyBankPackage package) : base(package)
        {
        }
    }

    public class PiggyBankPurchaseResultGivingFinishedEvent : PurchaseResultGivingEvent
    {
        public PiggyBankPurchaseResultGivingFinishedEvent(PiggyBankPackage package) : base(package)
        {
        }
    }

    public class PiggyBankMarketController
    {
        private Action<PurchaseFailureState, CoinReward> onPurchaseFinished;
        private Action successfulPurchaseAction;
        private PiggyBankManager manager;
        private GolmoradShopCenter golmoradShopCenter;
        
        public PiggyBankMarketController(PiggyBankManager manager, GolmoradShopCenter golmoradShopCenter, Action successfulPurchaseAction)
        {
            this.manager = manager;
            this.successfulPurchaseAction = successfulPurchaseAction;
            this.golmoradShopCenter = golmoradShopCenter;
        }

        public PiggyBankPackage GetCurrentPackage()
        {
            var packages = golmoradShopCenter.PackagesIn(PiggyBankShopConsts.PIGGY_BANK_PACKAGE_CATEGORY).ToArray();

            try
            {
                return packages[Mathf.Min(packages.Length-1, manager.BankLevel() - 1)] as PiggyBankPackage;
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.LogError("PiggyBank Packages Couldn't found\n" + ex.StackTrace);
                return null;
            }
        }


        public void Purchase(PiggyBankPackage package, Action<CoinReward> onSuccess, Action<PurchaseFailureResult> onFailure)
        {
            var reward = new CoinReward(manager.CurrentSavedCoins()); 
            package.SetCoinReward(reward);
            ServiceLocator.Find<EventManager>().Propagate(new PiggyBankPurchaseResultGivingStartedEvent(package), this);
            golmoradShopCenter.Purchase(
                package,
                onSuccess: HandlePurchaseSuccess,
                onFailure: HandlePurchaseFailure);


            void HandlePurchaseSuccess(PurchaseSuccessResult result)
            {
                successfulPurchaseAction.Invoke();
                onSuccess.Invoke(reward);
                ServiceLocator.Find<EventManager>().Propagate(new PiggyBankPurchaseResultGivingFinishedEvent(package), this);
            }

            void HandlePurchaseFailure(PurchaseFailureResult result)
            {
                onFailure.Invoke(result);
                ServiceLocator.Find<EventManager>().Propagate(new PiggyBankPurchaseResultGivingFinishedEvent(package), this);
            }

        }
    }
}