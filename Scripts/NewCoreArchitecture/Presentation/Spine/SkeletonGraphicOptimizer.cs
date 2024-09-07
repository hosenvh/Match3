using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation
{
    [RequireComponent(typeof(SkeletonGraphic))]
    public class SkeletonGraphicOptimizer : MonoBehaviour
    {
        SkeletonGraphic skeletonGraphic;
        private void Start()
        {

            skeletonGraphic = GetComponent<SkeletonGraphic>();

            if (skeletonGraphic.AnimationState == null)
            {
                Debug.LogError($"skeletonGraphic.AnimationState is null in {this.gameObject.name}");
                return;
            }

            if (skeletonGraphic.startingLoop == false)
            {
                var animationState = skeletonGraphic.AnimationState;
                bool hasPlayingAnimation = false;

                foreach (var track in animationState.Tracks)
                    if (track.IsComplete == false || track.loop)
                        hasPlayingAnimation = true;

                if (hasPlayingAnimation == false)
                    StartCoroutine(DelayedFreeze());
            }

            skeletonGraphic.AnimationState.Start += UnFreeze;
            skeletonGraphic.AnimationState.Interrupt += UnFreeze;
            // WARNING: Complete is called for looping animation on each loop.
            // TODO: Fix this.
            skeletonGraphic.AnimationState.Complete += Freeze;
        }

        private void UnFreeze(TrackEntry trackEntry)
        {
            StopAllCoroutines();
            skeletonGraphic.freeze = false;
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

            skeletonGraphic.freeze = true;
        }
    }
}