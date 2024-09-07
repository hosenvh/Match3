using KitchenParadise.Presentation;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Presentation.Gameplay.CellAttachments
{
    // NOTE: This is temporary. It will changed in next tasks.
    public class RopePresenter : CellAttachmentPresenter
    {
        [System.Serializable]
        public struct AnimationEntry
        {
            public GridPlacement placement;
            [SpineAnimation(dataField: "skeletonDataAsset")]
            public List<string> idleAnimations;
            [SpineAnimation(dataField: "skeletonDataAsset")]
            public List<string> hitAnimations;
        }

        public SkeletonGraphic skeletonGraphic;

        public List<AnimationEntry> animationEntries;

        Rope rope;

        protected override void InternalSetup(CellAttachment attachment)
        {
            rope = attachment as Rope;
            UpdateCurrentIdleAnimation();
        }


        private void UpdateCurrentIdleAnimation()
        {
            var animationEntry = FindAnimationEntryFor(rope.placement);

            skeletonGraphic.AnimationState.SetAnimation(0, animationEntry.idleAnimations[rope.CurrentLevel() - 1], false);
        }

        public void PlayHitEffect(Action onCompleted)
        {
            var animationEntry = FindAnimationEntryFor(rope.placement);
            skeletonGraphic.AnimationState.SetAnimation(0, animationEntry.hitAnimations[rope.CurrentLevel()], onCompleted);
        }


        private AnimationEntry FindAnimationEntryFor(GridPlacement placement)
        {
            foreach (var entry in animationEntries)
                if (entry.placement == placement)
                    return entry;

            return default(AnimationEntry);
        }
    }
}