using I2.Loc;
using Match3.Game;
using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Match3.Presentation.ReferralMarketing
{

    public class Popup_EndGameActiveShareSegment : GameState
    {
        public LocalizedStringTerm description;
        public TextAdapter descriptionText;
        public Button shareButton;
        
        public void Setup(Reward reward, UnityAction onShareButtonClick)
        {
            descriptionText.SetText(description.ToString());
            shareButton.onClick.AddListener(onShareButtonClick);
        }
        
    }

}


