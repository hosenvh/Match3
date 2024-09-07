using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using System;
using System.Collections.Generic;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class GardenTilePresenter : TilePresenter
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> idleAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> fillAnimations;

        Garden garden; 

        protected override void InternalSetup()
        {
            garden = tile as Garden;
            garden.onReset += ResetGarden;
            ResetGarden();
        }

        private void ResetGarden()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimations[garden.FillLevel()], false);
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            if (garden.FillLevel() > 3)
                return;

            skeletonGraphic.AnimationState.SetAnimation(0, fillAnimations[garden.FillLevel()-1], onCompleted);
        }
    }
}
