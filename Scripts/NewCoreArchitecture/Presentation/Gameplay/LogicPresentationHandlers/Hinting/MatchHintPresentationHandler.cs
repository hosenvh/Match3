using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using Match3.Presentation.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers.Hinting
{
    public class MatchHintPresentationHandler : HintPresentationHanlder<MatchHint>
    {
        TileStackPresenter targetTileStackPresenter;
        TilePresenter targetTilePresenter;

        public MatchHintPresentationHandler(MatchHint hint, HintingPresentationPort hintingPort) : base(hint, hintingPort)
        {
        }

        public override void Show()
        {
            if(hint.origin.CurrentTileStack() != null)
                targetTileStackPresenter = hint.origin.CurrentTileStack().GetComponent<TileStackPresenter>();
            if (hint.origin.CurrentTileStack().Top() != null)
                targetTilePresenter = hint.origin.CurrentTileStack().Top().GetComponent<TilePresenter>();
            if(targetTileStackPresenter)
                PlayHintAnimation(targetTileStackPresenter, hintingPort.FindMoveAnimationClip(hint.origin, hint.destination));
            TryPlayGlowEffect(targetTilePresenter);
            SetActiveGlowHintForCellStack(hint.otherMatchedCellStacks, true);
        }

        public override void Stop()
        {
            PlayIdleAnimation(targetTileStackPresenter);
            TryStopGlowEffect(targetTilePresenter);
            SetActiveGlowHintForCellStack(hint.otherMatchedCellStacks, false);

            targetTilePresenter = null;
            targetTileStackPresenter = null;
        }

        private void SetActiveGlowHintForCellStack(List<CellStack> cellStacks, bool active)
        {
            foreach (var cellStack in cellStacks)
            {
                var tileStack = cellStack.CurrentTileStack().Stack();
                foreach (var tile in tileStack)
                {
                    var glowEffectController = tile.GetComponent<TilePresenter>().glowEffectController;
                    if (glowEffectController == null) continue;

                    if (active)
                        glowEffectController.SetGlow();
                    else
                        glowEffectController.SetNormal();
                }
            }
        }

    }
}