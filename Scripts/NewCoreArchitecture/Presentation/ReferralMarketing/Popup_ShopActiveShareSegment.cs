using System;
using I2.Loc;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.ReferralMarketing.Segments;
using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Match3.Presentation.ReferralMarketing
{
    
    public class Popup_ShopActiveShareSegment : GameState
    {
        public TextAdapter messageText;

        public Button shareButton;

        private ShareSegment shareSegment;

        public void Setup(ShareSegment shareSegment)
        {
            this.shareSegment = shareSegment;
        }

        public void ShareClick()
        {
            shareSegment.Share(reward =>
            {
                if (!(reward is EmptyReward))
                {
                    ServiceLocator.Find<EventManager>().Propagate(new UpdateGUIEvent(), this);
                    var hudPresentationController = FindObjectOfType<Popup_MainMenu>().hudPresentationController;
                    gameManager.OpenPopup<Popup_ClaimReward>()
                               .Setup(new[] {reward})
                               .OverrideHudControllerForDisappearingEffect(hudPresentationController)
                               .StartPresentingRewards();
                }
                
                Close();
            }, () =>
            {
            
            });
        }
        
        
    }

}

