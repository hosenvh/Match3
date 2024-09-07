using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using Match3.Presentation.Gameplay.Core;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers.Hinting
{
    public class ActivationHintPresentationHandler : HintPresentationHanlder<ActivationHint>
    {
        TileStackPresenter targetTileStackPresenter;
        TilePresenter targetTilePresenter;

        public ActivationHintPresentationHandler(ActivationHint hint, HintingPresentationPort hintingPort) : base(hint, hintingPort)
        {
        }

        public override void Show()
        {
            targetTileStackPresenter = hint.target.CurrentTileStack().GetComponent<TileStackPresenter>();
            targetTileStackPresenter.animationPlayer.Play(hintingPort.activationAnimationClip, "Hinting");

            targetTilePresenter = hint.target.CurrentTileStack().Top().GetComponent<TilePresenter>();

            PlayHintAnimation(targetTileStackPresenter, hintingPort.activationAnimationClip);
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