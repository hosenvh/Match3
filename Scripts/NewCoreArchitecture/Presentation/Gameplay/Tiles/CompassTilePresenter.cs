using System.Collections.Generic;
using DG.Tweening;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using UnityEngine;


namespace Match3.Presentation.Gameplay.Tiles
{
    public class CompassTilePresenter:SpineTilePresenter
    {
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> upAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> downAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> rightAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> leftAnimations;

        public void SetRotationCompass(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    SetRotation(upAnimations);
                    break;
                case Direction.Right:
                    SetRotation(rightAnimations);
                    break;
                case Direction.Down:
                    SetRotation(downAnimations);
                    break;
                case Direction.Left:
                    SetRotation(leftAnimations);
                    break;
            }
        }
        private void SetRotation(List<string> animations)
        {
             skeletonGraphic.AnimationState.SetAnimation(0, animations[tile.CurrentLevel() - 1], loopIdleAnimation);
        }
        protected override void OnPreSetup()
        {

        }
    }
}