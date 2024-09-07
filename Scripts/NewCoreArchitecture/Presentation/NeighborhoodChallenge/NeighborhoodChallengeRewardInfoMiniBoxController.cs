using System;
using System.Collections.Generic;
using UnityEngine;



public class NeighborhoodChallengeRewardInfoMiniBoxController : MonoBehaviour
{

    public Transform rewardParent;
    public NeighborhoodChallengeRewardPresentationController miniRewardPresenter;
    private readonly List<NeighborhoodChallengeRewardPresentationController> myRewardPresenters = new List<NeighborhoodChallengeRewardPresentationController>();

    
    public void AddReward(Sprite rewardSpr, int rewardCount, bool haveX)
    {
        foreach (var rewardPresenter in myRewardPresenters)
        {
            if (rewardPresenter.gameObject.activeSelf) continue;
            rewardPresenter.gameObject.SetActive(true);
            rewardPresenter.Setup(rewardSpr, rewardCount.ToString("N0"), haveX);
            return;
        }

        var newRewardPresenter = Instantiate(miniRewardPresenter, rewardParent);
        myRewardPresenters.Add(newRewardPresenter);
        newRewardPresenter.Setup(rewardSpr, rewardCount.ToString("N0"), haveX);
    }

    public void ResetBox()
    {
        foreach (var rewardPresenter in myRewardPresenters)
        {
            rewardPresenter.gameObject.SetActive(false);
        }
    }
    
    
    
}
