using Match3.Game;
using Match3.Game.NeighborhoodChallenge;
using SeganX;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;


public class Popup_NeighborhoodChallengeGiftsInfo : GameState
{
    
    [SerializeField] private Transform contentParent = default;
    [SerializeField] private NeighborhoodChallengeRewardInfoBoxController rewardInfoBox = default;
    [SerializeField] private NeighborhoodChallengeRewardPresentationController rewardPresenter = default;
    
    [Space(10)] 
    [SerializeField] private Sprite doubleBombBoosterIcon = default;
    [SerializeField] private Sprite rainbowBoosterIcon = default;
    [SerializeField] private Sprite tntRainbowBoosterIcon = default;
    [SerializeField] private Sprite coinIcon = default;

    [Space(10)] 
    [SerializeField] private ContentSizeFitter contentSizeFitter = default;
    
    
    
    public void Setup(List<ChallengeRankingReward> rewards)
    {
        foreach (var challengeReward in rewards)
        {
            var rewardBox = Instantiate(rewardInfoBox, contentParent);
            var titleP1 = ScriptLocalization.UI_Misc.Rewards;
            var titleP2 = challengeReward.start == challengeReward.end || challengeReward.end==-1 ? ScriptLocalization.UI_Misc.Person : ScriptLocalization.UI_Misc.Persons;
            var titleP3 = "";
            if (challengeReward.end == -1)
                titleP3 = challengeReward.start.ToString() + ScriptLocalization.UI_Misc.ToTheEnd;
            else
                titleP3 = challengeReward.start == challengeReward.end ? challengeReward.start.ToString() : challengeReward.start + ScriptLocalization.UI_Misc.To + challengeReward.end;
            
            rewardBox.SetTitle(titleP1, titleP2, titleP3);

            foreach (var reward in challengeReward.rewards)
            {
                var rPresenter = Instantiate(rewardPresenter, rewardBox.transform);
                Sprite rewardIcon = null;
                
                bool haveX = true;

                switch (reward)
                {
                    case DoubleBombBoosterReward _:
                        rewardIcon = doubleBombBoosterIcon;
                        break;
                    case RainbowBoosterReward _:
                        rewardIcon = rainbowBoosterIcon;
                        break;
                    case TntRainbowBoosterReward _:
                        rewardIcon = tntRainbowBoosterIcon;
                        break;
                    case CoinReward _:
                        rewardIcon = coinIcon;
                        haveX = false;
                        break;
                }

                rPresenter.Setup(rewardIcon, reward.count.ToString("N0"), haveX);
            }
        }
        
        Canvas.ForceUpdateCanvases();
        contentSizeFitter.SetLayoutVertical();
    }


    public override void Back()
    {
        base.Back();
        gameManager.fxPresenter.PlayClickAudio();
    }
    
    
}
