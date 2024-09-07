using System;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Cells;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{
    public class HedgesBounceController : MonoBehaviour
    {
        CellStack[] cellStacks;
        GameBoard gameBoard;

        public void Setup(GameplayController gameplayController)
        {
            this.cellStacks = gameplayController.GameBoard().ArrbitrayCellStackArray();
            this.gameBoard = gameplayController.GameBoard();

            bool shouldBeEnabled = false;

            foreach (var cellStack in cellStacks)
                if (QueryUtilities.HasCellOnTop<HedgeCell>(cellStack))
                {
                    shouldBeEnabled = true;
                    break;
                }

            this.enabled = shouldBeEnabled;
        }

        private void Update()
        {
            foreach (var cellStack in cellStacks)
            {
                if (cellStack.HasTileStack() && IsFalling(cellStack.CurrentTileStack()))
                {
                    var tileStack = cellStack.CurrentTileStack();

                    var estimatedCellStack = DetermineCurrentCellStackBasedOnPositionOf(tileStack);
                    if (estimatedCellStack != null)
                        TryPlayBounceForHedge(estimatedCellStack, tileStack);
                }
            }
        }

        private void TryPlayBounceForHedge(CellStack cellStack, TileStack tileStack)
        {
            var hedgeCell = QueryUtilities.FindCell<HedgeCell>(cellStack);
            if (hedgeCell != null)
                hedgeCell.GetComponent<HedgeCellPresenter>().TryPlayBounceFor(tileStack);
        }

        private CellStack DetermineCurrentCellStackBasedOnPositionOf(TileStack tileStack)
        {
            var tileStackPosition = tileStack.Position();
            var estimatedCellStackPosition = new Vector2Int(Mathf.FloorToInt(tileStackPosition.x), Mathf.FloorToInt(tileStackPosition.y));
            return gameBoard.CellStackBoard()[estimatedCellStackPosition];
        }

        private bool IsFalling(TileStack tileStack)
        {
            return tileStack.componentCache.tileStackPhysicalState.PhysicsState() == Game.Gameplay.Physics.PhysicsState.Falling;
        }
    }
}