using UnityEngine;
using Match3.Game.ShopManagement;
using Match3.Presentation.TextAdapting;
using System;
using Match3.Game;
using Match3.Main;
using Match3.Utility.GolmoradLogging;
using SeganX;

namespace Match3.Presentation.ShopManagement
{
    [Serializable]
    public class MultiRewardPackageItemContent
    {
        public TextAdapter amountText;
        public GameObject rootObject;
    }

    public class MultiRewardPackagePresenter<T> : ShopPackagePresenter<T> where T : MultiRewardHardCurrencyPackage
    {


        [SerializeField] TextAdapter[] boosterCountTexts = default;
        [SerializeField] TextAdapter coinCountText = default;

        [SerializeField] MultiRewardPackageItemContent hammerPowerUpItem;
        [SerializeField] MultiRewardPackageItemContent handPowerUpItem;
        [SerializeField] MultiRewardPackageItemContent infinityLifeItem;

        protected override void InternalSetup(T package)
        {
            DisableAllRewardComponents();

            foreach (var reward in package.Rewards())
                SetupReward(reward);
        }

        private void DisableAllRewardComponents()
        {
            coinCountText.gameObject.SetActive(false);
            Array.ForEach(boosterCountTexts, text => text.gameObject.SetActive(false));
            hammerPowerUpItem.rootObject.SetActive(false);
            handPowerUpItem.rootObject.SetActive(false);
            infinityLifeItem.rootObject.SetActive(false);
        }

        private void SetupReward(Reward reward)
        {
            if (reward == null)
                return;

            switch(reward)
            {
                case CoinReward coinReward:
                    coinCountText.gameObject.SetActive(true);
                    coinCountText.SetText(coinReward.count.ToString());
                    break;
                case InfiniteLifeReward infiniteLifeReward:
                    ActivateItem(infinityLifeItem, Utilities.GetLocalizedFormattedTime(infiniteLifeReward.DurationTime, false));
                    break;
                case HandPowerUpReward handPowerUpReward:
                    ActivateItem(handPowerUpItem, handPowerUpReward.count.ToString());
                    break;
                case HammerPowerUpReward hammerPowerUpReward:
                    ActivateItem(hammerPowerUpItem, hammerPowerUpReward.count.ToString());
                    break;
                case AllBoostersReward allBoostersReward:
                    for (int i = 0; i < boosterCountTexts.Length; i++)
                    {
                        boosterCountTexts[i].gameObject.SetActive(true);
                        boosterCountTexts[i].SetText(allBoostersReward.count.ToString());
                    }
                    break;

                default:
                    DebugPro.LogError<ShopLogTag>($"Presentation for reward of type {reward.GetType()} is not defined");
                    break;
            }
        }

        private void ActivateItem(MultiRewardPackageItemContent itemContent, string text)
        {
            itemContent.rootObject.SetActive(true);
            itemContent.amountText.SetText(text);
        }
    }
}
