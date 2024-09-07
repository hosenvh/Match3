using System;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class IvyBushTilePresenter : SpineTilePresenter
    {
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string growthAnimation;

        protected override void OnPreSetup()
        {
            
        }

        public void PlayGrowthEffect()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, growthAnimation, false);
        }
    }
}