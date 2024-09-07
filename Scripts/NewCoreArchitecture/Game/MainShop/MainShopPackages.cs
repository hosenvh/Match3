using Match3.Game.ShopManagement;
using System;
using UnityEngine;

namespace Match3.Game.MainShop
{
    [Serializable]
    public class ShopCoinPackage : SingleRewardHardCurrenyPackage<CoinReward>
    {
    }

    [Serializable]
    public class ShopBundlePackage : MultiRewardHardCurrencyPackage
    {
    }


}