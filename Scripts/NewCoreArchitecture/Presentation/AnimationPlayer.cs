using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Presentation
{
    // TODO: Refactor the interface.
    [RequireComponent(typeof(Animation))]
    public class AnimationPlayer : MonoBehaviour
    {
        Animation animationComp;

        Action onCompletedAction = null;

        private void Awake()
        {
            animationComp = GetComponent<Animation>();
        }

        public void Play(string animationName)
        {
            animationComp.Stop();
            animationComp.Play(animationName);
        }

        public void Play(string animationName, Action onCompleted)
        {
            animationComp.Stop();
            AttachOnCompletedEvent(animationComp.GetClip(animationName), onCompleted);
            animationComp.Play(animationName);
        }

        public void Play()
        {
            animationComp.Stop();
            animationComp.Play();
        }

        public void Play(AnimationClip animationClip, Action onCompleted)
        {
            animationComp.Stop();
            animationComp.AddClip(animationClip, animationClip.name);
            AttachOnCompletedEvent(animationClip, onCompleted);
            animationComp.Play(animationClip.name);
        }

        public void Play(AnimationClip animationClip)
        {
            animationComp.Stop();
            animationComp.AddClip(animationClip, animationClip.name);
            animationComp.Play(animationClip.name);
        }


        public void Play(AnimationClip animationClip, string animationName)
        {
            animationComp.Stop();
            animationComp.AddClip(animationClip, animationName);
            animationComp.Play(animationName);
        }

        public void RemoveClip(string animationName)
        {
            animationComp.RemoveClip(animationName);
        }

        public void Stop()
        {
            animationComp.Stop();
        }

        void AttachOnCompletedEvent(AnimationClip clip, Action onCompleted)
        {
            if (onCompletedAction != null)
                Debug.LogError($"Don't call play during another play.");
            onCompletedAction = onCompleted;
            var onCompletedEvent = new AnimationEvent()
            { functionName = nameof(OnAnimationCompleted), time = clip.length };

            if(Array.Exists(clip.events, e => e.functionName == onCompletedEvent.functionName) == false)
                clip.AddEvent(onCompletedEvent);
        }

        void OnAnimationCompleted()
        {
            onCompletedAction();
            onCompletedAction = null;
        }
    }
}