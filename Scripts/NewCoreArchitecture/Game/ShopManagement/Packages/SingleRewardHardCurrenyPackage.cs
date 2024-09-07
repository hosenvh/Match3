using System;
using UnityEngine;

namespace Match3.Game.ShopManagement
{
    [Serializable]
    public class SingleRewardHardCurrenyPackage<T> : HardCurrencyPackage where T : Reward
    {
        [SerializeField] public T reward;

        public override void Apply()
        {
            reward.Apply();
        }

        public T Reward()
        {
            return reward;
        }
    }
}