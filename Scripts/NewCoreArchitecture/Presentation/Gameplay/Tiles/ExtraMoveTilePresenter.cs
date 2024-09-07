using System;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using SeganX;
using Spine.Unity;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class ExtraMoveTilePresenter : TilePresenter
    {
        public SkeletonGraphic skeletonGraphic;

        public bool loopIdleAnimation;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> idleAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> popAnimations;

        protected sealed override void InternalSetup()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimations[tile.As<ExtraMove>().moveAmount- 1], false);
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, popAnimations[tile.As<ExtraMove>().moveAmount - 1], onCompleted);
        }
    }
}