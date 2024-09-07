using System;
using System.Collections;
using UnityEngine;

public class MapItem_Element_SpineAnimation : MapItem_Element
{
    public bool shouldLoopAnimation;

    bool isShowing = false;
    Action onFinish;
    Spine.Unity.SkeletonAnimation skeletonAnimation = null;
    int currentStateIndex;

    private void Awake()
    {

        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<Spine.Unity.SkeletonAnimation>();
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
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<Spine.Unity.SkeletonAnimation>();
        if (skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false) == null)
            yield return new WaitForEndOfFrame();
        spineAnimation = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(false).FindAnimation(prevIndex.ToString() + "_" + index.ToString());

        if (spineAnimation != null && withAnimation)
        {
            skeletonAnimation.AnimationState.ClearTracks();
            skeletonAnimation.Skeleton.SetToSetupPose();
            skeletonAnimation.AnimationState.SetAnimation(0, spineAnimation.name, false);
            if (onFinish != null)
                skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        }
        else
        {
            spineAnimation = GetComponent<Spine.Unity.SkeletonAnimation>().SkeletonDataAsset.GetSkeletonData(false).FindAnimation(index.ToString());

            if (spineAnimation != null)
            {
                skeletonAnimation.Skeleton.SetToSetupPose();
                skeletonAnimation.AnimationState.ClearTracks();
                skeletonAnimation.AnimationState.SetAnimation(0, spineAnimation.name, shouldLoopAnimation);
                if (onFinish != null)
                    skeletonAnimation.AnimationState.Complete += AnimationState_Complete3;
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
        skeletonAnimation.AnimationState.Complete -= AnimationState_Complete;

        Spine.Animation spineAnimation;
        spineAnimation = GetComponent<Spine.Unity.SkeletonAnimation>().SkeletonDataAsset.GetSkeletonData(false).FindAnimation(currentStateIndex.ToString());
        if (spineAnimation != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, spineAnimation.name, shouldLoopAnimation);
            if (onFinish != null)
                skeletonAnimation.AnimationState.Complete += AnimationState_Complete2;
        }
        else
        {
            if (onFinish != null)
                onFinish();
        }
    }

    private void AnimationState_Complete2(Spine.TrackEntry trackEntry)
    {
        skeletonAnimation.AnimationState.Complete -= AnimationState_Complete2;

        if (onFinish != null)
            onFinish();
    }

    private void AnimationState_Complete3(Spine.TrackEntry trackEntry)
    {
        skeletonAnimation.AnimationState.Complete -= AnimationState_Complete3;

        if (onFinish != null)
            onFinish();
    }
}
