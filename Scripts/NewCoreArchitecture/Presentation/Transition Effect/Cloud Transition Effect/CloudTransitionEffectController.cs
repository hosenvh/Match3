using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;


namespace Match3.Presentation.TransitionEffects
{

    public class CloudTransitionEffectController : BaseTransitionEffectController
    {
        public Animation skyAnimation;
        public SkeletonGraphic cloudsSpine;
        
        [Space(10)]
        [SpineAnimation(dataField: "skeletonDataAsset")]
        [SerializeField] private string cloudsInSpineClip;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        [SerializeField] private string cloudsOutSpineClip;

        [Space(10)] 
        [SerializeField] private AnimationClip skyFadeInAnimationClip;
        [SerializeField] private AnimationClip skyFadeOutAnimationClip;


        private Action onFadeInFinished;
        private Action onFadeOutFinished;
        
        
        
        public override void StartTransition(Action onFadeInFinished, Action onFadeOutFinished)
        {
            this.onFadeInFinished = onFadeInFinished;
            this.onFadeOutFinished = onFadeOutFinished;
            FadeIn();
        }
        
        protected override void FadeIn()
        {
            skyAnimation.clip = skyFadeInAnimationClip;
            skyAnimation.Play();
            cloudsSpine.AnimationState.ClearTracks();
            cloudsSpine.Skeleton.SetToSetupPose();
            cloudsSpine.AnimationState.SetAnimation(trackIndex: 0, cloudsInSpineClip, false);
            cloudsSpine.AnimationState.Complete += FadeInComplete;
            cloudsSpine.color = new Color(0,0,0,0);
            StartCoroutine(SetColorOfSpine());
        }

        private IEnumerator SetColorOfSpine()
        {
            yield return null;
            cloudsSpine.color = Color.white;
        }

        private void FadeInComplete(Spine.TrackEntry trackEntry)
        {
            cloudsSpine.AnimationState.Complete -= FadeInComplete;
            onFadeInFinished.Invoke();
            FadeOut();
        }
        
        protected override void FadeOut()
        {
            skyAnimation.clip = skyFadeOutAnimationClip;
            skyAnimation.Play();
            cloudsSpine.AnimationState.SetAnimation(trackIndex: 0, cloudsOutSpineClip, false);
            cloudsSpine.AnimationState.Complete += entry => onFadeOutFinished.Invoke();
        }

        
    }
    
}


