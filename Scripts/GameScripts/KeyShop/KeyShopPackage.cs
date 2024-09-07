using Match3.Game.ShopManagement;
using System;

namespace Match3.Game.KeyShop
{
    public static class KeyShopConsts
    {
        public const string KEY_PACKAGE_CATEGORY = "KeyShop";
    }

    [Serializable]
    public class KeyShopPackage : SingleRewardHardCurrenyPackage<KeyReward>
    {
        public int KeyCount()
        {
            return reward.count;
        }
    }

}
