using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.WinSequence;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.GoalManagement;
using Match3.Presentation.Gameplay.LevelConditionManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{

    public class WinSequencePresentationHandlerImp : MonoBehaviour, WinSequencePresentationHandler
    {
        public MovingObject placementFlowPrefab;
        public BoosterCreationEffect placementEffect;
        public float flowMovementSpeed;
        public float flowForceManitude;

        public Transform targetTransform;
        public GameplayStateRoot gameStateRoot;
        public MultiTargetSequencialEffectPlayer itemCreationFlowEffectPlayer;
        public DefaultTilePlacingPresentationHandler tilePlacingPresentationHandler;
        public MovementStopConditionPresenter movementStopConditionPresenter;
        public GoalsPresentationController goalsPresentationController;
        public GainedCoinsPresentationController coinsPresentationController;


        Popup_WinCeleberation winCeleberationPopup;

        public void HandleActivation(Action onReadyForActivation)
        {
            coinsPresentationController.Setup(gameStateRoot.gameplayState.gameplayController);

            var winSequenceSystem = gameStateRoot.gameplayState.gameplayController.GetSystem<LevelWinSequenceSystem>();

            winCeleberationPopup = global::Base.gameManager.OpenPopup<Popup_WinCeleberation>();
            winCeleberationPopup.Setup(
                winSequenceSystem.Skip,
                () => PrepareCoinsPresentation(onReadyForActivation)
                );

        }

        private void PrepareCoinsPresentation(Action onReadyForActivation)
        {
            goalsPresentationController.Disappear(() => coinsPresentationController.Appear(onReadyForActivation));
        }

        public void HandleSkipping()
        {
            tilePlacingPresentationHandler.StopPlacing();
            TryCloseWinCelebrationPopup();
            coinsPresentationController.UpdateCoins();
        }

        public void PlaceExplosives(List<Tile> tiles, List<CellStack> targets, Action<int> onPlaced, Action onCompleted)
        {
            itemCreationFlowEffectPlayer.Play(
                count: tiles.Count,
                playStepEffectAction: (i) => PlayItemFlow(i, targets[i], onPlaced),
                onSequenceCompleted: onCompleted
                );
            //tilePlacingPresentationHandler.PlaceSequenceWithStartDelay(tiles, targets, onPlaced, onCompleted, 0);
        }

        private void PlayItemFlow(int i, CellStack cellStack, Action<int> onPlaced)
        {

            movementStopConditionPresenter.ReduceRemainingMoves(1);
            var flow = Instantiate(placementFlowPrefab, targetTransform, false);
            var startPos = movementStopConditionPresenter.transform.position + new Vector3(0,0,-100);
            var endPos = cellStack.GetComponent<CellStackPresenter>().transform.position + new Vector3(0, 0, -100); ;
            var force = Vector2.up * flowForceManitude;
            if (UnityEngine.Random.value < 0.5)
                force = Vector2.down * flowForceManitude;


            flow.Move(
                startPos,
                endPos,
                force,
                flowMovementSpeed,
                () => PlayPlacementEffect(endPos, () => onPlaced(i)));


        }

        private void PlayPlacementEffect(Vector3 pos, Action onCompleted)
        {
            var effect = Instantiate(placementEffect, targetTransform, false);
            effect.transform.position = pos;

            effect.Play();
            onCompleted();
        }

        public void HandleFinishing()
        {
            TryCloseWinCelebrationPopup();
        }

        private void TryCloseWinCelebrationPopup()
        {
            if (winCeleberationPopup != null)
            {
                winCeleberationPopup.Close();
                winCeleberationPopup = null;
            }
        }
    }
}