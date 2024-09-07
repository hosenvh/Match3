using Match3.Game.Gameplay.SubSystems.HintingAndShuffling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Match3.Presentation.Gameplay.Core;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class BoardShufflingPresentationHandlerImp : MonoBehaviour, BoardShufflingPresentationHandler
    {
        public GeneralGameplayFeedBack noMovesFeedback;
        public GeneralGameplayFeedBack shufflingFeedback;

        public float movementSpeed;
        public Transform targetTransform;


        public void Shuffle(List<BoardShufflingSystem.ShuffleData> shuffles, Action onCompleted)
        {
            ShowNoMoveFeedkBack(() => ShowSufflingFeedback(() => StartShuffling(shuffles, onCompleted)));
        }

        private void ShowSufflingFeedback(Action onFinished)
        {
            var feedback = Instantiate(shufflingFeedback, targetTransform, false);
            feedback.transform.position = Vector3.zero;
            feedback.Play(onFinished);
        }

        private void ShowNoMoveFeedkBack(Action onFinished)
        {
            var feedback = Instantiate(noMovesFeedback, targetTransform, false);
            feedback.transform.position = Vector3.zero;
            feedback.Play(onFinished);
        }

        private void StartShuffling(List<BoardShufflingSystem.ShuffleData> shuffles, Action onCompleted)
        {
            //ShowSufflingFeedback(delegate { });
            Counter counter = new Counter(shuffles.Count, () => TearDownShuffling(shuffles, onCompleted));
            foreach (var shuffle in shuffles)
            {
                var tileStackPresenter = shuffle.origin.GetComponent<TileStackPresenter>();
                tileStackPresenter.SetLogicPositionUpdateFlag(false);

                var target = shuffle.destination.GetComponent<CellStackPresenter>().transform.position;

                tileStackPresenter.transform.
                    DOMove(target, movementSpeed).
                    SetSpeedBased(true).
                    OnComplete(() => counter.Decrement());
            }
        }

        void TearDownShuffling(List<BoardShufflingSystem.ShuffleData> shuffles, Action onCompleted)
        {
            foreach (var shuffle in shuffles)
                shuffle.origin.GetComponent<TileStackPresenter>().SetLogicPositionUpdateFlag(true);
            

            onCompleted();
        }
    }
}