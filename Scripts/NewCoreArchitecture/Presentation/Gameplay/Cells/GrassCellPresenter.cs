using KitchenParadise.Presentation;
using Match3.Game.Gameplay.Cells;
using Spine.Unity;
using System;
using System.Collections.Generic;

namespace Match3.Presentation.Gameplay.Cells
{
    public class GrassCellPresenter : Core.CellPresenter
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> idleAnimations;
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> popAnimations;


        private GrassCell grassCell;

        protected sealed override void InternalSetup()
        {
            grassCell = cell as GrassCell;
            grassCell.onLevelIncreased += SetCurrentIdleAnimation;
            SetCurrentIdleAnimation();
        }

        private void OnDestroy()
        {
            grassCell.onLevelIncreased -= SetCurrentIdleAnimation;
        }

        private void SetCurrentIdleAnimation()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimations[grassCell.CurrentLevel() - 1], false);
        }

        public override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, popAnimations[cell.As<GrassCell>().CurrentLevel()], onCompleted);
        }
    }
}