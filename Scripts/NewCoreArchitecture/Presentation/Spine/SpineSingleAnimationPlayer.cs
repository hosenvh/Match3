using Spine;
using System;

namespace KitchenParadise.Presentation
{
    public struct SpineSingleAnimationPlayer
    {
        Action onCompletedCallback;
        AnimationState animationState;
        
        // NOTE: We used AnimationState instead of TrackEntry for Complete action, because we want previous Complete actions to be called too.
        public TrackEntry Play(AnimationState animationState,
            int trackIndex,
            string animationName,
            Action onCompleted)
        {

            this.animationState = animationState;
            this.onCompletedCallback = onCompleted;
            var trackEntry = animationState.SetAnimation(trackIndex, animationName, false);
            animationState.Complete += InternalOnCompleted;

            return trackEntry;
        }

        // WARNING: This is not tested.
        public TrackEntry PlayEnqueue(AnimationState animationState,
            int trackIndex,
            string animationName,
            Action onCompleted)
        {
            this.onCompletedCallback = onCompleted;
            var trackEntry =  animationState.AddAnimation(trackIndex, animationName, false, 0.1f);
            trackEntry.Complete += InternalEnqueuedOnCompleted;

            return trackEntry;
        }

        void InternalOnCompleted(TrackEntry trackEntry)
        {
            animationState.Complete -= InternalOnCompleted;
            onCompletedCallback();
            onCompletedCallback = delegate{ };
        }

        void InternalEnqueuedOnCompleted(TrackEntry trackEntry)
        {
            trackEntry.Complete -= InternalEnqueuedOnCompleted;
            onCompletedCallback();
            onCompletedCallback = delegate { };
        }
    }
}