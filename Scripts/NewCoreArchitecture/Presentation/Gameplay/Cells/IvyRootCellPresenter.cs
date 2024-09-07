using KitchenParadise.Presentation;
using Spine.Unity;
using System;

namespace Match3.Presentation.Gameplay.Cells
{
    public class IvyRootCellPresenter : Core.CellPresenter
    {
        public SkeletonGraphic skeletonGraphic;
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string idleAnimation;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string popAnimation;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string growthAnimation;

        protected sealed override void InternalSetup()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimation, false);
        }


        public override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, popAnimation, onCompleted);
        }

        public void PlayGrowthEffect()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, growthAnimation, false);
        }
    }
}