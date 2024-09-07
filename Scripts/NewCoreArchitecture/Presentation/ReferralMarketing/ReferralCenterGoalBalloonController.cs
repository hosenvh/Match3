using System;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;



namespace Match3.Presentation.ReferralMarketing
{

    public class ReferralCenterGoalBalloonController : MonoBehaviour
    {

        public Image numberBackImage;
        public Image rewardIconImage;
        public Image rewardShinyIconImage;

        public GameObject shinyIconObject;
        
        public Button interactionButton;

        [Header("Claim Status Fields")]
//        public Animation bounceAnimation;
        public UIShiny shinyEffect;
        public Sprite numberBackClaimSprite;
        public GameObject claimableEffectObject;
        
        [Header("Claimed Status Fields")] 
        public float smallScale;
        public UIEffect iconGrayEffect;
        public Sprite numberBackClaimedSprite;

        public int goalId { private set; get; }
        private Action<int> onBalloonClick;

        
        public void Init(int goalId, Sprite icon, Action<int> onBalloonClick)
        {
            this.goalId = goalId;
            this.onBalloonClick = onBalloonClick;
            rewardIconImage.sprite = icon;
            rewardShinyIconImage.sprite = icon;
            interactionButton.onClick.AddListener(OnBalloonClick);
        }
        
        
        public void OnBalloonClick()
        {
            Base.gameManager.fxPresenter.PlayClickAudio();
            onBalloonClick(goalId);
        }
        
        public void SetUnreachedGoalMode()
        {
            // No operation yet
        }

        public void SetReachedUnclaimedMode()
        {
//            bounceAnimation.Play();
            shinyIconObject.SetActive(true);
            shinyEffect.Play();
            numberBackImage.sprite = numberBackClaimSprite;
            claimableEffectObject.SetActive(true);
        }

        public void SetReachedClaimedMode()
        {
            ((RectTransform) transform).localScale = new Vector3(smallScale, smallScale, 1);
            shinyEffect.Stop();
            shinyIconObject.SetActive(false);
            iconGrayEffect.effectFactor = 1;
//            bounceAnimation.Stop();
            numberBackImage.sprite = numberBackClaimedSprite;
            claimableEffectObject.SetActive(false);
        }
        
    }


}

