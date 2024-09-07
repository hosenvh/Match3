
using System;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class HoneyTilePresenter : TilePresenter
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: nameof(skeletonGraphic))]
        public List<string> idleAnimations;


        [SpineAnimation(dataField: nameof(skeletonGraphic))]
        public string popAnimation;



        protected override void InternalSetup()
        {

            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimations[UnityEngine.Random.Range(0, idleAnimations.Count)], false);
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, popAnimation, onCompleted);
        }
    }
}