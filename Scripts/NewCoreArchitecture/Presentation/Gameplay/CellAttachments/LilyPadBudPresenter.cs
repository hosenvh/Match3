

using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using System;
using System.Collections.Generic;

namespace Match3.Presentation.Gameplay.CellAttachments
{
    public class LilyPadBudPresenter : CellAttachmentPresenter
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> idleAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> growthAnimations;

        [SpineAnimation(dataField: "skeletonDataAsset")]
        public List<string> shrinkAnimations;

        protected override void InternalSetup(CellAttachment attachment)
        {
            var bud = attachment.As<LilyPadBud>();

            bud.AddComponent(this);
            bud.onGrown += PlayGrowthAnimation;
            bud.onShrunk += PlayShrinkingAnimation;

            skeletonGraphic.AnimationState.SetAnimation(0, idleAnimations[bud.GrowthLevel()], false);
        }

        private void PlayShrinkingAnimation(int currentLevel)
        {
            skeletonGraphic.AnimationState.ClearTrack(0);
            skeletonGraphic.AnimationState.SetAnimation(0, shrinkAnimations[currentLevel], false);
        }

        private void PlayGrowthAnimation(int currentLevel)
        {
            skeletonGraphic.AnimationState.ClearTrack(0);
            skeletonGraphic.AnimationState.SetAnimation(0, growthAnimations[currentLevel], false);
        }
    }
}