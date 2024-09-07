using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Game.Gameplay;
using Match3.Utility.GolmoradLogging;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers.Hinting
{
    public class HintingPresentationPort : PresentationController, HintingPresentationHandler
    {
        [System.Serializable]
        public struct DirectionalAimationClipInfo
        {
            public AnimationClip animationClip;
            public Direction direction; 

        }

        public float hintShowingDelay;

        public List<DirectionalAimationClipInfo> directionalAimationClips;
        public AnimationClip activationAnimationClip;
        public AnimationClip idlleAnimationClip;


        CellStackBoard cellStackBoard;
        WaitForSeconds waitForSeconds;

        HintPresentationHanlder currentHintHanlder;

        protected override void InternalSetup(GameplayState gameState)
        {
            currentHintHanlder = null;
            waitForSeconds = new WaitForSeconds(hintShowingDelay);
            cellStackBoard = gameState.gameplayController.GameBoard().CellStackBoard();
        }

        public void Apply(Hint hint)
        {
            Stop();
            StartCoroutine(ShowHintWithDelay(hint)); 
        }

        IEnumerator ShowHintWithDelay(Hint hint)
        {
            yield return waitForSeconds;
            ShowHint(hint);
        }

        void ShowHint(Hint hint)
        {
            try
            {
                currentHintHanlder = CreateHintHandlerFor(hint);
                currentHintHanlder.Show();
            }
            catch (Exception)
            {
                // Todo: We know having Exceptions in 'showing' hints, such as 'emptyStack' or 'nullReference' but we are commenting this log for less filling GA Panel, this should be Checked In Future.
                // Debug.LogError($"Show Hint Error \n {e.Message} \n {e.StackTrace}");
            }
        }

        private HintPresentationHanlder CreateHintHandlerFor(Hint hint)
        {
            switch (hint)
            {
                case MoveHint moveHint:
                    return new MoveHintPresentationHandler(moveHint, this);
                case ActivationHint activationHint:
                    return new ActivationHintPresentationHandler(activationHint, this);
                case MatchHint matchHint:
                    return new MatchHintPresentationHandler(matchHint, this);
                default:
                    return new EmptyHintPresentationHandler();
            }
        }

        public AnimationClip FindMoveAnimationClip(CellStack origin, CellStack destination)
        {
            var direction = cellStackBoard.RelativeDirectionOf(origin.Position(), destination.Position());
            return FindAnimationClipFor(direction);
        }

        private AnimationClip FindAnimationClipFor(Direction direction)
        {
            return directionalAimationClips.Find(info => info.direction == direction).animationClip;
        }

        public void Stop()
        {
            try
            {
                StopAllCoroutines();
                if (currentHintHanlder != null)
                {
                    currentHintHanlder.Stop();
                    currentHintHanlder = null;
                }
            }
            catch (Exception e)
            {
                DebugPro.LogException<CoreGameplayLogTag>($"Stop Hint Error  \n {e.Message} \n {e.StackTrace}");
            }
        }
    }
}