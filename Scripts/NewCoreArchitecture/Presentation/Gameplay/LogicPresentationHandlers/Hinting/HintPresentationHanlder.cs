using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers.Hinting
{
    public interface HintPresentationHanlder
    {
        void Show();
        void Stop();
    }

    public abstract class HintPresentationHanlder<T> : HintPresentationHanlder where T : Hint
    {
        protected T hint;
        protected HintingPresentationPort hintingPort;

        protected HintPresentationHanlder(T hint, HintingPresentationPort hintingPort)
        {
            this.hint = hint;
            this.hintingPort = hintingPort;
        }

        public abstract void Show();
        public abstract void Stop();

        protected void PlayIdleAnimation(TileStackPresenter tileStackPresenter)
        {
            if (tileStackPresenter == null)
                return;

            tileStackPresenter.animationPlayer.Stop();
            tileStackPresenter.animationPlayer.RemoveClip("Hinting");
            tileStackPresenter.animationPlayer.Play(hintingPort.idlleAnimationClip, "Idle");
        }

        protected void PlayHintAnimation(TileStackPresenter tileStackPresenter, AnimationClip animationClip)
        {
            if (tileStackPresenter == null)
                return;

            tileStackPresenter.animationPlayer.Play(animationClip, "Hinting");
        }

        protected void TryPlayGlowEffect(TilePresenter tilePresenter)
        {
            if (tilePresenter == null)
                return;

            if (tilePresenter.glowEffectController != null)
                tilePresenter.glowEffectController.SetGlow();

        }

        protected void TryStopGlowEffect(TilePresenter tilePresenter)
        {
            if (tilePresenter == null)
                return;

            if (tilePresenter.glowEffectController != null)
                tilePresenter.glowEffectController.SetNormal();

        }
    }
}