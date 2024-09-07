using KitchenParadise.Presentation;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using PandasCanPlay.HexaWord.Utility;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Match3.Presentation.Gameplay.Tiles
{
    public class BaseSandTilePresenter : TilePresenter
    {
        [System.Serializable]
        public class StuckOutItemAnimationInfo
        {
            [Type(typeof(Tile), true)]
            public string itemType;

            [SpineAnimation(dataField: nameof(stuckOutItemSkeletonGraphic))]
            public string idleAnimation;

            [SpineAnimation(dataField: nameof(stuckOutItemSkeletonGraphic))]
            public string destroyAnimation;

            public bool Matches(Tile tile)
            {
                return tile.GetType() == Type.GetType(itemType);
            }
        }

        public SkeletonGraphic mainSkeletonGraphic;
        public SkeletonGraphic overlaySkeletonGraphic;
        [FormerlySerializedAs("tileBelowSandSkeletonGraphic")]
        public SkeletonGraphic stuckOutItemSkeletonGraphic;

        [SpineAnimation(dataField: nameof(mainSkeletonGraphic))]
        public string mainIdleAnimation;


        [SpineAnimation(dataField: nameof(mainSkeletonGraphic))]
        public string mainPopAnimation;

        [SpineAnimation(dataField: nameof(overlaySkeletonGraphic))]
        public List<string> overlayIdleAnimations;

        [SpineAnimation(dataField: nameof(overlaySkeletonGraphic))]
        public List<string> overlayPopAnimations;

        public List<StuckOutItemAnimationInfo> stuckOutItemAnimationInfos;

        StuckOutItemAnimationInfo currentStuckOutItemAnimationInfo = null;

        protected override void InternalSetup()
        {
            var level = tile.CurrentLevel();

            mainSkeletonGraphic.AnimationState.SetAnimation(0, mainIdleAnimation, false);

            if (level <= 1)
                overlaySkeletonGraphic.gameObject.SetActive(false);
            else
                overlaySkeletonGraphic.AnimationState.SetAnimation(0, overlayIdleAnimations[tile.CurrentLevel() - 2], false);

            if (stuckOutItemAnimationInfos.Count > 0)
                SetupStuckOutItem();
        }

        private void SetupStuckOutItem()
        {
            var bottomTile = QueryUtilities.TileBelowOf(tile);

            if (bottomTile == null)
                return;

            foreach (var info in stuckOutItemAnimationInfos)
                if (info.Matches(bottomTile))
                {
                    currentStuckOutItemAnimationInfo = info;
                    break;
                }
            
            if(currentStuckOutItemAnimationInfo != null)
                stuckOutItemSkeletonGraphic.AnimationState.SetAnimation(0, currentStuckOutItemAnimationInfo.idleAnimation, false);

        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            if (tile.CurrentLevel() == 0)
            {
                mainSkeletonGraphic.AnimationState.SetAnimation(0, mainPopAnimation, onCompleted);
                if(currentStuckOutItemAnimationInfo != null)
                    stuckOutItemSkeletonGraphic.AnimationState.SetAnimation(0, currentStuckOutItemAnimationInfo.destroyAnimation, false);
            }
            else
                overlaySkeletonGraphic.AnimationState.SetAnimation(0, overlayPopAnimations[tile.CurrentLevel() - 1], onCompleted);
        }


    }
}