using KitchenParadise.Presentation;
using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using PandasCanPlay.HexaWord.Utility;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class CatTilePresenter : TilePresenter
    {
        [System.Serializable]
        private class DirectionalAnimationData
        {
            public Direction direction;
            [SpineAnimation(dataField: nameof(skeletonGraphic))]
            public string idleAnimationName;
            [SpineAnimation(dataField: nameof(skeletonGraphic))]
            public string moveAnimationName;
            [SpineAnimation(dataField: nameof(skeletonGraphic))]
            public string attackAnimation;
            [SpineEvent(dataField: nameof(skeletonGraphic))]
            public string hitEvent;
        }

        [SerializeField] private SkeletonGraphic skeletonGraphic;

        [ArrayElementTitle(nameof(DirectionalAnimationData.direction))]
        [SerializeField] private DirectionalAnimationData[] animationsData;

        protected override void InternalSetup()
        {
        }

        public void PlayMoveAnimation(Direction direction)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, AnimationDataFor(direction).moveAnimationName, true);
        }

        public void PlayIdle(Direction direction)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, AnimationDataFor(direction).idleAnimationName, true);
        }

        public void PlayAttackAnimation(Direction direction, Action onHit, Action onCompleted)
        {
            var animationData = AnimationDataFor(direction);
            var trackEntry = skeletonGraphic.AnimationState.SetAnimation(0, animationData.attackAnimation, onCompleted);
            trackEntry.Event += (entry, evt) => { if (evt.data.name.Equals(animationData.hitEvent)) onHit.Invoke(); };
        }

        DirectionalAnimationData AnimationDataFor(Direction direction)
        {
            return animationsData.Find((d) => d.direction == direction);
        }
    }
}