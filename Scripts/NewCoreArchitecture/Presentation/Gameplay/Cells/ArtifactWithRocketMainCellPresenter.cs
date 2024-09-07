using KitchenParadise.Presentation;
using Spine.Unity;
using System;
using System.Collections.Generic;

namespace Match3.Presentation.Gameplay.Cells
{
    public class ArtifactWithRocketMainCellPresenter : AbstractArtifactMainCellPresenter
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string idleAnimation;
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string popAnimations;


        protected override void InternalSetup()
        {
            base.InternalSetup();

            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimation, false);
        }

        public override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, popAnimations, onCompleted);
        }
    }
}