using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Cells
{
    public class HedgeCellPresenter : Core.CellPresenter
    {
        // NOTE: Hedge Cell must be rendered on top of the tiles. For now it is handled by Sorting Order. 
        // It must be refactored when a better solution is implemented for ordering of Game Board.
        public Canvas canvas;
        public int sortingOrder;

        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> idleAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> popAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> bounceAnimations;


        TileStack lastBouncedTileStack = null;
        HedgeCell hedgeCell;

        protected sealed override void InternalSetup()
        {
            this.canvas.sortingOrder = sortingOrder;
            hedgeCell = cell as HedgeCell;
            SetCurrentIdleAnimation();
        }

        private void SetCurrentIdleAnimation()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimations[hedgeCell.CurrentLevel() - 1], false);
        }

        public override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, popAnimations[hedgeCell.CurrentLevel()], onCompleted);
        }

        public void TryPlayBounceFor(TileStack tileStack)
        {
            if (lastBouncedTileStack != tileStack)
            {
                lastBouncedTileStack = tileStack;
                PlayBounceAnimation();
            }
        }

        private void PlayBounceAnimation()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, bounceAnimations[hedgeCell.CurrentLevel()-1], false);
        }
    }
}