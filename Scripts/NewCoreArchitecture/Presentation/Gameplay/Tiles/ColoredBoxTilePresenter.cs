using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using System;
using System.Collections.Generic;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class ColoredBoxTilePresenter : TilePresenter
    {
        [Serializable]
        public struct AnimationEntry
        {
            public TileColor tileColor;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public List<string> idleAnimations;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public List<string> popAnimations;
        }

        public SkeletonGraphic skeletonGraphic;
        public List<AnimationEntry> animationEntries;


        AnimationEntry currentAnimationEntry;

        protected override void InternalSetup()
        {
            currentAnimationEntry = animationEntries.Find(c => c.tileColor == tile.As<ColoredBox>().color);
            skeletonGraphic.AnimationState.SetAnimation(0, currentAnimationEntry.idleAnimations[tile.CurrentLevel() - 1], false);
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, currentAnimationEntry.popAnimations[tile.CurrentLevel()], onCompleted);
        }
    }
}