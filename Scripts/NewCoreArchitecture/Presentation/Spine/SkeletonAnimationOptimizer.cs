using Spine;
using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace Match3.Presentation
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SkeletonAnimationOptimizer : MonoBehaviour
    {
        SkeletonAnimation skeletonAnimation;
        private void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();

            if (skeletonAnimation.AnimationState == null)
            {
                Debug.LogError($"skeletonGraphic.AnimationState is null in {this.gameObject.name}");
                return;
            }

            if (skeletonAnimation.loop == false)
            {
                var animationState = skeletonAnimation.AnimationState;
                bool hasPlayingAnimation = false;

                foreach (var track in animationState.Tracks)
                    if (track.IsComplete == false || track.loop)
                        hasPlayingAnimation = true;

                if (hasPlayingAnimation == false)
                    StartCoroutine(DelayedFreeze());
            }

            skeletonAnimation.AnimationState.Start += UnFreeze;
            skeletonAnimation.AnimationState.Interrupt += UnFreeze;
            // WARNING: Complete is called for looping animation on each loop.
            // TODO: Fix this.
            skeletonAnimation.AnimationState.Complete += Freeze;
        }

        private void UnFreeze(TrackEntry trackEntry)
        {
            StopAllCoroutines();
            skeletonAnimation.enabled = true;
        }

        private void Freeze(TrackEntry trackEntry)
        {
            if (trackEntry.loop == false)
            {
                StopAllCoroutines();
                StartCoroutine(DelayedFreeze());
            }
        }

        IEnumerator DelayedFreeze()
        {
            // NOTE: It seems Spine blends current animation with previous animation. 
            // This only works when the mixDuration is 0 in spine.
            // It also seems even when mixDuration is 0, spine needs at least two frame to be rendered correctly.
            yield return null;
            yield return null;

            skeletonAnimation.enabled = false;
        }
    }
}