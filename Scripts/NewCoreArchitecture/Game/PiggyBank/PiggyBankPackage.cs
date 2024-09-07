using Match3.Game.ShopManagement;
using System;

namespace Match3.Game.PiggyBank
{
    public static class PiggyBankShopConsts
    {
        public const string PIGGY_BANK_PACKAGE_CATEGORY = "PiggyBankShop";
    }


    [Serializable]
    public class PiggyBankPackage : HardCurrencyPackage
    {
        private CoinReward coinReward = default;

        public void SetCoinReward(CoinReward coinReward)
        {
            this.coinReward = coinReward;
        }

        public override void Apply()
        {
            coinReward.Apply();
        }
    }
}