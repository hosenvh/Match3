using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Match3.Presentation.TextAdapting;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.ReferralMarketing
{

    public class GoalRewardDescriptionPanelController : MonoBehaviour
    {
        public Image iconImage;
        public TextAdapter descriptionText;


        public void Setup(Sprite icon, string description)
        {
            iconImage.sprite = icon;
            descriptionText.SetText(description);
        }
        

        public void Close()
        {
            gameObject.SetActive(false);
        }
        
        
        
    }

}


