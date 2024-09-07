using System;
using UnityEngine;
using UnityEngine.UI;



namespace Match3.Presentation.Shop
{
    public class ShopMoreItem : MonoBehaviour
    {
        private static readonly int Appear = Animator.StringToHash("Appear");
        private static readonly int Hide = Animator.StringToHash("Hide");
        
        [SerializeField] private Button moreButton = null;
        [Space(10)] 
        [SerializeField] private Animator animator;
        
        
        public void Setup(Action onMoreButtonClicked)
        {
            moreButton.onClick.AddListener(() => onMoreButtonClicked());
        }
        
        
        public void PlayAppearAnimation()
        {
            animator.SetTrigger(Appear);
        }
        
        public void PlayHideAnimation()
        {
            animator.SetTrigger(Hide);
        }
        
    }
}


