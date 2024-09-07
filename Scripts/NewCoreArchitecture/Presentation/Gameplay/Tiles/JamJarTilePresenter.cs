
using System;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class JamJarTilePresenter : TilePresenter
    {
        [Serializable]
        public struct AnimationEntry
        {
            public TileColor tileColor;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string fillAnimation;

            [SpineAnimation(dataField: "skeletonDataAsset")]
            public string popAnimation;
        }

        public SkeletonGraphic skeletonGraphic;
        public List<AnimationEntry> animationEntries;


        protected override void InternalSetup()
        {

        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            // NOTE: This relies on the internal implementation of JamJar.
            // TODO: Try to remove this reliance.
            switch(tile.CurrentLevel())
            {
                case 0:
                    skeletonGraphic.AnimationState.SetAnimation(0, FindCurrentAnimationEntry().popAnimation, onCompleted);
                    break;
                case 1:
                    skeletonGraphic.AnimationState.SetAnimation(0, FindCurrentAnimationEntry().fillAnimation, onCompleted);
                    break;
            }
        }

        private AnimationEntry FindCurrentAnimationEntry()
        {
            return animationEntries.Find(e => e.tileColor == tile.As<JamJar>().FilledColor());
        }

    }
}