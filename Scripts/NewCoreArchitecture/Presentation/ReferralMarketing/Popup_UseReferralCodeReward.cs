using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Match3.Game;
using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine;


namespace Match3.Presentation.ReferralMarketing
{

    public class Popup_UseReferralCodeReward : GameState
    {
        public TextAdapter messageText;
        public LocalizedStringTerm message;
        public LocalizedStringTerm friendLocalizedWord;
        
        private Action onConfirm;
        
        public void Setup(Reward reward, string inviterName, Action onConfirm)
        {
            this.onConfirm = onConfirm;
            if (inviterName.IsNullOrEmpty())
                inviterName = friendLocalizedWord;
            messageText.SetText(string.Format(message, reward.count, inviterName));
        }

        public override void Back()
        {
            onConfirm();
            base.Back();
        }
    }

}