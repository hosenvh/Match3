using System;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.LogicPresentationHandlers;
using Spine.Unity;

namespace Match3.Presentation.Gameplay.Tiles
{
    [Serializable]
    public struct Range
    {
        public float min;
        public float max;
    }

    public class ButterflyTilePresenter : TilePresenter
    {
        public Range idleStartDelayRange;

        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string idleAnimation;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string flyAnimation;

        protected override void InternalSetup()
        {
            PlayIdleEffect();
        }

        public void PlayFlyEffect()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, flyAnimation, true);
        }

        public void PlayIdleEffect()
        {
            skeletonGraphic.AnimationState.SetEmptyAnimation(0, 0);

            this.Wait(
                UnityEngine.Random.Range(idleStartDelayRange.min, idleStartDelayRange.max),
                StartIdleAnimation);
        }

        void StartIdleAnimation()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimation, true);
        }
    }
}