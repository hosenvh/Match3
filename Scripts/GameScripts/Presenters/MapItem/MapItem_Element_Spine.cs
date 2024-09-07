using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapItem_Element_Spine : MapItem_Element
{
    public bool shouldLoopAnimation;

    bool isShowing = false;
    Action onFinish;
    Spine.Unity.SkeletonGraphic skeletonGraphic = null;
    int currentStateIndex;

    private void Awake()
    {
        if (skeletonGraphic == null)
            skeletonGraphic = GetComponent<Spine.Unity.SkeletonGraphic>();
        if (!isShowing)
            SetActiveAll(false);
    }

    public override void ShowElementState(int index, bool withAnimation, Action onFinish)
    {
        this.onFinish = onFinish;
        if (index > 0)
        {
            isShowing = true;
            SetActiveAll(true);
        }
        if (isShowing)
            StartCoroutine(ShowSkeletonGraphic(index, currentStateIndex, withAnimation));
    }

    IEnumerator ShowSkeletonGraphic(int index, int prevIndex, bool withAnimation)
    {
        Spine.Animation spineAnimation = null;
        if (skeletonGraphic == null)
            skeletonGraphic = GetComponent<Spine.Unity.SkeletonGraphic>();
        if (skeletonGraphic.SkeletonData == null)
            yield return new WaitForEndOfFrame();
        spineAnimation = skeletonGraphic.SkeletonData.FindAnimation(prevIndex.ToString() + "_" + index.ToString());

        if (spineAnimation != null && withAnimation)
        {
            skeletonGraphic.AnimationState.ClearTracks();
            skeletonGraphic.Skeleton.SetToSetupPose();
            skeletonGraphic.AnimationState.SetAnimation(0, spineAnimation.name, false);
            if (onFinish != null)
                skeletonGraphic.AnimationState.Complete += AnimationState_Complete;
        }
        else
        {
            spineAnimation = GetComponent<Spine.Unity.SkeletonGraphic>().SkeletonData.FindAnimation(index.ToString());

            if (spineAnimation != null)
            {
                skeletonGraphic.AnimationState.ClearTracks();
                skeletonGraphic.Skeleton.SetToSetupPose();
                skeletonGraphic.AnimationState.SetAnimation(0, spineAnimation.name, shouldLoopAnimation);
                if (onFinish != null)
                    skeletonGraphic.AnimationState.Complete += AnimationState_Complete3;
            }
            else
            {
                if (index == 0)
                {
                    isShowing = false;
                    SetActiveAll(false);

                    if (onFinish != null)
                        onFinish();
                }
            }
        }

        currentStateIndex = index;
    }

    private void AnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        skeletonGraphic.AnimationState.Complete -= AnimationState_Complete;

        Spine.Animation spineAnimation;
        spineAnimation = GetComponent<Spine.Unity.SkeletonGraphic>().SkeletonData.FindAnimation(currentStateIndex.ToString());
        if (spineAnimation != null)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, spineAnimation.name, shouldLoopAnimation);
            if (onFinish != null)
                skeletonGraphic.AnimationState.Complete += AnimationState_Complete2;
        }
        else
        {
            if (onFinish != null)
                onFinish();
        }
    }

    private void AnimationState_Complete2(Spine.TrackEntry trackEntry)
    {
        skeletonGraphic.AnimationState.Complete -= AnimationState_Complete2;

        if (onFinish != null)
            onFinish();
    }

    private void AnimationState_Complete3(Spine.TrackEntry trackEntry)
    {
        skeletonGraphic.AnimationState.Complete -= AnimationState_Complete3;

        if (onFinish != null)
            onFinish();
    }
}