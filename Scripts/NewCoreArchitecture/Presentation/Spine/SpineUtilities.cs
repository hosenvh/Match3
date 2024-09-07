using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KitchenParadise.Presentation
{

    public  static class SpineUtilities 
    {
        public static void SetSkeletonAsset(this SkeletonGraphic skeleton, SkeletonDataAsset asset)
        {
            skeleton.Clear();
            skeleton.skeletonDataAsset = asset;
            skeleton.Initialize(true);
            skeleton.UpdateMesh();
            skeleton.material = asset.atlasAssets[0].materials[0];
            skeleton.Update(0);
        }

        public static TrackEntry SetAnimation(
            this AnimationState animationState, 
            int trackIndex, 
            string animationName, 
            Action onCompleted
            )
        {
            return new SpineSingleAnimationPlayer().Play(animationState, trackIndex, animationName, onCompleted);
        }

        public static void AddAnimation(
            this AnimationState animationState,
            int trackIndex,
            string animationName,
            Action onCompleted
    )
        {
            new SpineSingleAnimationPlayer().PlayEnqueue(animationState, trackIndex, animationName, onCompleted);
        }
    }
}