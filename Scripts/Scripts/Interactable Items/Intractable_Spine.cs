using System;
using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;


[Serializable]
public struct SpineAnimationSequence
{
    [SpineAnimation(dataField: "skeletonDataAsset")]
    public string fromAnimationClipName;
    [SpineAnimation(dataField: "skeletonDataAsset")]
    public string toAnimationClipName;
}


[RequireComponent(typeof(SkeletonAnimation))]
public class Intractable_Spine : Intractable
{
    
    // ----------------------------------------------- Public Fields ----------------------------------------------- \\ 
    
    public SpineAnimationSequence[] animationsSequence;

    // ----------------------------------------------- Private Fields ----------------------------------------------- \\

    private bool animationIsPlaying = false;
    
//    private int currentAnimationSequenceIndex = -1;
    private SkeletonAnimation skeletonAnimation = null;

    
    // ================================================================================= \\


    protected override void Awake()
    {
        base.Awake();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    

    protected override void InternalClickDown()
    {
        // Nothing
    }

    protected override void InternalClickUp()
    {
        if (animationIsPlaying) return;
        StartCoroutine(PlayNextAnimation());
    }

    
    private IEnumerator PlayNextAnimation()
    {
        animationIsPlaying = true;

        CurrentStateIndex++;
        if (CurrentStateIndex >= animationsSequence.Length)
            CurrentStateIndex = 0;

        if (skeletonAnimation.AnimationState == null)
            yield return new WaitForEndOfFrame();
        
        skeletonAnimation.AnimationState.SetAnimation(0,
            animationsSequence[CurrentStateIndex].fromAnimationClipName, false);
        skeletonAnimation.AnimationState.Complete += CompleteFromAnimation;
        
        SaveState();
    }
    

    private void CompleteFromAnimation(TrackEntry trackEntry)
    {
        skeletonAnimation.AnimationState.Complete -= CompleteFromAnimation;
        
        Spine.Animation toAnimation =
            skeletonAnimation.AnimationState.Data.SkeletonData.FindAnimation(animationsSequence[CurrentStateIndex]
                .toAnimationClipName);

        if (toAnimation != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, toAnimation, false);
            skeletonAnimation.AnimationState.Complete += CompleteToAnimation;
        }
        else
        {
            animationIsPlaying = false;
        }
    }

    private void CompleteToAnimation(TrackEntry trackEntry)
    {
        skeletonAnimation.AnimationState.Complete -= CompleteToAnimation;
        animationIsPlaying = false;
    }
    

    protected override void InternalSaveState()
    {
        // Nothing
    }

    protected override void InternalLoadState()
    {
        string animationName = "";
        animationName = !string.IsNullOrEmpty(animationsSequence[CurrentStateIndex].toAnimationClipName)
            ? animationsSequence[CurrentStateIndex].toAnimationClipName
            : animationsSequence[CurrentStateIndex].fromAnimationClipName;
        
        skeletonAnimation.AnimationState.SetAnimation(0, animationName, false);
    }
}
