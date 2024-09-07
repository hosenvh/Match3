using System;
using System.Collections.Generic;
using DynamicSpecialOfferSpace;
using Match3.Game;
using Match3.Presentation;
using Match3.Presentation.Shop;
using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine;


public class Popup_DynamicSpecialOffer : GameState
{

    [Serializable]
    public class FeatureArtPack
    {
        public GameObject[] featureArtObjects;

        public void Active()
        {
            foreach (var featureArtObject in featureArtObjects)
            {
                featureArtObject.SetActive(true);
            }
        }
    }
    
    [SerializeField]
    private List<BundleRewardPresentationController> rewardPresenters = new List<BundleRewardPresentationController>();
    
    [Space(10)]
    public TextAdapter realPriceText;
    public TextAdapter discountPriceText;
    public TextAdapter offPercentText;

    [Space(10)] 
    public FeatureArtPack[] featureArtPacks;

    private Action onPurchaseClicked;
    private Action onCloseClicked;

    
    
    public Popup_DynamicSpecialOffer Setup(DynamicSpecialOfferPackage offer, int featureArtIndex, Action onPurchaseClicked, Action onCloseClicked)
    {
        this.onCloseClicked = onCloseClicked;
        this.onPurchaseClicked = onPurchaseClicked;
        
        var decomposedRewards = GetDecomposedRewards(offer.Rewards());
        SetupRewardPresenters(decomposedRewards);
        
        realPriceText.SetText(offer.DiscountInfo().BeforeDiscountPrice().FormatMoneyToString());
        discountPriceText.SetText(offer.Price().FormatMoneyToString());
        offPercentText.SetText(offer.Tag());
        
        featureArtPacks[Mathf.Clamp(featureArtIndex, 0, featureArtPacks.Length)].Active();

        return this;
    }


    private List<Reward> GetDecomposedRewards(IEnumerable<Reward> rewards)
    {
        List<Reward> decomposedRewards = new List<Reward>();
        foreach (var reward in rewards)
        {
            if (reward is AllBoostersReward allBoostersReward)
            {
                decomposedRewards.Add(new DoubleBombBoosterReward(allBoostersReward.count));
                decomposedRewards.Add(new RainbowBoosterReward(allBoostersReward.count));
                decomposedRewards.Add(new TntRainbowBoosterReward(allBoostersReward.count));
            }
            else
                decomposedRewards.Add(reward);
        }

        return decomposedRewards;
    }

    private void SetupRewardPresenters(IEnumerable<Reward> rewards)
    {
        foreach (var reward in rewards)
        {
            foreach (var rewardPresenter in rewardPresenters)
            {
                if (rewardPresenter.RewardType == reward.GetType())
                {
                    rewardPresenter.gameObject.SetActive(true);
                    
                    int rewardCount;
                    if (reward is InfiniteLifeReward)
                        rewardCount = reward.count / 60; // Make it base on hours
                    else
                        rewardCount = reward.count;
                    rewardPresenter.Setup(rewardCount);
                }
            }
        }
    }

    public override void Back()
    {
        base.Back();
        onCloseClicked();
    }

    public void OnPurchaseButtonClick()
    {
        onPurchaseClicked();
    }
    
}
