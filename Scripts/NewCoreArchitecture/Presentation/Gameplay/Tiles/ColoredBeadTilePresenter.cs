using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class ColoredBeadTilePresenter : TilePresenter
    {
        [System.Serializable]
        public struct DirtinessAnimationInfo
        {
            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string cleanAnimation;
            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string dirtyAnimation;
        }

        public SkeletonGraphic skeletonGraphic;

        public DirtinessAnimationInfo idleAnimationInfo;
        public DirtinessAnimationInfo popAnimationInfo;
        public DirtinessAnimationInfo changingAnimationInfo;


        ColoredBead coloredBead;

        protected override void InternalSetup()
        {
            coloredBead = tile.As<ColoredBead>();
            coloredBead.onDirtinessChanged += PlayDirtinessChangingAnimation;

            SetupIdleAnimation(coloredBead.GetDirtinessState());
        }

        private void SetupIdleAnimation(ColoredBead.DirtinessState dirtinessState)
        {
            var entry = skeletonGraphic.AnimationState.SetAnimation(0, GetAnimation(idleAnimationInfo, dirtinessState), false);
            // NOTE: Mix duration is set to 0 because of pooling
            entry.MixDuration = 0;
        }

        private void PlayDirtinessChangingAnimation(ColoredBead.DirtinessState dirtinessState)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, GetAnimation(changingAnimationInfo, dirtinessState), false);
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            // NOTE: This ignores the situation when a dirty colored bead is hit.
            // TODO: Find a way to remove this check.
            if (coloredBead.CurrentLevel() >= 1)
                return;

            skeletonGraphic.AnimationState.SetAnimation(0, GetAnimation(popAnimationInfo, coloredBead.GetDirtinessState()), onCompleted);
        }

        private string GetAnimation(DirtinessAnimationInfo animationInfo, ColoredBead.DirtinessState dirtinessState)
        {
            switch (dirtinessState)
            {
                case ColoredBead.DirtinessState.Clean:
                    return animationInfo.cleanAnimation;
                case ColoredBead.DirtinessState.Dirty:
                    return animationInfo.dirtyAnimation;
                default:
                    return "";
            }
        }

    }
}