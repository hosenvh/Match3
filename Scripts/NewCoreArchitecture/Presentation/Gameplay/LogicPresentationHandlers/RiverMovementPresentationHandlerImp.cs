using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.SubSystems.RiverMechanic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Match3.Presentation.Gameplay.Core;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Presentation.Gameplay.CellAttachments;
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    public class RiverMovementPresentationHandlerImp : MonoBehaviour, RiverMovementPresentationHandler
    {
        public GameplayStateRoot gameplayRoot;
        public float movementDuration;

        CellStackBoard cellStackBoard;

        public void MoveRiverCells(List<RiverCell> riverCells, Action onCompleted)
        {
            cellStackBoard = gameplayRoot.
                gameplayState.
                gameplayController.
                GameBoard().
                CellStackBoard();

            foreach (var riverCell in riverCells)
            {
                var tileStack = riverCell.Parent().CurrentTileStack();
                var attachments = riverCell.Parent().Attachments();
                var currentCell = riverCell.Parent();
                var nextCell = riverCell.NextRiverCell().Parent();
                var nextCellStackPresenter = nextCell.GetComponent<CellStackPresenter>();
                var currentCellStackPresenter = currentCell.GetComponent<CellStackPresenter>();

                if (tileStack!= null && tileStack.IsDestroyed() == false)
                {
                    var tileStackPresenter = riverCell.Parent().CurrentTileStack().GetComponent<TileStackPresenter>();
                    tileStackPresenter.SetLogicPositionUpdateFlag(false);


                    if (AreAdjacent(currentCell, nextCell))
                        MoveDirectlyTo(tileStackPresenter.transform, nextCellStackPresenter);
                    else
                        TeleportTo(tileStackPresenter.transform, currentCellStackPresenter, nextCellStackPresenter);
                }

                foreach (var attachment in attachments)
                {
                    if (ShouldMove(attachment) && attachment is BasicEntity attachmentEntity)
                    {
                        var presenter = attachmentEntity.GetComponent<CellAttachmentPresenter>();

                        if (AreAdjacent(currentCell, nextCell))
                            MoveDirectlyTo(presenter.transform, nextCellStackPresenter);
                        else
                            TeleportTo(presenter.transform, currentCellStackPresenter, nextCellStackPresenter);
                    }
                }

            }
            ServiceLocator.Find<UnityTimeScheduler>().Schedule(movementDuration, () => OnComplete(riverCells, onCompleted), this);
        }

        // TODO: Should I link it to RiverMovementSystem.ShouldMove?
        private bool ShouldMove(CellAttachment attachment)
        {
            return attachment is LilyPad || attachment is LilyPadBud;
        }

        private void TeleportTo(Transform transform, CellStackPresenter currentCellStackPresenter, CellStackPresenter nextCellStackPresenter)
        {
            var exitDirection = DetermineEmptyDirection(currentCellStackPresenter.CellStack().Position());
            var enterDirection = DetermineEmptyDirection(nextCellStackPresenter.CellStack().Position());

            var exitPosision = transform.position;
            exitPosision +=  exitDirection * (gameplayRoot.boardPresenter.BoardDimensions().tileSize.x/2);
            var enterPosition = nextCellStackPresenter.transform.position;
            enterPosition += enterDirection * (gameplayRoot.boardPresenter.BoardDimensions().tileSize.x/2);


            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(
                transform.DOMove(exitPosision, movementDuration/2).OnComplete(() => transform.position = enterPosition));
            mySequence.Append(
                transform.DOMove(nextCellStackPresenter.transform.position, movementDuration/2));

        }

        Vector3 DetermineEmptyDirection(Vector2Int pos)
        {
            var left = cellStackBoard.DirectionalElementOf(pos, Direction.Left);
            var right = cellStackBoard.DirectionalElementOf(pos, Direction.Right);
            var up = cellStackBoard.DirectionalElementOf(pos, Direction.Up);
            var down = cellStackBoard.DirectionalElementOf(pos, Direction.Down);

            if (IsEmpty(left))
                return Vector3.left;
            if (IsEmpty(right))
                return Vector3.right;
            if (IsEmpty(up))
                return Vector3.up;
            if (IsEmpty(down))
                return Vector3.down;

            return new Vector3(1, 0, 0);
        }

        bool IsEmpty(CellStack cell)
        {
            return cell == null || cell.Top() is EmptyCell;
        }


        private void MoveDirectlyTo(Transform transform, CellStackPresenter nextCellStackPresenter)
        {
            transform.DOMove(nextCellStackPresenter.transform.position, movementDuration);
        }

        private bool AreAdjacent(CellStack cellStack1, CellStack cellStack2)
        {
            return cellStackBoard.AreAdjacent(
                cellStack1.Position(),
                cellStack2.Position());
        }

        void OnComplete(List<RiverCell> riverCells, Action onCompleted)
        {
            onCompleted();
            foreach (var riverCell in riverCells)
            {
                if (riverCell.Parent().HasTileStack())
                {
                    var tileStackPresenter = riverCell.Parent().CurrentTileStack().GetComponent<TileStackPresenter>();
                    tileStackPresenter.SetLogicPositionUpdateFlag(true);
                }
            }


        }

        void OnDestroy()
        {
            ServiceLocator.Find<UnityTimeScheduler>().UnSchedule(this);
        }

    }
}