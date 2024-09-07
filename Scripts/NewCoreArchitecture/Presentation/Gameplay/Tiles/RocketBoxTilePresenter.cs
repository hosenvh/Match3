using System;
using System.Collections.Generic;
using KitchenParadise.Presentation;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class RocketBoxTilePresenter : TilePresenter
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string idleAnimation;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public string activationAnimation;

        public List<SeekingRocketPresenter> seekingRocketPresenters;

        public AudioClip activationAudioClip;

        protected override void InternalSetup()
        {
            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimation, true);
        }

        public void PlayActivationEffect(Action onCompleted)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, activationAnimation, onCompleted);
            ServiceLocator.Find<GameplaySoundManager>().TryPlay(activationAudioClip);
        }
    }
}