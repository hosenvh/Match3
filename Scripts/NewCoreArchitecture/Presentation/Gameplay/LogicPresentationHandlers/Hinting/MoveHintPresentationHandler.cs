using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using Match3.Presentation.Gameplay.Core;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers.Hinting
{
    public class MoveHintPresentationHandler : HintPresentationHanlder<MoveHint>
    {
        TileStackPresenter targetTileStackPresenter;
        TilePresenter targetTilePresenter;

        public MoveHintPresentationHandler(MoveHint hint, HintingPresentationPort hintingPort) : base(hint, hintingPort)
        {
        }

        public override void Show()
        {
            targetTileStackPresenter = hint.origin.CurrentTileStack().GetComponent<TileStackPresenter>();
            targetTilePresenter = hint.origin.CurrentTileStack().Top().GetComponent<TilePresenter>();

            PlayHintAnimation(targetTileStackPresenter, hintingPort.FindMoveAnimationClip(hint.origin, hint.destination));
            TryPlayGlowEffect(targetTilePresenter);
        }

        public override void Stop()
        {
            PlayIdleAnimation(targetTileStackPresenter);
            TryStopGlowEffect(targetTilePresenter);

            targetTilePresenter = null;
            targetTileStackPresenter = null;
        }
    }
}