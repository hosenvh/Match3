using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.PiggyBank;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{

    public class PiggyBankRewardCollectingTask : LevelEndingTask
    {
        private const int MaxShowFullBankMessage = 5;
        private int score;
        
        public PiggyBankRewardCollectingTask(int score)
        {
            this.score = score;
        }
        
        protected override void ExecuteLevelEndingTask(Action onComplete)
        {
            var piggyBankManager = ServiceLocator.Find<PiggyBankManager>();

            if (piggyBankManager.IsUnlocked()
                                          && (!piggyBankManager.IsBankFull() ||
                                              Popup_PiggyBankReward.FullBankModeShownCount < MaxShowFullBankMessage))
            {
                Base.gameManager.OpenPopup<Popup_PiggyBankReward>().Setup(score, onComplete);
            }
            else
                onComplete();
        }

    }

}