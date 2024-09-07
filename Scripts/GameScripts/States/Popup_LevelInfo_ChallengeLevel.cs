using System;
using I2.Loc;
using Match3.Presentation.TextAdapting;
using UnityEngine;


public class Popup_LevelInfo_ChallengeLevel : Popup_LevelInfo
{
    
    public LocalizedStringTerm infinityTimeUnit;
    public LocalizedStringTerm coinCountUnit;
    
    [Space(15)]
    public TextAdapter ChallengeLevelInfinityLifeTimeText;
    public TextAdapter ChallengeLevelCoinCountText;
    public TextAdapter ChallengeLevelInfinityBombTimeText;
    
    
    public new void Setup(BoardConfig boardConfig, int levelIndex, Action onPurchaseFinish, Action onPlay)
    {
        base.Setup(boardConfig, levelIndex, onPurchaseFinish, onPlay);
        ChallengeLevelInfinityLifeTimeText.SetText(string.Format(infinityTimeUnit,
            boardConfig.challengeLevelConfig.infiniteLifeReward.count.ToString()));
        ChallengeLevelCoinCountText.SetText(string.Format(coinCountUnit,
            boardConfig.challengeLevelConfig.coinReward.count.ToString()));
        ChallengeLevelInfinityBombTimeText.SetText(string.Format(infinityTimeUnit,
            boardConfig.challengeLevelConfig.infiniteDoubleBombBoosterReward1.count.ToString()));

        TryStartTutorial();
    }

    private void TryStartTutorial()
    {
        gameManager.tutorialManager.CheckThenShowTutorial(
            88, 
            0,
            onHide: () => gameManager.tutorialManager.CheckThenShowTutorial(
                89, 
                0, 
                null));
    }
}
