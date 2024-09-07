
using KitchenParadise.Presentation;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using System;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class RainbowTilePresenter : TilePresenter
    {
        public UnityEvent onActivated;
        public UnityEvent onDestoryed;

        public SkeletonGraphic skeletonGraphic;



        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string destrcutionAnimation;
        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string activationAnimation;
        protected override void InternalSetup()
        {
            
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, destrcutionAnimation, onCompleted);
            onDestoryed.Invoke();
        }

        public void PlayActivation()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, activationAnimation, false);
            onActivated.Invoke();
        }
    }
}