using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapItem_Element_Animation_3D : MapItem_Element
{
    bool isShowing = false;
    int currentStateIndex;
    Animation myAnimation;

    private void Awake()
    {
        if (myAnimation == null)
            myAnimation = GetComponent<Animation>();
        if (!isShowing) // todo : improve this
            gameObject.SetActive(false);
    }

    public override void ShowElementState(int index, bool withAnimation, Action onFinish)
    {
        bool onFinishHandled = false;
        AnimationState animationState;
        if (index > 0)
        {
            isShowing = true;
            gameObject.SetActive(true);
            if (myAnimation == null)
                myAnimation = GetComponent<Animation>();
            if (withAnimation)
            {
                string animationName = currentStateIndex.ToString() + "_" + index.ToString();
                animationState = GetAnimationState(animationName);

                if (animationState)
                {
                    onFinishHandled = true;
                    PlayAnimation(animationState, onFinish);
                }
                else
                {
                    animationState = GetAnimationState(index.ToString());
                    if (animationState)
                    {
                        onFinishHandled = true;
                        PlayAnimation(animationState, onFinish);
                    }
                }
            }
            else
            {
                string animationName = index.ToString();
                animationState = GetAnimationState(animationName);
                if (animationState)
                {
                    onFinishHandled = true;
                    PlayAnimation(animationState, onFinish);
                }
            }
        }
        else
        {
            if (withAnimation)
            {
                string animationName = currentStateIndex.ToString() + "_0";
                animationState = GetAnimationState(animationName);
                if (animationState)
                {
                    onFinishHandled = true;
                    PlayAnimation(animationState, onFinish);
                }
                else
                    gameObject.SetActive(false);
            }
            else
                gameObject.SetActive(false);
        }

        currentStateIndex = index;
        if (!onFinishHandled && onFinish != null)
            onFinish();
    }

    AnimationState GetAnimationState(string animationName)
    {
        foreach (AnimationState animationState in myAnimation)
            if (animationName == animationState.name)
                return animationState;
        return null;
    }

    void PlayAnimation(AnimationState animationState, Action onFinish)
    {
        myAnimation.Play(animationState.name);
        DelayCall(animationState.clip.length, () =>
        {
            if (onFinish != null)
                onFinish();
        });
    }
}