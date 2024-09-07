using System;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using SeganX;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.LevelConditionManagement
{
    // TODO: Refactor delayed movement changing.
    public class MovementStopConditionPresenter : StopConditionPresenter
    {
        public LocalText remainMoveText;

        public ExtraMoveFlowEffect extraMoveFlowEffect;
        public Transform effectTarget;

        public UnityEvent onRemainingMovesDecreased;

        Animation remainMoveTextAnimation;
        MovementStopCondition movementStopCondition;

        int pendingMoves;

        public override void Setup(StopConditinon stopCondition)
        {
            pendingMoves = 0;
            remainMoveTextAnimation = remainMoveText.GetComponent<Animation>();

            movementStopCondition = stopCondition.As<MovementStopCondition>();

            movementStopCondition.remainingMovesChanged += UpdateRemainingMoves;

            UpdateRemainingMoves(movementStopCondition.RemainingMovements());
        }


        void UpdateRemainingMoves(int remainingMoveAmount)
        {
            remainMoveText.SetText((remainingMoveAmount - pendingMoves).ToString());

            if (remainingMoveAmount < 5)
            {
                if (!remainMoveTextAnimation.IsPlaying("LowMoveCount"))
                    remainMoveTextAnimation.Play("LowMoveCount");
            }
            else
            {
                if (!remainMoveTextAnimation.IsPlaying("Idle"))
                    remainMoveTextAnimation.Play("Idle");
            }
        }

        public void HandleExtraMovesEffect(ExtraMove extraMoveTile)
        {
            pendingMoves += extraMoveTile.moveAmount;
            var effect = Instantiate(extraMoveFlowEffect, effectTarget, false);
            effect.Play(
                extraMoveTile.GetComponent<TilePresenter>().transform.position,
                this.transform.position, 
                () => ReducePendingExtraMoves(extraMoveTile.moveAmount));
        }

        private void ReducePendingExtraMoves(int moveAmount)
        {
            pendingMoves -= moveAmount;
            UpdateRemainingMoves(movementStopCondition.RemainingMovements());
        }

        public void ReduceRemainingMoves(int amount)
        {
            onRemainingMovesDecreased.Invoke();
            pendingMoves += amount;
            UpdateRemainingMoves(movementStopCondition.RemainingMovements());
        }
    }
}