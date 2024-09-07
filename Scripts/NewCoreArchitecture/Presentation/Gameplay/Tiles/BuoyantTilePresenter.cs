
using System;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class BuoyantTilePresenter : TilePresenter
    {
        [Serializable]
        public struct AnimationEntry
        {
            public TileColor tileColor;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string idleAnimation;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string popAnimation;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string appearingAnimation;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string disappearingAnimation;
        }

        public SkeletonGraphic skeletonGraphic;
        public List<AnimationEntry> animationEntries;

        AnimationEntry currentAnimationEntry;

        protected override void InternalSetup()
        {
            currentAnimationEntry = animationEntries.Find(c => c.tileColor == tile.As<Buoyant>().Color());
            skeletonGraphic.AnimationState.SetAnimation(0, currentAnimationEntry.idleAnimation, false);
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, currentAnimationEntry.popAnimation, onCompleted);
        }

        public void PlayAppearAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, currentAnimationEntry.appearingAnimation, onCompleted);
        }

        public void PlayDisapperAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, currentAnimationEntry.disappearingAnimation, onCompleted);
        }
    }
}