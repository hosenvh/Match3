using Match3.Game.Gameplay.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Match3.Presentation.Gameplay.Core;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class BroomPowerUpEffect : MonoBehaviour
    {

        public float movementSpeed;
        public float movementDelay;
        public UnityEvent onStartedHorizeontalMovement;
        public UnityEvent onStartedVerticalMovement;
        public UnityEvent onFinishedEffect;

        Action<CellStack> onPassedCellStack;
        Action onFinishedCallback;
        Queue<CellStack> horizontalQueue;
        Queue<CellStack> verticalQueue;


        public void Play(List<CellStack> horizontals, List<CellStack> verticals, Action<CellStack> onPassedCellStack, Action onFinished)
        {
            this.onPassedCellStack = onPassedCellStack;
            this.onFinishedCallback = onFinished;

            horizontalQueue = new Queue<CellStack>(horizontals);
            verticalQueue = new Queue<CellStack>(verticals);

            StartHorizontal();

        }

        private void StartHorizontal()
        {

            var pos = horizontalQueue.Peek().GetComponent<CellStackPresenter>().transform.position;
            pos.x -= BoardPresenterNew.CellSize;
            this.transform.position = pos;
            onStartedHorizeontalMovement.Invoke();
            MoveOverTo(HorizontalPosFor(horizontalQueue.Peek()), movementDelay, OnHoriontallyPassed);
        }

        private Vector3 HorizontalPosFor(CellStack cellStack)
        {
            var pos = cellStack.GetComponent<CellStackPresenter>().transform.position;
            //pos.x += BoardPresenterNew.CellSize/2;
            return pos;
        }

        private void MoveOverTo(Vector3 position, float delay, Action OnCompleted)
        {

            this.transform.
                DOMove(position, movementSpeed).
                SetSpeedBased(true).
                SetEase(Ease.Linear).
                SetDelay(delay).
                OnComplete(() => OnCompleted());
        }

        private void OnHoriontallyPassed()
        {
            onPassedCellStack(horizontalQueue.Dequeue());

            if (horizontalQueue.Count > 0)
                MoveOverTo(HorizontalPosFor(horizontalQueue.Peek()), 0, OnHoriontallyPassed);
            else
                StartVertical();

        }

        private void StartVertical()
        {
            var pos = verticalQueue.Peek().GetComponent<CellStackPresenter>().transform.position;
            pos.y += BoardPresenterNew.CellSize;
            this.transform.position = pos;
            onStartedVerticalMovement.Invoke();
            MoveOverTo(VerticalPosFor(verticalQueue.Peek()), movementDelay, OnVerticallPassed);
        }

        private Vector3 VerticalPosFor(CellStack cellStack)
        {
            var pos = cellStack.GetComponent<CellStackPresenter>().transform.position;
            //pos.y -= BoardPresenterNew.CellSize;
            return pos;
        }

        private void OnVerticallPassed()
        {
            onPassedCellStack(verticalQueue.Dequeue());

            if (verticalQueue.Count > 0)
                MoveOverTo(VerticalPosFor(verticalQueue.Peek()), 0, OnVerticallPassed);
            else
                Finish();

        }

        private void Finish()
        {
            onFinishedCallback();
            onFinishedEffect.Invoke();
            Destroy(this.gameObject);
        }

        private void StartSweeping(List<CellStack> horizontals, Action onEnded )
        {
            this.transform.position = horizontals[0].GetComponent<CellStackPresenter>().transform.position;
        }
    }
}