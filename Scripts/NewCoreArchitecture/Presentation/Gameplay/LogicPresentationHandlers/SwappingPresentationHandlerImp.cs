using DG.Tweening;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Swapping;
using Match3.Presentation.Gameplay.Core;
using System;
using Match3.Game.Gameplay;
using Match3.Utility.GolmoradLogging;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{

    public class SwappingPresentationHandlerImp : MonoBehaviour, SwappingPresentationHandler
    {
        public float defaultMovementDuration;
        public AnimationCurve defaultMovementAnimationCurve;

        GameplaySoundManager soundManager;


        void Awake()
        {
            soundManager = ServiceLocator.Find<GameplaySoundManager>();
        }

        public void HandleSwap(CellStack origin, CellStack destination, Action onComplete)
        {
            HandleSwap(origin, destination, defaultMovementDuration, defaultMovementAnimationCurve, onComplete);
        }

        // NOTE: This whole thing about checking depletion is just a temporary and indirect fix.
        // If a tilestack is being destroyed its DoTween animation may not complete, Depletion is an indirect way for checking that.
        // TODO: Find a fix for the about note.
        public void HandleSwap(CellStack origin, CellStack destination, float duration, AnimationCurve animationCurve, Action onComplete)
        {
            var originCellStackPresenter = origin.GetComponent<CellStackPresenter>();
            var destinationCellStackPresenter = destination.GetComponent<CellStackPresenter>();

            bool completedActionIsAssigned = false;

            if (origin.HasTileStack())
            {
                var tileStackPresenter = origin.CurrentTileStack().GetComponent<TileStackPresenter>();
                tileStackPresenter.SetLogicPositionUpdateFlag(false);
                var tween = tileStackPresenter.transform.
                    DOMove(destinationCellStackPresenter.transform.position, duration).
                    SetEase(animationCurve);

                if (origin.CurrentTileStack().IsDepleted() == false)
                {
                    tween.OnComplete(() => { tileStackPresenter.SetLogicPositionUpdateFlag(true); onComplete.Invoke(); });
                    completedActionIsAssigned = true;
                }
                else
                    tween.OnComplete(() => { tileStackPresenter.SetLogicPositionUpdateFlag(true); });
            }

            if (destination.HasTileStack())
            {
                var tileStackPresenter = destination.CurrentTileStack().GetComponent<TileStackPresenter>();
                tileStackPresenter.SetLogicPositionUpdateFlag(false);
                var tween = tileStackPresenter.transform.
                    DOMove(originCellStackPresenter.transform.position, duration).
                    SetEase(animationCurve);

                if (completedActionIsAssigned)
                    tween.OnComplete(() => { tileStackPresenter.SetLogicPositionUpdateFlag(true); });
                else if (destination.CurrentTileStack().IsDepleted() == false)
                {
                    tween.OnComplete(() => { tileStackPresenter.SetLogicPositionUpdateFlag(true); onComplete.Invoke(); });
                    completedActionIsAssigned = true;
                }
            }

            if (completedActionIsAssigned == false)
            {
                DebugPro.LogError<CoreGameplayLogTag>("Could not apply swapping correctly.");
                onComplete.Invoke();
            }

            soundManager.PlaySwapSoundEffect();
        }
    }
}