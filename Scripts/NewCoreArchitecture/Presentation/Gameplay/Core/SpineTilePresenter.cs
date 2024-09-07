using KitchenParadise.Presentation;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Core
{
    public abstract class SpineTilePresenter : TilePresenter
    {
        public SkeletonGraphic skeletonGraphic;

        public bool loopIdleAnimation;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> idleAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> popAnimations;

        protected sealed override void InternalSetup()
        {
            OnPreSetup();
            var entry = skeletonGraphic.AnimationState.SetAnimation(0, idleAnimations[tile.CurrentLevel() - 1], loopIdleAnimation);
            // NOTE: Mix duration is set to 0 because of pooling
            entry.MixDuration = 0;
        }

        protected abstract void OnPreSetup();

        protected override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, popAnimations[tile.CurrentLevel()], onCompleted);
        }

    }
}