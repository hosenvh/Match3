using System.Collections;
using UnityEngine;


namespace Match3.Presentation.PiggyBank
{

    public class PiggyBankFullNotifBubbleController : MonoBehaviour
    {
        public GameObject bubbleObject;
        public Animation bubbleAnimation;
        public AnimationClip bubbleAppearAnimationClip;
        public AnimationClip bubbleDisappearAnimationClip;
        public float showDelay;
        public float presentingTime;

        
        public void Show()
        {
            StartCoroutine(cShow());
        }

        private IEnumerator cShow()
        {
            yield return new WaitForSeconds(showDelay);
            bubbleObject.SetActive(true);
            bubbleAnimation.clip = bubbleAppearAnimationClip;
            bubbleAnimation.Play();
            StartCoroutine(CloseBubble());
        }
        
        public void InstantClose()
        {
            StopAllCoroutines();
            bubbleObject.SetActive(false);
        }
        
        private IEnumerator CloseBubble()
        {
            yield return new WaitForSeconds(presentingTime);
            bubbleAnimation.clip = bubbleDisappearAnimationClip;
            bubbleAnimation.Play();
            yield return new WaitForSeconds(bubbleDisappearAnimationClip.length);
            bubbleObject.SetActive(false);
        }
        
    }

}