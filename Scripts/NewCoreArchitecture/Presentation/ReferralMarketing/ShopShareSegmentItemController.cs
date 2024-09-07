using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Match3.Game;
using Match3.Presentation.TextAdapting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Match3.Presentation.ReferralMarketing
{
    
    public class ShopShareSegmentItemController : MonoBehaviour
    {
        public Button shareButton;

        public LocalizedStringTerm comebackTomorrowString;
        public LocalizedStringTerm shareString;
    
        public TextAdapter rewardCountText;
        public TextAdapter shareButtonText;

        public ShopShareSegmentItemController Setup(Reward reward, UnityAction onShareButtonPressed)
        {
            rewardCountText.SetText(reward.count.ToString());
            shareButton.onClick.AddListener(onShareButtonPressed);
            return this;
        }
    
        public void SetActiveMode()
        {
            shareButton.interactable = true;
            shareButtonText.SetText(shareString);
        }

        public void SetOutOfQuotaMode()
        {
            shareButton.interactable = false;
            shareButtonText.SetText(comebackTomorrowString);
        }
    }
    
}

