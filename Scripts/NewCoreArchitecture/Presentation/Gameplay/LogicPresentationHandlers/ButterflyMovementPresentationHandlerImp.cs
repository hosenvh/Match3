using System;
using System.Collections.Generic;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.ButterflyMechanic;
using UnityEngine;
using DG.Tweening;
using Match3.Presentation.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Tiles;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class ButterflyMovementPresentationHandlerImp : MonoBehaviour, ButterflyMovementPresentationHandler
    {

        public float movementDuration;
        public BoardPresenterNew boardPresenter;

        Counter counter;

        public void Move(List<ButterflyMovementGroup> movementGroups, Action<ButterflyMovementGroup> onMoveCompleted, Action onCompleted)
        {
            counter = new Counter(movementGroups.Count, onCompleted);

            foreach (var goup in movementGroups)
                MoveGroup(goup, onMoveCompleted);
            
        }

        private void MoveGroup(ButterflyMovementGroup group, Action<ButterflyMovementGroup> onMoveCompleted)
        {
            var sequence = DOTween.Sequence();

            for (int i = 0; i < group.butterfliesTilestacks.Count - 1; ++i)
            {
                sequence.Insert(0, CreateMoveTween(group.butterfliesTilestacks[i], group.butterfliesTilestacks[i + 1].Parent()));
            }

            sequence.Insert(0, CreateMoveTween(group.Head(), group.topTileStack.Parent()));
            sequence.Insert(0, CreateMoveTween(group.topTileStack, group.Tail().Parent()));

            sequence.OnComplete(() => HandleMoveCompletion(group, onMoveCompleted));

            foreach (var tileStack in group.butterfliesTilestacks)
                tileStack.Top().GetComponent<ButterflyTilePresenter>().PlayFlyEffect();
        }

        private Tween CreateMoveTween(TileStack tileStack, CellStack targetCellStack)
        {
            var tileStackPresenter = tileStack.GetComponent<TileStackPresenter>();
            var cellStackPresenter = targetCellStack.GetComponent<CellStackPresenter>();

            tileStackPresenter.SetLogicPositionUpdateFlag(false);


            if (targetCellStack.HasAttachment<PortalEntrance>() && tileStack.Parent().HasAttachment<PortalExit>())
                tileStackPresenter.transform.position = ToPresentationalPosition(targetCellStack.Position() + Vector2Int.up);
            else if(targetCellStack.HasAttachment<PortalExit>() && tileStack.Parent().HasAttachment<PortalEntrance>())
                tileStackPresenter.transform.position = ToPresentationalPosition(targetCellStack.Position() + Vector2Int.down);


            return tileStackPresenter.transform.DOLocalMove(cellStackPresenter.transform.localPosition, movementDuration);
        }

        Vector2 ToPresentationalPosition(Vector2 logicalPos)
        {
            return boardPresenter.BoardDimensions().LogicalPosToPresentaionalPos(logicalPos);
        }

        private void HandleMoveCompletion(ButterflyMovementGroup group, Action<ButterflyMovementGroup> onMoveCompleted)
        {
            foreach (var tileStack in group.butterfliesTilestacks)
                tileStack.GetComponent<TileStackPresenter>().SetLogicPositionUpdateFlag(true);

            group.topTileStack.GetComponent<TileStackPresenter>().SetLogicPositionUpdateFlag(true);

            foreach (var tileStack in group.butterfliesTilestacks)
                tileStack.Top().GetComponent<ButterflyTilePresenter>().PlayIdleEffect();

            onMoveCompleted(group);
            counter.Decrement();
        }
    }
}