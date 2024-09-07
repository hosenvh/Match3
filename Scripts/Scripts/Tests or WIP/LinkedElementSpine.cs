using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

[DisallowMultipleComponent]
public class LinkedElementSpine : MonoBehaviour
{
    
    // ------------------------------------------------ Properties Fields ------------------------------------------------ \\
    
    [field: SerializeField] public SkeletonGraphic SkeletonGraphic { get; private set; }
    [field: SerializeField] public RectTransform RectTransform { get; private set; }

    // ------------------------------------------------ Private Fields ------------------------------------------------ \\
    
    private Action onFinish;
    private int currentStateIndex;
    private bool isSkeletonGraphicNull;

    // ================================================================================================================ \\
    
    
    private void Awake()
    {
        FindComponents();
    }

    public void StartAnimation(int index, Action onFinish)
    {
        this.onFinish = onFinish;
        StartCoroutine(CStartAnimation(index));
    }

    IEnumerator CStartAnimation(int index)
    {
        void PlaySpine(string animationName, bool loop, AnimationState.TrackEntryDelegate onComplete)
        {
            SkeletonGraphic.AnimationState.ClearTracks();
            SkeletonGraphic.Skeleton.SetToSetupPose();
            SkeletonGraphic.AnimationState.SetAnimation(0, animationName, loop);
            if (onFinish != null && onComplete!=null)
                SkeletonGraphic.AnimationState.Complete += onComplete;
        }
        
        if (isSkeletonGraphicNull) yield break;
        if (SkeletonGraphic.SkeletonData == null)
            yield return new WaitForEndOfFrame();

        var spineAnimation = SkeletonGraphic.SkeletonData?.FindAnimation(currentStateIndex + "_" + index);
        if (spineAnimation != null)
        {
            PlaySpine(spineAnimation.name, false, AnimationState_Transition_Complete);
        }
        else
        {
            spineAnimation = SkeletonGraphic.SkeletonData?.FindAnimation(index.ToString());
            if (spineAnimation != null)
            {
                PlaySpine(spineAnimation.name, true, AnimationState_Normal_Complete);
            }
            else
            {
                if (index == 0)
                {
                    onFinish?.Invoke();
                }
            }
        }

        currentStateIndex = index;
    }

    
#region Spine Animation CallBacks

    private void AnimationState_Transition_Complete(Spine.TrackEntry trackEntry)
    {
        SkeletonGraphic.AnimationState.Complete -= AnimationState_Transition_Complete;
        
        var spineAnimation = SkeletonGraphic.SkeletonData.FindAnimation(currentStateIndex.ToString());
        if (spineAnimation != null)
        {
            SkeletonGraphic.AnimationState.SetAnimation(0, spineAnimation.name, true);
            if (onFinish != null)
                SkeletonGraphic.AnimationState.Complete += AnimationState_PostTransition_Complete;
        }
        else
        {
            onFinish?.Invoke();
        }
    }

    private void AnimationState_PostTransition_Complete(Spine.TrackEntry trackEntry)
    {
        SkeletonGraphic.AnimationState.Complete -= AnimationState_PostTransition_Complete;
        onFinish?.Invoke();
    }

    private void AnimationState_Normal_Complete(Spine.TrackEntry trackEntry)
    {
        SkeletonGraphic.AnimationState.Complete -= AnimationState_Normal_Complete;
        onFinish?.Invoke();
    }

    #endregion
    
    
    [ContextMenu("Find Components")]
    private void FindComponents()
    {
        if(SkeletonGraphic==null)
            SkeletonGraphic = GetComponent<SkeletonGraphic>();
        if(RectTransform==null)
            RectTransform = GetComponent<RectTransform>();
        isSkeletonGraphicNull = SkeletonGraphic==null; 
    }

    private void Reset()
    {
        FindComponents();
    }
    
}